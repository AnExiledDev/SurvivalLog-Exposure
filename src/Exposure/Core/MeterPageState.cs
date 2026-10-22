using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Exposure.Core;

/// <summary>Which of the meter's additions are shown.</summary>
public sealed record MeterSettings(bool ShowNumbers, bool ShowTimeToFull, bool ShowCosts);

/// <summary>The bar as last read: its points, its full mark, and its steady rise in points per real second.</summary>
public readonly record struct MeterReading(double Current, int Max, double? PointsPerSecond);

/// <summary>
/// What page.js is sent: the numbers on the bar, the time-to-full line under it, and each furniture button's cost by
/// its function id. A part that is turned off or unknown is null, and the page then leaves the game's own drawing alone.
/// Text is escaped to ASCII so the script stays ASCII.
/// </summary>
public static class MeterPageState
{
    /// <summary>Off an expedition: nothing to add to any page.</summary>
    public static string Empty { get; } = Json(null, null, null);

    public static string Json(MeterSettings settings, MeterReading? reading, IReadOnlyDictionary<int, int>? costs, bool isChinese)
    {
        var numbers = settings.ShowNumbers && reading is { } shown ? MeterText.Numbers(shown.Current, shown.Max) : null;
        var full = settings.ShowTimeToFull && reading is { } rising ? MeterText.TimeToFull(rising.Current, rising.Max, rising.PointsPerSecond, isChinese) : null;

        return Json(numbers, full, settings.ShowCosts ? costs : null);
    }

    private static string Json(string? numbers, string? full, IReadOnlyDictionary<int, int>? costs)
    {
        var json = new StringBuilder("{\"numbers\":");

        AppendString(json, numbers);
        json.Append(",\"full\":");
        AppendString(json, full);
        json.Append(",\"costs\":");
        AppendCosts(json, costs);
        json.Append('}');

        return json.ToString();
    }

    private static void AppendString(StringBuilder json, string? text)
    {
        if (text is null)
        {
            json.Append("null");

            return;
        }

        json.Append('"');

        foreach (var c in text)
        {
            if (c == '"' || c == '\\') json.Append('\\').Append(c);
            else if (c < ' ' || c > '~') json.Append("\\u").Append(((int)c).ToString("x4"));
            else json.Append(c);
        }

        json.Append('"');
    }

    // Sorted by id, so the same costs always make the same text and an unchanged state is not sent again.
    private static void AppendCosts(StringBuilder json, IReadOnlyDictionary<int, int>? costs)
    {
        if (costs is null)
        {
            json.Append("null");

            return;
        }

        json.Append('{');
        json.Append(string.Join(",", costs.OrderBy(cost => cost.Key).Select(cost => "\"" + cost.Key.ToString(CultureInfo.InvariantCulture) + "\":" + cost.Value.ToString(CultureInfo.InvariantCulture))));
        json.Append('}');
    }
}
