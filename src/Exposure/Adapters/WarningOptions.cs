using System;
using BepInEx.Configuration;
using Exposure.Core;

namespace Exposure.Adapters;

/// <summary>The [Warnings] and [Trip] settings, applied at start and again whenever one changes.</summary>
internal static class WarningOptions
{
    public static void Bind(ConfigFile config, Action<WarningSettings> useWarnings, Action<bool> useSummary, Action<string> log)
    {
        var thresholds = config.Bind("Warnings", "Thresholds", "50,75,90",
            "On an expedition, warn when exposure first reaches each of these percents of the bar. Whole numbers from 1 to 100, separated by commas. Each one warns once per trip. Leave empty for no warnings.\n" +
            "探索时，暴露值首次达到暴露值上限的这些百分比时发出提醒。填写 1 到 100 的整数，用逗号分隔。每个百分比在每次探索中只提醒一次。留空则不提醒。");
        var isToastOn = config.Bind("Warnings", "Toast", true,
            "Show a notice when exposure reaches a warning percent. Every warning is also written to the log.\n" +
            "暴露值达到提醒百分比时显示通知。每次提醒也都会写入日志。");
        var isSoundOn = config.Bind("Warnings", "Sound", true,
            "Play the game's own exposure alarm, the zombie roar, when exposure reaches a warning percent.\n" +
            "暴露值达到提醒百分比时，播放游戏自带的暴露警报声（丧尸的吼叫）。");
        var pauseAt = config.Bind("Warnings", "PauseAt", 0,
            $"Pause the game when exposure first reaches this percent of the bar on an expedition. 0 is off. The game itself cannot pause or carry on during an expedition, so this needs Time Control with TimeControlOnExpeditions on; without it the game is not paused and the warning says why. From 0 to {WarningRules.MaxPercent}.\n" +
            $"探索中暴露值首次达到暴露值上限的这个百分比时暂停游戏。0 为关闭。游戏本身在探索中无法暂停和继续，所以需要安装 Time Control 并开启 TimeControlOnExpeditions；否则不会暂停，提醒中会说明原因。范围 0 到 {WarningRules.MaxPercent}。");
        var isSummaryOn = config.Bind("Trip", "ShowSummary", true,
            "Once you are home from an expedition, show a notice with the exposure the trip added from time, running, searching and looting, events and anything else, and how many items you brought home and what they are worth. The summary is always written to the log.\n" +
            "探索结束回到家后显示一条通知，列出这次探索中时间、跑步、翻找和拾取、事件及其他来源增加的暴露值，以及带回家的物品数量和价值。总结总会写入日志。");

        OptionalLive.Watch(() =>
        {
            var levels = new WarningLevels(WarningRules.ParseThresholds(thresholds.Value), WarningRules.ClampPauseAt(pauseAt.Value));

            useWarnings(new WarningSettings(levels, isToastOn.Value, isSoundOn.Value));
            log($"warnings at {Describe(levels)}, notice {OnOff(isToastOn.Value)}, sound {OnOff(isSoundOn.Value)}, pause {(levels.PauseAt > 0 ? $"at {levels.PauseAt}%" : "off")}");
        }, thresholds, isToastOn, isSoundOn, pauseAt);
        OptionalLive.Watch(isSummaryOn, isOn =>
        {
            useSummary(isOn);
            log($"trip summary notice {OnOff(isOn)}");
        });
    }

    private static string Describe(WarningLevels levels) =>
        levels.Thresholds.Count > 0 ? string.Join(", ", levels.Thresholds) + "%" : "no percents";

    private static string OnOff(bool isOn) => isOn ? "on" : "off";
}
