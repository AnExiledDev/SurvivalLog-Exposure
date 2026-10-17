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

    /// <summary>
    /// The whole part of a slowed one-off add, and the fraction carried to the next one: the game only adds whole
    /// numbers, so a 5 slowed by 2 gives 2 now and 3 next time instead of 2 every time. A drop keeps its full effect.
    /// </summary>
    public static (int Value, double Carried) OneOff(int value, double slowdown, double carried)
    {
        if (value <= 0) return (value, carried);

        var share = value / ClampSlowdown(slowdown) + carried;
        var whole = Math.Floor(share);

        return ((int)whole, share - whole);
    }
}
