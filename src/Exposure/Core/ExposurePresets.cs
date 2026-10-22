using System;
using System.Collections.Generic;
using System.Linq;

namespace Exposure.Core;

/// <summary>Custom uses the player's own settings; every other preset replaces all of them.</summary>
public enum ExposurePreset
{
    Custom,
    Vanilla,
    Relaxed,
    Tourist,
    Hardcore,
}

/// <summary>The settings as the player wrote them, before a preset or any range is applied.</summary>
/// <param name="SearchSlowdown">0 or less follows <paramref name="LootSlowdown"/>; so do the other per-source slowdowns.</param>
public sealed record ExposureChoices(
    string Preset,
    double TimeSlowdown,
    bool IsRunningExposed,
    double LootSlowdown,
    double SearchSlowdown,
    double PickupSlowdown,
    double BreakInSlowdown,
    double EventSlowdown,
    bool IsFasterThanGameAllowed,
    double DangerStart,
    double DangerEnd,
    int RampDays,
    string Locations);

/// <param name="IgnoredLocations">Location entries that could not be read, so the player can be told.</param>
public sealed record ResolvedExposure(ExposurePreset Preset, ExposureSettings Settings, IReadOnlyList<string> IgnoredLocations);

public static class ExposurePresets
{
    private static readonly ExposurePreset[] All = (ExposurePreset[])Enum.GetValues(typeof(ExposurePreset));

    public static IReadOnlyList<string> Names { get; } = All.Select(preset => preset.ToString()).ToArray();

    /// <summary>A name the mod does not know reads as <see cref="ExposurePreset.Custom"/>, so the player's own settings apply.</summary>
    public static ExposurePreset Parse(string? name)
    {
        var trimmed = name?.Trim();

        return All.FirstOrDefault(preset => string.Equals(preset.ToString(), trimmed, StringComparison.OrdinalIgnoreCase));
    }

    public static ResolvedExposure Resolve(ExposureChoices choices)
    {
        var preset = Parse(choices.Preset);
        var locations = LocationList.Parse(choices.Locations);
        var settings = preset switch
        {
            ExposurePreset.Vanilla => new ExposureSettings(1, IsRunningExposed: true, 1),
            ExposurePreset.Relaxed => new ExposureSettings(2, IsRunningExposed: false, 2),
            ExposurePreset.Tourist => new ExposureSettings(5, IsRunningExposed: false, 5),
            ExposurePreset.Hardcore => new ExposureSettings(0.75, IsRunningExposed: true, 0.75) { IsFasterThanGameAllowed = true },
            _ => Custom(choices, locations),
        };

        return new ResolvedExposure(preset, settings, locations.Ignored);
    }

    private static ExposureSettings Custom(ExposureChoices choices, LocationList locations)
    {
        var floor = choices.IsFasterThanGameAllowed ? ExposureRules.FasterFloor : ExposureRules.MinSlowdown;
        var danger = new DangerRules(
            DangerRules.ClampMultiplier(choices.DangerStart),
            DangerRules.ClampMultiplier(choices.DangerEnd),
            Math.Max(1, choices.RampDays),
            locations.Multipliers);

        return new ExposureSettings(
            ExposureRules.ClampSlowdown(choices.TimeSlowdown, floor),
            choices.IsRunningExposed,
            ExposureRules.ClampSlowdown(choices.LootSlowdown, floor))
        {
            SearchSlowdown = OwnSlowdown(choices.SearchSlowdown, floor),
            PickupSlowdown = OwnSlowdown(choices.PickupSlowdown, floor),
            BreakInSlowdown = OwnSlowdown(choices.BreakInSlowdown, floor),
            EventSlowdown = OwnSlowdown(choices.EventSlowdown, floor),
            IsFasterThanGameAllowed = choices.IsFasterThanGameAllowed,
            Danger = danger,
        };
    }

    private static double? OwnSlowdown(double slowdown, double floor) =>
        double.IsNaN(slowdown) || slowdown <= 0 ? null : ExposureRules.ClampSlowdown(slowdown, floor);
}
