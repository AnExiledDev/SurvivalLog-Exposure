using System;
using BepInEx.Unity.IL2CPP;
using Exposure.Core;
using GameCore.HotUpdate;
using GameCore.HotUpdate.Battle.Logic;
using HarmonyLib;

namespace Exposure.Adapters;

/// <summary>
/// Adds up the exposure each source raised from a trip's start to its end, and once the player is home writes it, with
/// what came home, to the log and shows it as a notice. Everything here runs on the main thread: the feed's events
/// inside the game's update, the checks in <see cref="TripSummaryTicker"/>.
/// </summary>
internal static class TripSummary
{
    private const string ToastKey = "exposure.trip";

    private static readonly TripTally Tally = new();

    private static TripLoot? _loot;
    private static SummaryWait? _wait;
    private static bool _isSummaryOn = true;
    private static Action<string> _log = _ => { };
    private static bool _hasReportedFailure;

    public static void Start(BasePlugin plugin, Harmony harmony, Action<string> log)
    {
        _log = log;
        ExposureFeed.TripStarted += OnTripStarted;
        ExposureFeed.Gained += Tally.Add;
        ExposureFeed.TripEnded += OnTripEnded;
        TripSummaryLoot.Apply(harmony, loot => _loot = loot, log);
        plugin.AddComponent<TripSummaryTicker>();
    }

    /// <summary>Whether the summary shows as a notice; it is logged either way.</summary>
    public static void UseSummary(bool isOn) => _isSummaryOn = isOn;

    public static void Check(float now)
    {
        if (_wait == null) return;

        try
        {
            var step = TripTallyDelivery.Next(_wait, now, IsHomeReady());

            _wait = step.Wait;
            Carry(step.Action);
        }
        catch (Exception ex)
        {
            _wait = null;
            ReportOnce($"could not show the trip summary, logged once: {ex.Message}");
            _log(TripTallyText.LogLine(Tally, _loot));
        }
    }

    private static void OnTripStarted()
    {
        if (_wait != null) _log(TripTallyText.LogLine(Tally, _loot) + " (not shown: another expedition started first)");

        Tally.Start();
        _loot = null;
        _wait = null;
    }

    /// <summary>A trip ends once; a second end without a new start is the same trip.</summary>
    private static void OnTripEnded()
    {
        if (!Tally.IsCollecting) return;

        Tally.End();
        _wait = new SummaryWait(UnityEngine.Time.unscaledTime, null);
    }

    private static void Carry(DeliveryAction action)
    {
        switch (action)
        {
            case DeliveryAction.None:
                break;
            case DeliveryAction.Show:
                Deliver();
                break;
            case DeliveryAction.GiveUp:
                _log(TripTallyText.LogLine(Tally, _loot) + $" (not shown: home did not load within {TripTallyDelivery.GiveUpSeconds:0} seconds)");
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(action), action, "a delivery action with no handling");
        }
    }

    private static void Deliver()
    {
        _log(TripTallyText.LogLine(Tally, _loot));

        if (!_isSummaryOn) return;

        var isChinese = GameLanguage.IsChinese();

        WarningsToast.Show(TripTallyText.Title(isChinese), TripTallyText.Body(Tally, _loot, isChinese), isWarning: false, ToastKey, _log);
    }

    /// <summary>Home is up once the game has made a world that is not an expedition's and started its clock.</summary>
    private static bool IsHomeReady()
    {
        if (!BaseSingleton<BattleLogicWorld>.IsInstanceCreated) return false;

        var world = BaseSingleton<BattleLogicWorld>.Instance;

        return world != null && world.GameModeKey != GameMode.Explore && world._GameTimeManager?.IsGameStart == true;
    }

    private static void ReportOnce(string message)
    {
        if (_hasReportedFailure) return;

        _hasReportedFailure = true;
        _log(message);
    }
}
