using System;
using BepInEx.Unity.IL2CPP;
using ExiledToolkit.Game;

namespace Exposure.Adapters;

/// <summary>Starts the threshold warnings and the trip summary, both fed by the exposure hooks, and binds their settings.</summary>
internal static class TripSummaryAndWarnings
{
    public static void Start(BasePlugin plugin, PatchKit hooks, Action<string> log)
    {
        Warnings.Start(log);
        TripSummary.Start(plugin, hooks, log);
        WarningOptions.Bind(plugin.Config, Warnings.UseSettings, TripSummary.UseSummary, log);
    }
}
