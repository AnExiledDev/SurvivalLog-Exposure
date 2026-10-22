using System.Linq;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using Exposure.Adapters;
using Exposure.Core;
using HarmonyLib;

namespace Exposure;

[BepInPlugin(Guid, "Exposure", Version)]
[BepInDependency(OptionalLive.ToolkitGuid, BepInDependency.DependencyFlags.SoftDependency)]
public sealed class Plugin : BasePlugin
{
    private const string Guid = "com.dev.survivallog.exposure";
    private const string Version = "0.3.0";

    private const string Range = "From 1 to 100, or from 0.25 when AllowFasterThanGame is on.";
    private const string RangeZh = "范围 1 到 100，开启「AllowFasterThanGame」时最低为 0.25。";
    private const string OwnSlowdownNote = "0 uses LootSlowdown. " + Range;
    private const string OwnSlowdownNoteZh = "设为 0 时使用「LootSlowdown」。" + RangeZh;
    private const string CustomOnly = "Used with the Custom preset only.";
    private const string CustomOnlyZh = "仅在「Custom」预设下生效。";

    public override void Load()
    {
        if (!OptionalLive.IsToolkitInstalled) Log.LogInfo(OptionalLive.MissingNote);

        var preset = Config.Bind("Exposure", "Preset", nameof(ExposurePreset.Custom), new ConfigDescription(
            "A ready-made set of exposure settings. Custom uses the settings below as you set them. Vanilla is the game's own exposure. Relaxed halves time and one-off exposure, and running adds none. Tourist gives a fifth of the game's exposure, and running adds none. Hardcore makes time and one-off exposure rise a third faster than in the game, and running adds exposure at the game's rate. Any preset other than Custom replaces every other setting here and in Danger.\n" +
            "预设的暴露值方案。「Custom」使用下方各项设置。「Vanilla」为游戏原本的暴露值。「Relaxed」随时间和一次性增加的暴露值减半，跑步不增加暴露值。「Tourist」暴露值只有游戏的五分之一，跑步不增加暴露值。「Hardcore」随时间和一次性增加的暴露值比游戏快三分之一，跑步按游戏原速增加暴露值。选择「Custom」以外的预设时，会替换此处和「Danger」中的所有其他设置。",
            new AcceptableValueList<string>(ExposurePresets.Names.ToArray())));
        var timeSlowdown = Config.Bind("Exposure", "TimeSlowdown", 2.0,
            $"On an expedition, exposure from time spent out rises this many times slower. 1 is the game's rate. {Range}\n" +
            $"探索时，随时间上涨的暴露值，上涨速度会除以这个数。1 为游戏原速。{RangeZh}");
        var isRunningExposed = Config.Bind("Exposure", "RunningAddsExposure", false,
            "When false, running on an expedition adds no exposure. When true, running adds exposure as in the game.\n" +
            "设为 false 时，探索时跑步不会增加暴露值。设为 true 时，跑步和游戏原本一样增加暴露值。");
        var lootSlowdown = Config.Bind("Exposure", "LootSlowdown", 2.0,
            $"Exposure added at once by searching, picking up, breaking in and events is divided by this, unless that kind has its own setting below. 1 is the game's amount. {Range}\n" +
            $"翻找、拾取、撬开和事件一次性增加的暴露值会除以这个数，除非该类在下方有单独设置。1 为游戏原量。{RangeZh}");
        var searchSlowdown = Config.Bind("Exposure", "SearchSlowdown", 0.0,
            $"Exposure added by searching and looking over furniture is divided by this. {OwnSlowdownNote}\n" +
            $"搜索、翻找和查看家具增加的暴露值会除以这个数。{OwnSlowdownNoteZh}");
        var pickupSlowdown = Config.Bind("Exposure", "PickupSlowdown", 0.0,
            $"Exposure added by picking up and taking out supplies is divided by this. {OwnSlowdownNote}\n" +
            $"拾取和取出物资增加的暴露值会除以这个数。{OwnSlowdownNoteZh}");
        var breakInSlowdown = Config.Bind("Exposure", "BreakInSlowdown", 0.0,
            $"Exposure added by prying and forcing doors and locks open, unlocking, tearing down and swiping cards is divided by this. {OwnSlowdownNote}\n" +
            $"撬开、强行开启、解锁、拆除和刷卡增加的暴露值会除以这个数。{OwnSlowdownNoteZh}");
        var eventSlowdown = Config.Bind("Exposure", "EventSlowdown", 0.0,
            $"Exposure added by events and story rewards on an expedition is divided by this. {OwnSlowdownNote}\n" +
            $"探索时事件和剧情奖励增加的暴露值会除以这个数。{OwnSlowdownNoteZh}");
        var isFasterAllowed = Config.Bind("Exposure", "AllowFasterThanGame", false,
            "When true, a slowdown below 1 makes exposure rise faster than in the game, down to 0.25, which is four times as fast. When false, no setting makes exposure rise faster than in the game. The Hardcore preset allows it by itself.\n" +
            "设为 true 时，低于 1 的数值会让暴露值比游戏上涨更快，最低 0.25，即四倍速。设为 false 时，任何设置都不会让暴露值比游戏上涨更快。「Hardcore」预设会自动允许。");
        var dangerStart = Config.Bind("Danger", "StartMultiplier", 1.0,
            $"How many times faster exposure rises on day 1, on top of the Exposure settings. 1 changes nothing, 2 is twice as fast, 0.5 is half as fast. From 0.1 to 10. {CustomOnly}\n" +
            $"第 1 天暴露值上涨的倍数，在「Exposure」设置之上再计算。1 不变，2 为两倍速，0.5 为一半。范围 0.1 到 10。{CustomOnlyZh}");
        var dangerEnd = Config.Bind("Danger", "EndMultiplier", 1.0,
            $"How many times faster exposure rises from day RampDays on. Between day 1 and that day it changes evenly from StartMultiplier to this. From 0.1 to 10. {CustomOnly}\n" +
            $"从第「RampDays」天起暴露值上涨的倍数。第 1 天到那一天之间，从「StartMultiplier」均匀变化到这个数。范围 0.1 到 10。{CustomOnlyZh}");
        var rampDays = Config.Bind("Danger", "RampDays", 30,
            $"The day on which EndMultiplier is reached. 1 uses EndMultiplier from the first day. {CustomOnly}\n" +
            $"达到「EndMultiplier」的天数。设为 1 时从第一天起使用「EndMultiplier」。{CustomOnlyZh}");
        var locations = Config.Bind("Danger", "Locations", "",
            "Extra multipliers for single locations, written as id=multiplier and separated by commas, for example 1002=1.5, 1005=2. Ids: 1000 Community Maintenance Hut, 1001 Gas Station, 1002 Abandoned School, 1003 Tech Company, 1004 Deserted Supermarket, 1005 Abandoned Hospital. Applied on top of the day multiplier. From 0.1 to 10. " + CustomOnly + "\n" +
            "单个地点的额外倍数，格式为 编号=倍数，用英文逗号分隔，例如 1002=1.5, 1005=2。编号：1000「社区检修小屋」，1001「加油站」，1002「废弃学校」，1003「科技公司」，1004「残破超市」，1005「废弃医院」。在天数倍数之上再计算。范围 0.1 到 10。" + CustomOnlyZh);

        Log.LogInfo($"Exposure {Version} started");
        MeterPage.Start(Config, Write);
        TripSummaryAndWarnings.Start(this, new Harmony(Guid), Write);
        ExposurePatch.Apply(new Harmony(Guid), Write);
        OptionalLive.Watch(() =>
        {
            var resolved = ExposurePresets.Resolve(new ExposureChoices(
                preset.Value,
                timeSlowdown.Value,
                isRunningExposed.Value,
                lootSlowdown.Value,
                searchSlowdown.Value,
                pickupSlowdown.Value,
                breakInSlowdown.Value,
                eventSlowdown.Value,
                isFasterAllowed.Value,
                dangerStart.Value,
                dangerEnd.Value,
                rampDays.Value,
                locations.Value));

            ExposurePatch.UseSettings(resolved.Settings);
            Log.LogInfo(Describe(resolved));

            if (resolved.IgnoredLocations.Count > 0) Log.LogWarning($"Danger Locations could not read {string.Join(", ", resolved.IgnoredLocations)}; write each as id=multiplier, such as 1002=1.5");
        }, preset, timeSlowdown, isRunningExposed, lootSlowdown, searchSlowdown, pickupSlowdown, breakInSlowdown, eventSlowdown, isFasterAllowed, dangerStart, dangerEnd, rampDays, locations);
    }

    private static string Describe(ResolvedExposure resolved)
    {
        var settings = resolved.Settings;
        var danger = settings.Danger;

        return $"preset {resolved.Preset}: time /{settings.TimeSlowdown}, running {(settings.IsRunningExposed ? "on" : "off")}, loot /{settings.LootSlowdown}, " +
               $"search {Own(settings.SearchSlowdown)}, pickup {Own(settings.PickupSlowdown)}, break-in {Own(settings.BreakInSlowdown)}, event {Own(settings.EventSlowdown)}, " +
               $"faster than the game {(settings.IsFasterThanGameAllowed ? "allowed" : "off")}, " +
               $"danger x{danger.StartMultiplier} on day 1 to x{danger.EndMultiplier} on day {danger.RampDays}, {danger.Locations.Count} location(s)";
    }

    private static string Own(double? slowdown) => slowdown is { } own ? $"/{own}" : "as loot";

    private void Write(string message)
    {
        if (HookFailure.Is(message)) Log.LogError(message);
        else Log.LogInfo(message);
    }
}
