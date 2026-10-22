using Exposure.Core;
using Xunit;

namespace Exposure.Tests;

public class MeterTextTests
{
    [Fact]
    public void The_numbers_show_whole_points_out_of_the_full_mark()
    {
        Assert.Equal("62 / 100", MeterText.Numbers(62.9, 100));
    }

    [Fact]
    public void Minutes_left_are_rounded_to_the_nearest_minute()
    {
        Assert.Equal("full in about 4 min", MeterText.TimeToFull(62, 100, 38.0 / 250, isChinese: false));
    }

    [Fact]
    public void Under_a_minute_left_is_given_in_seconds_rounded_up_to_fives()
    {
        Assert.Equal("full in about 45 s", MeterText.TimeToFull(59, 100, 1, isChinese: false));
    }

    [Fact]
    public void A_few_seconds_left_still_reads_as_five_seconds()
    {
        Assert.Equal("full in about 5 s", MeterText.TimeToFull(99.5, 100, 1, isChinese: false));
    }

    [Fact]
    public void Just_under_a_minute_left_rounds_to_one_minute_not_zero()
    {
        Assert.Equal("full in about 1 min", MeterText.TimeToFull(40, 100, 1, isChinese: false));
    }

    [Fact]
    public void Very_long_waits_are_given_in_hours()
    {
        Assert.Equal("full in about 3 h", MeterText.TimeToFull(0, 1800, 1.0 / 6, isChinese: false));
    }

    [Fact]
    public void Chinese_uses_the_game_word_for_filling_up()
    {
        Assert.Equal("约4分钟后涨满", MeterText.TimeToFull(62, 100, 38.0 / 250, isChinese: true));
    }

    [Fact]
    public void Chinese_seconds_are_given_as_seconds()
    {
        Assert.Equal("约45秒后涨满", MeterText.TimeToFull(59, 100, 1, isChinese: true));
    }

    [Fact]
    public void A_bar_that_is_not_rising_has_no_time_to_full()
    {
        Assert.Null(MeterText.TimeToFull(50, 100, null, isChinese: false));
    }

    [Fact]
    public void A_bar_with_no_rise_has_no_time_to_full()
    {
        Assert.Null(MeterText.TimeToFull(50, 100, 0, isChinese: false));
    }

    [Fact]
    public void A_full_bar_has_no_time_to_full()
    {
        Assert.Null(MeterText.TimeToFull(100, 100, 2, isChinese: false));
    }
}
