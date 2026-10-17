using Exposure.Core;
using Xunit;

namespace Exposure.Tests;

public class ExposureRulesTests
{
    [Fact]
    public void Time_spent_out_raises_exposure_half_as_fast_at_a_slowdown_of_two()
    {
        Assert.Equal(0.5f, ExposureRules.TimeRate(1f, 2));
    }

    [Fact]
    public void A_time_rate_that_lowers_exposure_is_left_alone()
    {
        Assert.Equal(-1f, ExposureRules.TimeRate(-1f, 2));
    }

    [Fact]
    public void Running_adds_nothing_when_turned_off()
    {
        Assert.Equal(0f, ExposureRules.RunRate(3f, isRunningExposed: false));
    }

    [Fact]
    public void Running_keeps_the_game_rate_when_turned_on()
    {
        Assert.Equal(3f, ExposureRules.RunRate(3f, isRunningExposed: true));
    }

    [Fact]
    public void Looting_adds_half_as_much_at_a_slowdown_of_two()
    {
        Assert.Equal(15f, ExposureRules.AfterOneOff(10f, 20f, 2));
    }

    [Fact]
    public void Something_that_lowers_exposure_keeps_its_full_effect()
    {
        Assert.Equal(5f, ExposureRules.AfterOneOff(10f, 5f, 2));
    }

    [Fact]
    public void A_slowdown_below_one_never_speeds_exposure_up()
    {
        Assert.Equal(1f, ExposureRules.TimeRate(1f, 0.1));
        Assert.Equal(20f, ExposureRules.AfterOneOff(10f, 20f, double.NaN));
    }
}
