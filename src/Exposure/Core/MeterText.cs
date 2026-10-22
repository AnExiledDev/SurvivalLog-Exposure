using System;

namespace Exposure.Core;

/// <summary>The words the meter shows, in English or in the game's Chinese.</summary>
public static class MeterText
{
    /// <summary>Under this many seconds left, the time is given in seconds.</summary>
    public const double SecondsUpTo = 55;

    /// <summary>From this many minutes left, the time is given in hours.</summary>
    public const double MinutesUpTo = 100;

    /// <summary>The bar's points as whole numbers, "62 / 100".</summary>
    public static string Numbers(double current, int max) => $"{(int)Math.Floor(Math.Max(0, current))} / {max}";

    /// <summary>
    /// How long until the bar is full at the given rise, in real time, or null when it is not rising or already full.
    /// Seconds are rounded up to fives, so the line never promises more time than is left.
    /// </summary>
    public static string? TimeToFull(double current, int max, double? pointsPerSecond, bool isChinese)
    {
        if (pointsPerSecond is not > 0 || current >= max) return null;

        var seconds = (max - current) / pointsPerSecond.Value;

        if (seconds < SecondsUpTo)
        {
            var fives = Math.Max(5, (int)Math.Ceiling(seconds / 5) * 5);

            return isChinese ? $"约{fives}秒后涨满" : $"full in about {fives} s";
        }

        var minutes = seconds / 60;

        if (minutes < MinutesUpTo)
        {
            var whole = Math.Max(1, (int)Math.Round(minutes));

            return isChinese ? $"约{whole}分钟后涨满" : $"full in about {whole} min";
        }

        var hours = Math.Max(2, (int)Math.Round(minutes / 60));

        return isChinese ? $"约{hours}小时后涨满" : $"full in about {hours} h";
    }
}
