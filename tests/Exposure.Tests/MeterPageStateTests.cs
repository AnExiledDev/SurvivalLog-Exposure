using Exposure.Core;
using Xunit;

namespace Exposure.Tests;

public class MeterPageStateTests
{
    private static readonly MeterSettings AllOn = new(true, true, true);
    private static readonly MeterReading Rising = new(62, 100, 38.0 / 250);
    private static readonly Dictionary<int, int> Costs = new() { [1503] = 25, [1101] = 85 };

    [Fact]
    public void The_page_gets_the_numbers_the_time_to_full_and_each_button_cost()
    {
        Assert.Equal("{\"numbers\":\"62 / 100\",\"full\":\"full in about 4 min\",\"costs\":{\"1101\":85,\"1503\":25}}",
            MeterPageState.Json(AllOn, Rising, Costs, isChinese: false));
    }

    [Fact]
    public void Chinese_text_is_sent_as_ascii_escapes()
    {
        var json = MeterPageState.Json(AllOn, Rising, null, isChinese: true);

        Assert.Contains("\"full\":\"\\u7ea64\\u5206\\u949f\\u540e\\u6da8\\u6ee1\"", json);
        Assert.All(json, c => Assert.InRange(c, ' ', '~'));
    }

    [Fact]
    public void A_bar_that_is_not_rising_sends_no_time_to_full()
    {
        var json = MeterPageState.Json(AllOn, Rising with { PointsPerSecond = null }, null, isChinese: false);

        Assert.Contains("\"full\":null", json);
        Assert.Contains("\"numbers\":\"62 / 100\"", json);
    }

    [Fact]
    public void Each_part_turned_off_is_left_to_the_game()
    {
        Assert.Equal(MeterPageState.Empty, MeterPageState.Json(new MeterSettings(false, false, false), Rising, Costs, isChinese: false));
    }

    [Fact]
    public void Costs_can_show_before_the_bar_has_been_read()
    {
        Assert.Equal("{\"numbers\":null,\"full\":null,\"costs\":{\"1101\":85,\"1503\":25}}",
            MeterPageState.Json(AllOn, null, Costs, isChinese: false));
    }

    [Fact]
    public void Off_an_expedition_nothing_is_added()
    {
        Assert.Equal("{\"numbers\":null,\"full\":null,\"costs\":null}", MeterPageState.Empty);
    }
}
