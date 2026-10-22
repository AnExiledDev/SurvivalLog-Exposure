using BepInEx;
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
    private const string Version = "0.2.1";

    public override void Load()
    {
        if (!OptionalLive.IsToolkitInstalled) Log.LogInfo(OptionalLive.MissingNote);

        var timeSlowdown = Config.Bind("Exposure", "TimeSlowdown", 2.0,
            $"On an expedition, exposure from time spent out rises this many times slower. 1 is the game's rate. From {ExposureRules.MinSlowdown} to {ExposureRules.MaxSlowdown}.\n" +
            $"探索时，随时间上涨的暴露值，上涨速度会除以这个数。1 为游戏原速。范围 {ExposureRules.MinSlowdown} 到 {ExposureRules.MaxSlowdown}。");
        var isRunningExposed = Config.Bind("Exposure", "RunningAddsExposure", false,
            "When false, running on an expedition adds no exposure. When true, running adds exposure as in the game.\n" +
            "设为 false 时，探索时跑步不会增加暴露值。设为 true 时，跑步和游戏原本一样增加暴露值。");
        var lootSlowdown = Config.Bind("Exposure", "LootSlowdown", 2.0,
            $"Exposure added at once by searching, looting and events is divided by this. 1 is the game's amount. From {ExposureRules.MinSlowdown} to {ExposureRules.MaxSlowdown}.\n" +
            $"翻找、拾取和事件一次性增加的暴露值会除以这个数。1 为游戏原量。范围 {ExposureRules.MinSlowdown} 到 {ExposureRules.MaxSlowdown}。");

        Log.LogInfo($"Exposure {Version} started");
        // Meter and cost hints: one line here.
        // Warnings and trip summary: one line here.
        ExposurePatch.Apply(new Harmony(Guid), Write);
        OptionalLive.Watch(() =>
        {
            var settings = new ExposureSettings(ExposureRules.ClampSlowdown(timeSlowdown.Value), isRunningExposed.Value, ExposureRules.ClampSlowdown(lootSlowdown.Value));

            ExposurePatch.UseSettings(settings);
            Log.LogInfo($"time /{settings.TimeSlowdown}, running {(settings.IsRunningExposed ? "on" : "off")}, loot /{settings.LootSlowdown}");
        }, timeSlowdown, isRunningExposed, lootSlowdown);
    }

    private void Write(string message)
    {
        if (HookFailure.Is(message)) Log.LogError(message);
        else Log.LogInfo(message);
    }
}
