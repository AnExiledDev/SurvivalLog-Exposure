using System;
using Exposure.Core;
using GameCore.HotUpdate;
using GameCore.HotUpdate.Battle.Logic;
using HarmonyLib;

namespace Exposure.Adapters;

/// <summary>
/// ExploreManager.Update adds TimeExposure, and MoveExposure while running, every frame; AddExposure is the one
/// funnel for searches, loot (reward sets) and event effects. The per-frame rates are swapped for the slowed ones
/// around the game's update and put back after, so the game's own value is never lost. A one-off add is slowed
/// before the game applies it, since AddExposure updates the bar and checks for the end of the trip itself.
/// Each rise is then measured on the bar and told to the feed, after the game's work and apart from it.
/// </summary>
internal static class ExposurePatch
{
    private static readonly TripTracker Trip = new();
    private static readonly FailureLog ChangeFailure = new("exposure change failed, the game's exposure is used where it failed");
    private static readonly FailureLog FeedFailure = new("exposure feed failed, the meter and trip summary may miss some exposure");
    private static readonly FailureLog DayFailure = new("the game's day could not be read, the day ramp is left out of danger");
    private static ExposureSettings _settings = new(1, true, 1);
    private static double _carried;
    private static double _danger = 1;
    private static TripPlace? _place;
    private static IntPtr _tripManager;
    private static PendingFrame? _frame;
    private static PendingOneOff? _oneOff;

    public struct Rates
    {
        public bool IsUnlocked;
        public bool IsSwapped;
        public float GameTime;
        public float GameRun;
        public float SlowTime;
        public float SlowRun;
    }

    private readonly record struct TripPlace(int? Day, int MapId);

    /// <summary>The bar before the game's update, to measure what the update added.</summary>
    private struct PendingFrame
    {
        public float Before;
        public float TimeRate;
        public float RunRate;
        public bool IsRunning;
    }

    private readonly record struct PendingOneOff(ExposureSource Source, float Before);

    public static void Apply(Harmony harmony, Action<string> log)
    {
        Hooks.Log = log;
        Hooks.Patch(harmony, typeof(ExploreManager), nameof(ExploreManager.Update), typeof(ExposurePatch), nameof(BeforeUpdate), nameof(AfterUpdate), "time and running exposure keep the game's rate");
        Hooks.Patch(harmony, typeof(ExploreManager), nameof(ExploreManager.AddExposure), typeof(ExposurePatch), nameof(BeforeAdd), nameof(AfterAdd), "searching and looting keep the game's exposure");
        SourcePatch.Apply(harmony);
        TripPatch.Apply(harmony);
        ExposureFeed.PreviewOneOff = Preview;
    }

    /// <summary>Used from the next frame. Set on the main thread, where the game updates exposure.</summary>
    public static void UseSettings(ExposureSettings settings)
    {
        _settings = settings;
        _danger = _place is { } place ? settings.Danger.Multiplier(place.Day, place.MapId) : 1;
    }

    /// <summary>A new explore manager means a new expedition, so an expedition still open is ended first.</summary>
    public static void UseManager(ExploreManager manager)
    {
        EndTrip(null);
        Trip.Reset();
        _tripManager = manager.Pointer;
        _place = null;
        _danger = 1;
        _frame = null;
        _oneOff = null;
    }

    /// <summary>Tells the feed what the bar gained so far, then that the expedition ended; only the first end of a trip counts.</summary>
    public static void EndTrip(ExploreManager? manager)
    {
        if (manager != null)
        {
            FlushOneOff(manager);
            FlushFrame(manager);
        }

        if (Trip.TryEnd()) Notify(ExposureFeed.RaiseTripEnded);
    }

    private static int Preview(ExposureSource source, int value) => _settings.OneOff(source, value, _carried, _danger).Value;

    private static void BeforeUpdate(ExploreManager __instance, out Rates __state)
    {
        __state = default;

        try
        {
            if (__instance._IsLocked_k__BackingField) return;

            __state.IsUnlocked = true;
            StartTripIfNew(__instance);
            __state.GameTime = __instance.TimeExposure;
            __state.GameRun = __instance.MoveExposure;
            __state.SlowTime = _settings.TimeRate(__state.GameTime, _danger);
            __state.SlowRun = _settings.RunRate(__state.GameRun, _danger);
            __instance.TimeExposure = __state.SlowTime;
            __instance.MoveExposure = __state.SlowRun;
            __state.IsSwapped = true;
            _frame = new PendingFrame
            {
                Before = __instance._CurExposure_k__BackingField,
                TimeRate = __state.SlowTime,
                RunRate = __state.SlowRun,
                IsRunning = __instance._IsRunning_k__BackingField,
            };
        }
        catch (Exception ex)
        {
            ChangeFailure.Report(ex);
        }
    }

