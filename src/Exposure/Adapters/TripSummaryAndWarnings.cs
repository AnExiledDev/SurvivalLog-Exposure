using System;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;

namespace Exposure.Adapters;

/// <summary>Starts the threshold warnings and the trip summary, both fed by the exposure hooks, and binds their settings.</summary>
internal static class TripSummaryAndWarnings
{
    public static void Start(BasePlugin plugin, Harmony harmony, Action<string> log)
    {
        Warnings.Start(log);
        TripSummary.Start(plugin, harmony, log);
        WarningOptions.Bind(plugin.Config, Warnings.UseSettings, TripSummary.UseSummary, log);
    }
}
