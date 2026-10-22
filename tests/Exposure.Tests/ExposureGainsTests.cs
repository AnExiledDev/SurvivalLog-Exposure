using Exposure.Core;
using Xunit;

namespace Exposure.Tests;

public class ExposureGainsTests
{
    [Fact]
    public void A_frame_while_walking_is_all_time()
    {
        Assert.Equal((3.0, 0.0), ExposureGains.SplitFrame(3, 1f, 5f, isRunning: false));
    }

    [Fact]
    public void A_frame_while_running_is_shared_by_the_two_rates()
    {
        var (time, running) = ExposureGains.SplitFrame(6, 1f, 5f, isRunning: true);

        Assert.Equal(1, time, 6);
        Assert.Equal(5, running, 6);
    }

    [Fact]
    public void Running_that_adds_no_exposure_takes_no_share()
    {
        Assert.Equal((2.0, 0.0), ExposureGains.SplitFrame(2, 1f, 0f, isRunning: true));
    }

    [Fact]
    public void A_rise_with_no_rate_behind_it_is_put_on_time()
    {
        Assert.Equal((4.0, 0.0), ExposureGains.SplitFrame(4, 0f, 0f, isRunning: true));
    }

    [Fact]
    public void A_frame_that_lowers_or_keeps_the_bar_tells_nothing()
    {
        Assert.Equal((0.0, 0.0), ExposureGains.SplitFrame(-3, 1f, 5f, isRunning: true));
        Assert.Equal((0.0, 0.0), ExposureGains.SplitFrame(0, 1f, 5f, isRunning: true));
        Assert.Equal((0.0, 0.0), ExposureGains.SplitFrame(double.NaN, 1f, 5f, isRunning: true));
    }
}