    /// <summary>A rate the game changed during its update is its new value, so only an untouched one is put back.</summary>
    private static void AfterUpdate(ExploreManager __instance, Rates __state, float unscaledDeltaTime)
    {
        if (__state.IsSwapped) RestoreRates(__instance, __state);

        if (!__state.IsUnlocked) return;

        FlushFrame(__instance);

        try
        {
            ExposureFeed.RaiseTicked(__instance, unscaledDeltaTime);
        }
        catch (Exception ex)
        {
            FeedFailure.Report(ex);
        }
    }

    private static void RestoreRates(ExploreManager manager, Rates rates)
    {
        try
        {
            if (manager.TimeExposure == rates.SlowTime) manager.TimeExposure = rates.GameTime;
            if (manager.MoveExposure == rates.SlowRun) manager.MoveExposure = rates.GameRun;
        }
        catch (Exception ex)
        {
            ChangeFailure.Report(ex);
        }
    }

    private static void BeforeAdd(ExploreManager __instance, ref int value)
    {
        var source = SourcePatch.Current;

        (value, _carried) = _settings.OneOff(source, value, _carried, _danger);

        try
        {
            _oneOff = new PendingOneOff(source, __instance._CurExposure_k__BackingField);
        }
        catch (Exception ex)
        {
            FeedFailure.Report(ex);
        }
    }

    private static void AfterAdd(ExploreManager __instance) => FlushOneOff(__instance);

    private static void StartTripIfNew(ExploreManager manager)
    {
        if (manager.Pointer != _tripManager) UseManager(manager);

        if (!Trip.TryStart(manager._hasEndProcessed)) return;

        var place = new TripPlace(ReadDay(), manager._MapConfigId_k__BackingField);

        _place = place;
        _danger = _settings.Danger.Multiplier(place.Day, place.MapId);
        Hooks.Log($"expedition at location {place.MapId} on day {place.Day?.ToString() ?? "unknown"}, danger x{_danger:0.##}");
        Notify(ExposureFeed.RaiseTripStarted);
    }

    /// <summary>The day the game shows: its clock counts from 0 and the game adds 1 wherever it shows or compares days.</summary>
    private static int? ReadDay()
    {
        try
        {
            var clock = BaseSingleton<BattleLogicWorld>.IsInstanceCreated ? BaseSingleton<BattleLogicWorld>.Instance?._GameTimeManager : null;

            return clock == null ? null : clock.GetDay() + 1;
        }
        catch (Exception ex)
        {
            DayFailure.Report(ex);

            return null;
        }
    }

    /// <summary>A one-off add inside the game's update is taken off the frame's rise, so it is not told twice.</summary>
    private static void FlushOneOff(ExploreManager manager)
    {
        if (_oneOff is not { } add) return;

        _oneOff = null;

        try
        {
            var added = manager._CurExposure_k__BackingField - add.Before;

            if (_frame is { } frame)
            {
                frame.Before += added;
                _frame = frame;
            }

            if (added > 0) Notify(add.Source, added);
        }
        catch (Exception ex)
        {
            FeedFailure.Report(ex);
        }
    }

    private static void FlushFrame(ExploreManager manager)
    {
        if (_frame is not { } frame) return;

        _frame = null;

        try
        {
            var gained = manager._CurExposure_k__BackingField - frame.Before;
            var (time, running) = ExposureGains.SplitFrame(gained, frame.TimeRate, frame.RunRate, frame.IsRunning);

            if (time > 0) Notify(ExposureSource.Time, time);
            if (running > 0) Notify(ExposureSource.Running, running);
        }
        catch (Exception ex)
        {
            FeedFailure.Report(ex);
        }
    }

    private static void Notify(ExposureSource source, double amount)
    {
        try
        {
            ExposureFeed.RaiseGained(source, amount);
        }
        catch (Exception ex)
        {
            FeedFailure.Report(ex);
        }
    }

    private static void Notify(Action raise)
    {
        try
        {
            raise();
        }
        catch (Exception ex)
        {
            FeedFailure.Report(ex);
        }
    }
}
