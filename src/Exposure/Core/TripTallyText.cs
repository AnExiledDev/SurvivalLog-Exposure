using System;
using System.Collections.Generic;
using System.Globalization;

namespace Exposure.Core;

/// <summary>
/// The trip summary, in English or Simplified Chinese, and its log line. The notice shows at most six lines, so it
/// puts what came home first and folds searching, looting and doors into one line; the log keeps every source.
/// </summary>
public static class TripTallyText
{
    /// <summary>Exposure below this shows as 0, so it gets no line.</summary>
    private const double SmallestShown = 0.05;

    /// <summary>The notice keeps line breaks in its body.</summary>
    private const char LineBreak = (char)10;

    public static string Title(bool isChinese) => isChinese ? "探索总结" : "Trip summary";

    /// <param name="loot">What came home, or null when it could not be read; then the loot line is left out.</param>
    public static string Body(TripTally tally, TripLoot? loot, bool isChinese)
    {
        var exposureLines = ExposureLines(tally, isChinese);
        var lines = new List<string>();

        if (loot != null) lines.Add(LootLine(loot, isChinese));

        if (exposureLines.Count > 0) lines.AddRange(exposureLines);
        else lines.Add(isChinese ? "这次探索没有增加暴露值。" : "No exposure added on this trip.");

        return string.Join(LineBreak, lines);
    }

    public static string LootLine(TripLoot loot, bool isChinese)
    {
        if (loot.Count <= 0) return isChinese ? "带回家：没有新物品" : "Brought home: nothing new";

        var worth = loot.Value > 0 ? (isChinese ? $"，价值 {Whole(loot.Value)}$" : $", worth {Whole(loot.Value)}$") : "";

        if (isChinese) return $"带回家：{Whole(loot.Count)} 件物品（{Whole(loot.Kinds)} 种）{worth}";

        return $"Brought home: {Whole(loot.Count)} {Plural(loot.Count, "item", "items")} ({Whole(loot.Kinds)} {Plural(loot.Kinds, "kind", "kinds")}){worth}";
    }

    public static string LogLine(TripTally tally, TripLoot? loot)
    {
        var sources = new List<string>();

        foreach (ExposureSource source in Enum.GetValues(typeof(ExposureSource)))
        {
            sources.Add($"{source.ToString().ToLowerInvariant()} {Amount(tally.Of(source))}");
        }

        var lootPart = loot == null
            ? "loot brought home could not be read"
            : $"brought home {Whole(loot.Count)} items ({Whole(loot.Kinds)} kinds) worth {Whole(loot.Value)}$";

        return $"trip summary: exposure {string.Join(", ", sources)}, total {Amount(tally.Total())}; {lootPart}";
    }

    /// <summary>Whole points from 10 up, one decimal below, so a small source does not read as 0.</summary>
    public static string Amount(double amount) =>
        amount.ToString(amount >= 10 ? "0" : "0.#", CultureInfo.InvariantCulture);

    private static List<string> ExposureLines(TripTally tally, bool isChinese)
    {
        var separator = isChinese ? "：" : ": ";
        var shares = new (double Amount, string Label)[]
        {
            (tally.Of(ExposureSource.Time), isChinese ? "时间" : "Time"),
            (tally.Of(ExposureSource.Running), isChinese ? "跑步" : "Running"),
            (tally.Of(ExposureSource.Search) + tally.Of(ExposureSource.Loot) + tally.Of(ExposureSource.Door),
                isChinese ? "翻找、拾取和开门" : "Searching, looting and doors"),
            (tally.Of(ExposureSource.Event), isChinese ? "事件" : "Events"),
            (tally.Of(ExposureSource.Other), isChinese ? "其他" : "Other"),
        };
        var lines = new List<string>();

        foreach (var (amount, label) in shares)
        {
            if (amount >= SmallestShown) lines.Add(label + separator + Amount(amount));
        }

        return lines;
    }

    private static string Whole(long value) => value.ToString(CultureInfo.InvariantCulture);

    private static string Plural(long count, string one, string many) => count == 1 ? one : many;
}
