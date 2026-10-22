using System;
using Exposure.Core;
using GameCore.HotUpdate.Battle.Logic;

namespace Exposure.Adapters;

/// <summary>
/// Watches the bar on an expedition a few times a second and, the first time on a trip it reaches a warning or pause
/// percent, logs it, plays the game's exposure alarm, pauses the game if that percent pauses, and shows a notice.
/// Each of those fails on its own, so a sound that fails still leaves the notice.
/// </summary>
internal static class Warnings
{
    private const float CheckSeconds = 0.25f;
    private const string ToastKey = "exposure.warning";

    private static WarningSettings _settings = new(new WarningLevels(Array.Empty<int>(), 0), IsToastOn: false, IsSoundOn: false);
    private static WarningWatch _watch = WarningWatch.Start;
    private static float _sinceCheck = CheckSeconds;
    private static Action<string> _log = _ => { };
    private static bool _hasReportedFailure;

    public static void Start(Action<string> log)
    {
        _log = log;
        ExposureFeed.TripStarted += OnTripStarted;
        ExposureFeed.Ticked += OnTicked;
    }

    /// <summary>Used from the next check. Set on the main thread, where the checks run.</summary>
    public static void UseSettings(WarningSettings settings) => _settings = settings;

    private static void OnTripStarted()
    {
        _watch = WarningWatch.Start;
        _sinceCheck = CheckSeconds;
    }

    private static void OnTicked(ExploreManager manager, float unscaledSeconds)
    {
        _sinceCheck += unscaledSeconds;

        if (_sinceCheck < CheckSeconds) return;

        _sinceCheck = 0;
        Attempt("read the exposure bar", () => Check(manager));
    }

    private static void Check(ExploreManager manager)
    {
        var settings = _settings;
        var (exposure, max) = WarningsGame.ReadBar(manager);
        var step = WarningRules.Next(_watch, WarningRules.Percent(exposure, max), settings.Levels);

        _watch = step.Watch;

        if (step.ReachedAt is not { } percent) return;

        var outcome = PauseOutcome.NotDue;

        if (step.IsPauseDue) Attempt("pause the game", () => outcome = WarningsGame.Pause(manager));

        _log(WarningText.LogLine(percent, exposure, max, outcome));

        if (settings.IsSoundOn) Attempt("play the exposure alarm", () => WarningsGame.PlayAlarm(manager));

        if (settings.IsToastOn) ShowToast(percent, outcome);
    }

    private static void ShowToast(int percent, PauseOutcome outcome)
    {
        var isChinese = GameLanguage.IsChinese();

        WarningsToast.Show(WarningText.Title(percent, isChinese), WarningText.Body(outcome, isChinese), isWarning: true, ToastKey, _log);
    }

    /// <summary>A step that fails is logged once and the warning goes on without it.</summary>
    private static void Attempt(string what, Action step)
    {
        try
        {
            step();
        }
        catch (Exception ex)
        {
            if (_hasReportedFailure) return;

            _hasReportedFailure = true;
            _log($"warning could not {what}, logged once: {ex.Message}");
        }
    }
}
