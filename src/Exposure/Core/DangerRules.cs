using System;
using System.Collections.Generic;
using System.Globalization;

namespace Exposure.Core;

/// <summary>
/// How much harsher an expedition is than the settings alone: a multiplier of 2 makes exposure rise twice as fast.
/// It ramps from <see cref="StartMultiplier"/> on day 1 to <see cref="EndMultiplier"/> on day <see cref="RampDays"/>
/// and stays there, and a location's own multiplier is applied on top.
/// </summary>
/// <param name="Locations">A multiplier per expedition location (the game's map id).</param>
public sealed record DangerRules(double StartMultiplier, double EndMultiplier, int RampDays, IReadOnlyDictionary<int, double> Locations)
{
    public const double MinMultiplier = 0.1;
    public const double MaxMultiplier = 10;

    public static readonly DangerRules None = new(1, 1, 1, new Dictionary<int, double>());

    public static double ClampMultiplier(double multiplier) =>
        double.IsNaN(multiplier) ? 1 : Math.Clamp(multiplier, MinMultiplier, MaxMultiplier);

    /// <param name="day">The in-game day, from 1; null when the game's clock could not be read.</param>
    public double Multiplier(int? day, int mapId)
    {
        var location = Locations.TryGetValue(mapId, out var own) ? ClampMultiplier(own) : 1;

        return ClampMultiplier(DayMultiplier(day) * location);
    }

    /// <summary>An unknown day counts as neither early nor late.</summary>
    public double DayMultiplier(int? day)
    {
        if (day is not { } known) return 1;

        var start = ClampMultiplier(StartMultiplier);
        var end = ClampMultiplier(EndMultiplier);
        var progress = RampDays <= 1 ? 1 : Math.Clamp((known - 1) / (double)(RampDays - 1), 0, 1);

        return start + (end - start) * progress;
    }
}

/// <summary>Location multipliers read from a setting such as <c>1002=1.5, 1005=2</c>.</summary>
/// <param name="Ignored">Entries that could not be read, as written.</param>
public sealed record LocationList(IReadOnlyDictionary<int, double> Multipliers, IReadOnlyList<string> Ignored)
{
    private static readonly char[] EntrySeparators = { ',', ';' };
    private static readonly char[] ValueSeparators = { '=', ':' };

    /// <summary>A later entry for the same location replaces an earlier one. Multipliers are kept in range.</summary>
    public static LocationList Parse(string? text)
    {
        var multipliers = new Dictionary<int, double>();
        var ignored = new List<string>();

        foreach (var raw in (text ?? "").Split(EntrySeparators))
        {
            var entry = raw.Trim();

            if (entry.Length == 0) continue;

            if (TryParseEntry(entry, out var mapId, out var multiplier)) multipliers[mapId] = DangerRules.ClampMultiplier(multiplier);
            else ignored.Add(entry);
        }

        return new LocationList(multipliers, ignored);
    }

    private static bool TryParseEntry(string entry, out int mapId, out double multiplier)
    {
        var parts = entry.Split(ValueSeparators);

        mapId = 0;
        multiplier = 0;

        return parts.Length == 2
               && int.TryParse(parts[0].Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out mapId)
               && double.TryParse(parts[1].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out multiplier)
               && double.IsFinite(multiplier);
    }
}
