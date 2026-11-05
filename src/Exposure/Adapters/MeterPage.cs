using System;
using BepInEx.Configuration;
using ExiledToolkit;
using ExiledToolkit.Game;
using ExiledToolkit.Web;
using Exposure.Core;
using GameCore.HotUpdate;
using GameCore.HotUpdate.Battle.Logic;

namespace Exposure.Adapters;

/// <summary>
/// Puts the bar's numbers and its time to full on the expedition meter, and each search's cost on the furniture
/// menu. The bar is read after every explore update the feed reports; page.js, run in the web UI's root document,
/// draws what this sends. The web view can reload and lose the script, so a probe asks every few seconds whether it is
/// still there, and the page's answer that it is not sends it again. Everything here runs on the main thread, inside
/// the game's update.
/// </summary>
internal static class MeterPage
{
    private const float PushSeconds = 0.5f;
    private const string ScriptResource = "Exposure.page.js";
    private const string Handle = "window.__slexposure";
    private const string NoScriptMessage = "slmod|exposure|noscript";
    private const string Section = "Meter";

    private static readonly MeterRate Rate = new();

    private static Action<string> _log = _ => { };
    private static MeterSettings _settings = new(true, true, true);
    private static WebPage? _page;
    private static bool _hasReportedFailure;
    private static double _clock;
    private static double _nextPush;
    private static string _lastSent = "";
    private static IntPtr _manager;
    private static MeterCostTable? _costs;

    public static void Start(ConfigFile config, Action<string> log)
    {
        _log = log;
        _page = new WebPage(typeof(MeterPage).Assembly, ScriptResource, Handle, NoScriptMessage, log);

        var showNumbers = config.Bind(Section, "ShowNumbers", true,
            "Show the exposure bar's points as numbers, such as 62 / 100, in place of the percent.\n" +
            "在暴露值条上用数字显示当前点数，例如 62 / 100，代替百分比。");
        var showTimeToFull = config.Bind(Section, "ShowTimeToFull", true,
            "Under the exposure bar, show about how long until it is full at the current pace, in real time. Hidden while the bar is not rising.\n" +
            "在暴露值条下方显示按当前速度大约多久后涨满（现实时间）。暴露值不上涨时不显示。");
        var showCosts = config.Bind(Section, "ShowCosts", true,
            "On an expedition, show how much exposure each search, open or pick up adds, such as +25, on its button in the furniture menu.\n" +
            "探索时，在家具菜单的按钮上显示翻找、开启或拾取会增加多少暴露值，例如 +25。");

        LiveSettings.Watch(() =>
        {
            _settings = new MeterSettings(showNumbers.Value, showTimeToFull.Value, showCosts.Value);
            _nextPush = 0;
        }, showNumbers, showTimeToFull, showCosts);

        PageInbox.Listen(NoScriptMessage, _ => Install());
        ExposureFeed.Ticked += OnTicked;
        ExposureFeed.TripEnded += OnTripEnded;
    }

    private static void OnTicked(ExploreManager manager, float unscaledSeconds)
    {
        try
        {
            Read(manager, unscaledSeconds);
            _page?.KeepInstalled(_clock);

            if (_clock >= _nextPush) Push(manager);
        }
        catch (Exception ex)
        {
            ReportOnce(ex);
        }
    }

    private static void OnTripEnded()
    {
        try
        {
            Rate.Reset();

            (_manager, _costs) = (IntPtr.Zero, null);

            Send(MeterPageState.Empty);
        }
        catch (Exception ex)
        {
            ReportOnce(ex);
        }
    }

    // The backing field, not the property: this runs every frame, and a property read goes through the game's runtime.
    private static void Read(ExploreManager manager, float unscaledSeconds)
    {
        if (manager.Pointer != _manager)
        {
            Rate.Reset();
            _manager = manager.Pointer;
        }

        _clock += unscaledSeconds;

        Rate.Add(unscaledSeconds, manager._CurExposure_k__BackingField);
    }

    // A reloaded page has the script again but none of the numbers, so they are sent again as well.
    private static void Install()
    {
        try
        {
            if (_page?.Install() == true) _lastSent = "";
        }
        catch (Exception ex)
        {
            ReportOnce(ex);
        }
    }

    private static void Push(ExploreManager manager)
    {
        _nextPush = _clock + PushSeconds;

        var settings = _settings;
        var reading = new MeterReading(manager._CurExposure_k__BackingField, manager.MaxExposure, Rate.PointsPerSecond);
        var costs = settings.ShowCosts ? CostTable()?.Preview(ExposureFeed.PreviewOneOff) : null;

        Send(MeterPageState.Json(settings, reading, costs, GameLanguage.IsChinese()));
    }

    /// <summary>Built once an expedition, after the game has loaded its tables; tried again on the next push until then.</summary>
    private static MeterCostTable? CostTable() => _costs ??= MeterCostTable.Read(GameSingleton.Existing<ConfigManager>());

    private static void Send(string json)
    {
        if (json != _lastSent && _page?.Run(Handle + "&&" + Handle + ".set(" + json + ")") == true) _lastSent = json;
    }

    private static void ReportOnce(Exception ex)
    {
        if (_hasReportedFailure) return;

        _hasReportedFailure = true;
        _log("the exposure meter could not be updated, the game's meter is shown where it failed (reported once): " + ex.Message);
    }
}
