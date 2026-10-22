using System;

namespace Exposure.Core;

/// <param name="TimeSlowdown">Exposure from time spent out rises this many times slower.</param>
/// <param name="IsRunningExposed">Whether running adds exposure on top of time.</param>
/// <param name="LootSlowdown">
/// One-off exposure is divided by this, unless the add's source has its own slowdown below.
/// </param>
public sealed record ExposureSettings(double TimeSlowdown, bool IsRunningExposed, double LootSlowdown)
{
    /// <summary>Searching and looking over furniture. Null follows <see cref="LootSlowdown"/>.</summary>
    public double? SearchSlowdown { get; init; }

    /// <summary>Picking up and taking out supplies. Null follows <see cref="LootSlowdown"/>.</summary>
    public double? PickupSlowdown { get; init; }

    /// <summary>Breaking doors and locks open, unlocking and card swipes. Null follows <see cref="LootSlowdown"/>.</summary>
    public double? BreakInSlowdown { get; init; }

    /// <summary>Events, stories and dialogue rewards. Null follows <see cref="LootSlowdown"/>.</summary>
    public double? EventSlowdown { get; init; }

    /// <summary>Whether a slowdown below 1, which makes exposure rise faster than the game's, is allowed.</summary>
    public bool IsFasterThanGameAllowed { get; init; }

    /// <summary>How much harsher an expedition is by day and by location.</summary>
    public DangerRules Danger { get; init; } = DangerRules.None;

    public double Floor => IsFasterThanGameAllowed ? ExposureRules.FasterFloor : ExposureRules.MinSlowdown;

    public double OneOffSlowdown(ExposureSource source)
    {
        var own = source switch
        {
            ExposureSource.Search => SearchSlowdown,
            ExposureSource.Loot => PickupSlowdown,
            ExposureSource.Door => BreakInSlowdown,
            ExposureSource.Event => EventSlowdown,
            _ => null,
        };

        return own ?? LootSlowdown;
    }

    public float TimeRate(float gameRate, double danger) =>
        ExposureRules.TimeRate(gameRate, ExposureRules.Effective(TimeSlowdown, danger, Floor), Floor);

    /// <summary>Running is never slowed by a setting; when it adds exposure, only danger scales it.</summary>
    public float RunRate(float gameRate, double danger) =>
        IsRunningExposed
            ? ExposureRules.TimeRate(gameRate, ExposureRules.Effective(ExposureRules.MinSlowdown, danger, Floor), Floor)
            : ExposureRules.RunRate(gameRate, isRunningExposed: false);

    public (int Value, double Carried) OneOff(ExposureSource source, int value, double carried, double danger) =>
        ExposureRules.OneOff(value, ExposureRules.Effective(OneOffSlowdown(source), danger, Floor), carried, Floor);
}

/// <summary>
/// On an expedition, exposure rises every frame by a time rate, plus a running rate while the character runs,
/// and in one-off amounts when a search, loot or event adds some. Only rises are slowed; a drop is the game's own.
/// A slowdown is never below 1, so nothing rises faster than in the game, unless the player allows it; then the
/// floor is <see cref="FasterFloor"/>.
/// </summary>
public static class ExposureRules
{
    public const double MinSlowdown = 1;
    public const double MaxSlowdown = 100;
    public const double FasterFloor = 0.25;

    public static double ClampSlowdown(double slowdown, double floor = MinSlowdown) =>
        double.IsNaN(slowdown) ? 1 : Math.Clamp(slowdown, floor, MaxSlowdown);

    /// <summary>A slowdown after danger: a danger of 2 halves it, so exposure rises twice as fast as the setting alone.</summary>
    public static double Effective(double slowdown, double danger, double floor = MinSlowdown) =>
        ClampSlowdown(ClampSlowdown(slowdown, floor) / DangerRules.ClampMultiplier(danger), floor);

    public static float TimeRate(float gameRate, double slowdown, double floor = MinSlowdown) =>
        gameRate > 0 ? (float)(gameRate / ClampSlowdown(slowdown, floor)) : gameRate;

    public static float RunRate(float gameRate, bool isRunningExposed) =>
        isRunningExposed || gameRate <= 0 ? gameRate : 0f;

    /// <summary>
    /// The whole part of a slowed one-off add, and the fraction carried to the next one: the game only adds whole
    /// numbers, so a 5 slowed by 2 gives 2 now and 3 next time instead of 2 every time. A drop keeps its full effect.
    /// </summary>
    public static (int Value, double Carried) OneOff(int value, double slowdown, double carried, double floor = MinSlowdown)
    {
        if (value <= 0) return (value, carried);

        var share = value / ClampSlowdown(slowdown, floor) + carried;
        var whole = Math.Floor(share);

        return ((int)whole, share - whole);
    }
}
