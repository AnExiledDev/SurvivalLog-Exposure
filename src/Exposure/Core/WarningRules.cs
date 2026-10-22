using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Exposure.Core;

/// <param name="Thresholds">The exposure levels that warn, as percents of the bar, lowest first.</param>
/// <param name="PauseAt">The exposure level that pauses the game, as a percent of the bar; 0 never pauses.</param>
public sealed record WarningLevels(IReadOnlyList<int> Thresholds, int PauseAt);

/// <param name="Levels">Where warnings and the pause fall.</param>
/// <param name="IsToastOn">Whether a warning shows a notice.</param>
/// <param name="IsSoundOn">Whether a warning plays the game's exposure alarm.</param>
public sealed record WarningSettings(WarningLevels Levels, bool IsToastOn, bool IsSoundOn);

/// <summary>
/// The highest exposure seen on this trip, as a percent of the bar. It is empty until the trip's first reading, and
/// that reading is where the trip starts from rather than a rise, so a trip loaded part way through does not warn
/// about levels it had already passed.
/// </summary>
public readonly record struct WarningWatch(double? HighestPercent)
{
    public static readonly WarningWatch Start = new((double?)null);
}

/// <param name="Watch">The watch to keep for the next reading.</param>
/// <param name="ReachedAt">The highest warning or pause level this reading reached for the first time on the trip, if any.</param>
/// <param name="IsPauseDue">Whether this reading reached the pause level for the first time on the trip.</param>
public readonly record struct WarningStep(WarningWatch Watch, int? ReachedAt, bool IsPauseDue);

/// <summary>Why a due pause was or was not made.</summary>
public enum PauseOutcome
{
    NotDue,
    Pauses,
    AlreadyPaused,
    TripEnding,
    NeedsTimeControl,
    NeedsTimeControlOnExpeditions,
}

/// <summary>
/// Each warning level fires once per trip, the first time exposure rises to it. A level is reached when the bar goes
/// from below it to at or above it, so a drop and a second rise do not warn again, and one big rise past several
/// levels warns once, for the highest.
/// </summary>
public static class WarningRules
{
    public const int MaxPercent = 100;

    private static readonly char[] Separators = { ',', ';', ' ', (char)0xFF0C, (char)0x3001 };

    /// <summary>Whole percents from 1 to 100 in any order, split by commas, semicolons or spaces. Anything else is skipped.</summary>
    public static IReadOnlyList<int> ParseThresholds(string? text)
    {
        if (string.IsNullOrWhiteSpace(text)) return Array.Empty<int>();

        return text.Split(Separators, StringSplitOptions.RemoveEmptyEntries)
            .Select(ParsePercent)
            .Where(percent => percent is >= 1 and <= MaxPercent)
            .Distinct()
            .OrderBy(percent => percent)
            .ToArray();
    }

    /// <summary>A pause level below 1 is off (0); one above the bar is the full bar.</summary>
    public static int ClampPauseAt(int percent) => Math.Clamp(percent, 0, MaxPercent);

    /// <summary>The bar's fill as a percent; a bar with no room reads as empty.</summary>
    public static double Percent(double exposure, double maxExposure) =>
        maxExposure > 0 ? exposure / maxExposure * MaxPercent : 0;

    public static WarningStep Next(WarningWatch watch, double percent, WarningLevels levels)
    {
        if (watch.HighestPercent is not { } highest) return new WarningStep(new WarningWatch(percent), null, false);

        if (percent <= highest) return new WarningStep(watch, null, false);

        bool IsReached(int level) => level > highest && level <= percent;

        var isPauseDue = levels.PauseAt > 0 && IsReached(levels.PauseAt);
        var reached = levels.Thresholds.Where(IsReached).Concat(isPauseDue ? new[] { levels.PauseAt } : Array.Empty<int>()).ToArray();
        int? reachedAt = reached.Length > 0 ? reached.Max() : null;

        return new WarningStep(new WarningWatch(percent), reachedAt, isPauseDue);
    }

    /// <summary>
    /// The game has no pause on an expedition: it ignores its speed keys there and shows no speed buttons, so only
    /// Time Control, with its expedition controls on, lets the player resume. A pause while the trip is ending would
    /// hold up the trip home, and a pause already standing is left as it is.
    /// </summary>
    public static PauseOutcome DecidePause(bool isDue, bool hasTimeControl, bool isTimeControlOnExpeditions, bool isPaused, bool isTripEnding)
    {
        if (!isDue) return PauseOutcome.NotDue;

        if (isTripEnding) return PauseOutcome.TripEnding;

        if (!hasTimeControl) return PauseOutcome.NeedsTimeControl;

        if (!isTimeControlOnExpeditions) return PauseOutcome.NeedsTimeControlOnExpeditions;

        return isPaused ? PauseOutcome.AlreadyPaused : PauseOutcome.Pauses;
    }

    private static int ParsePercent(string part) =>
        int.TryParse(part.Trim().TrimEnd('%'), NumberStyles.Integer, CultureInfo.InvariantCulture, out var percent) ? percent : 0;
}
