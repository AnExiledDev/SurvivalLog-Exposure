using System;
using Exposure.Core;
using GameCore.HotUpdate.Battle.Logic;
using HarmonyLib;

namespace Exposure.Adapters;

/// <summary>
/// ExploreManager.Update adds TimeExposure, and MoveExposure while running, every frame; AddExposure is the one
/// funnel for searches, loot (reward sets) and event effects. The per-frame rates are swapped for the slowed ones
/// around the game's update and put back after, so the game's own value is never lost.
/// </summary>
internal static class ExposurePatch
{
    private static ExposureSettings _settings = new(1, true, 1);
    private static Action<string> _log = _ => { };
    private static bool _hasReportedFailure;

    public struct Rates
    {
        public bool IsSwapped;
        public float GameTime;
        public float GameRun;
        public float SlowTime;
        public float SlowRun;
    }

    public static void Apply(Harmony harmony, ExposureSettings settings, Action<string> log)
    {
        _settings = settings;
        _log = log;
        Patch(harmony, nameof(ExploreManager.Update), nameof(BeforeUpdate), nameof(AfterUpdate), "time and running exposure keep the game's rate");
        Patch(harmony, nameof(ExploreManager.AddExposure), nameof(BeforeAdd), nameof(AfterAdd), "searching and looting keep the game's exposure");
    }

    private static void Patch(Harmony harmony, string method, string prefix, string postfix, string consequence)
    {
        try
        {
            harmony.Patch(
                AccessTools.Method(typeof(ExploreManager), method),
                prefix: new HarmonyMethod(typeof(ExposurePatch), prefix),
                postfix: new HarmonyMethod(typeof(ExposurePatch), postfix));
            _log($"hooked ExploreManager.{method}");
        }
        catch (Exception ex)
        {
            _log($"could not hook ExploreManager.{method}, {consequence}: {ex.Message}");
        }
    }

    private static void BeforeUpdate(ExploreManager __instance, out Rates __state)
    {
        __state = default;

        try
        {
            __state.GameTime = __instance.TimeExposure;
            __state.GameRun = __instance.MoveExposure;
            __state.SlowTime = ExposureRules.TimeRate(__state.GameTime, _settings.TimeSlowdown);
            __state.SlowRun = ExposureRules.RunRate(__state.GameRun, _settings.IsRunningExposed);
            __instance.TimeExposure = __state.SlowTime;
            __instance.MoveExposure = __state.SlowRun;
            __state.IsSwapped = true;
        }
        catch (Exception ex)
        {
            ReportOnce(ex);
        }
    }

    /// <summary>A rate the game changed during its update is its new value, so only an untouched one is put back.</summary>
    private static void AfterUpdate(ExploreManager __instance, Rates __state)
    {
        if (!__state.IsSwapped) return;

        try
        {
            if (__instance.TimeExposure == __state.SlowTime) __instance.TimeExposure = __state.GameTime;
            if (__instance.MoveExposure == __state.SlowRun) __instance.MoveExposure = __state.GameRun;
        }
        catch (Exception ex)
        {
            ReportOnce(ex);
        }
    }

    private static void BeforeAdd(ExploreManager __instance, out float __state)
    {
        __state = float.NaN;

        try
        {
            __state = __instance.CurExposure;
        }
        catch (Exception ex)
        {
            ReportOnce(ex);
        }
    }

    private static void AfterAdd(ExploreManager __instance, float __state)
    {
        if (float.IsNaN(__state)) return;

        try
        {
            __instance.CurExposure = ExposureRules.AfterOneOff(__state, __instance.CurExposure, _settings.LootSlowdown);
        }
        catch (Exception ex)
        {
            ReportOnce(ex);
        }
    }

    private static void ReportOnce(Exception ex)
    {
        if (_hasReportedFailure) return;

        _hasReportedFailure = true;
        _log($"exposure change failed, the game's exposure is used where it failed: {ex.Message}");
    }
}
