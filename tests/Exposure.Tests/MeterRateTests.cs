using Exposure.Core;
using Xunit;

namespace Exposure.Tests;

public class MeterRateTests
{
    private const double Frame = 0.02;

    /// <summary>Feeds updates of one frame each, rising by the given points per second, from the given exposure.</summary>
    private static double Rise(MeterRate rate, double from, double pointsPerSecond, double seconds)
    {
        var exposure = from;
        var frames = (int)Math.Round(seconds / Frame);

        for (var i = 0; i < frames; i++)
        {
            exposure += pointsPerSecond * Frame;
            rate.Add(Frame, exposure);
        }

        return exposure;
    }

    private static MeterRate Started(double exposure)
    {
        var rate = new MeterRate();

        rate.Add(0, exposure);

        return rate;
    }

    [Fact]
    public void A_steady_rise_is_measured_in_points_per_real_second()
    {
        var rate = Started(0);

        Rise(rate, 0, 2, 3);

        Assert.Equal(2, rate.PointsPerSecond!.Value, 6);
    }

    [Fact]
    public void There_is_no_rate_before_a_second_has_been_read()
    {
        var rate = Started(0);

        Rise(rate, 0, 2, 0.5);

        Assert.Null(rate.PointsPerSecond);
    }

    [Fact]
    public void A_search_add_does_not_count_as_rise_so_the_time_to_full_assumes_the_same_pace()
    {
        var rate = Started(0);
        var exposure = Rise(rate, 0, 2, 2);

        exposure += 25;
        rate.Add(Frame, exposure);
        Rise(rate, exposure, 2, 2);

        Assert.Equal(2 * 4 / (4 + Frame), rate.PointsPerSecond!.Value, 6);
    }

    [Fact]
    public void An_add_with_no_time_passed_is_a_one_off_add()
    {
        var rate = Started(0);
        var exposure = Rise(rate, 0, 2, 2);

        rate.Add(0, exposure + 1);
        Rise(rate, exposure + 1, 2, 0.5);

        Assert.Equal(2, rate.PointsPerSecond!.Value, 6);
    }

    [Fact]
    public void A_paused_game_reads_as_not_rising_once_a_second_passes_with_no_rise()
    {
        var rate = Started(0);
        var exposure = Rise(rate, 0, 2, 3);

        Rise(rate, exposure, 0, 1.2);

        Assert.Null(rate.PointsPerSecond);
    }

    [Fact]
    public void A_short_hitch_with_no_rise_still_has_a_rate()
    {
        var rate = Started(0);
        var exposure = Rise(rate, 0, 2, 3);

        Rise(rate, exposure, 0, 0.5);

        Assert.NotNull(rate.PointsPerSecond);
    }

    [Fact]
    public void A_falling_bar_reads_as_not_rising()
    {
        var rate = Started(100);

        Rise(rate, 100, -5, 3);

        Assert.Null(rate.PointsPerSecond);
    }

    [Fact]
    public void Only_the_last_few_seconds_count_so_a_change_of_pace_shows_soon()
    {
        var rate = Started(0);
        var exposure = Rise(rate, 0, 10, 20);

        Rise(rate, exposure, 2, MeterRate.WindowSeconds + 1);

        Assert.Equal(2, rate.PointsPerSecond!.Value, 6);
    }

    [Fact]
    public void After_a_reset_nothing_read_before_counts()
    {
        var rate = Started(0);

        Rise(rate, 0, 2, 3);
        rate.Reset();
        rate.Add(Frame, 500);
        Rise(rate, 500, 4, 2);

        Assert.Equal(4, rate.PointsPerSecond!.Value, 6);
    }
}
