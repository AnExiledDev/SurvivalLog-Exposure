using System;

namespace Exposure.Core;

/// <param name="TimeSlowdown">Exposure from time spent out rises this many times slower.</param>
/// <param name="IsRunningExposed">Whether running adds exposure on top of time.</param>
/// <param name="LootSlowdown">One-off exposure from searching, looting and events is divided by this.</param>
public sealed record ExposureSettings(double TimeSlowdown, bool IsRunningExposed, double LootSlowdown);

/// <summary>
/// On an expedition, exposure rises every frame by a time rate, plus a running rate while the character runs,
/// and in one-off amounts when a search, loot or event adds some. Only rises are slowed; a drop is the game's own.
/// </summary>
public static class ExposureRules
{
    public const double MinSlowdown = 1;
    public const double MaxSlowdown = 100;

    public static double ClampSlowdown(double slowdown) =>
        double.IsNaN(slowdown) ? 1 : Math.Clamp(slowdown, MinSlowdown, MaxSlowdown);

    public static float TimeRate(float gameRate, double slowdown) =>
        gameRate > 0 ? (float)(gameRate / ClampSlowdown(slowdown)) : gameRate;

    public static float RunRate(float gameRate, bool isRunningExposed) =>
        isRunningExposed || gameRate <= 0 ? gameRate : 0f;

    /// <summary>What a one-off add should have left, given exposure before and after the game applied it.</summary>
    public static float AfterOneOff(float before, float after, double slowdown) =>
        after <= before ? after : before + (float)((after - before) / ClampSlowdown(slowdown));
}
