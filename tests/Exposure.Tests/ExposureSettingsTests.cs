using Exposure.Core;
using Xunit;

namespace Exposure.Tests;

public class ExposureSettingsTests
{
    private static readonly ExposureSettings Halved = new(2, IsRunningExposed: false, 2);

    [Fact]
    public void A_source_without_its_own_slowdown_follows_the_loot_slowdown()
    {
        var settings = Halved with { SearchSlowdown = 4 };

        Assert.Equal((5, 0.0), settings.OneOff(ExposureSource.Loot, 10, 0, danger: 1));
        Assert.Equal((5, 0.0), settings.OneOff(ExposureSource.Door, 10, 0, danger: 1));
        Assert.Equal((5, 0.0), settings.OneOff(ExposureSource.Other, 10, 0, danger: 1));
    }

    [Fact]
    public void Each_one_off_source_uses_its_own_slowdown()
    {
        var settings = Halved with { SearchSlowdown = 4, PickupSlowdown = 5, BreakInSlowdown = 10, EventSlowdown = 20 };

        Assert.Equal(25, settings.OneOff(ExposureSource.Search, 100, 0, danger: 1).Value);
        Assert.Equal(20, settings.OneOff(ExposureSource.Loot, 100, 0, danger: 1).Value);
        Assert.Equal(10, settings.OneOff(ExposureSource.Door, 100, 0, danger: 1).Value);
        Assert.Equal(5, settings.OneOff(ExposureSource.Event, 100, 0, danger: 1).Value);
    }

    [Fact]
    public void Without_the_opt_in_nothing_rises_faster_than_the_game()
    {
        var settings = new ExposureSettings(0.5, IsRunningExposed: true, 0.25) { SearchSlowdown = 0.25 };

        Assert.Equal(2f, settings.TimeRate(2f, danger: 1));
        Assert.Equal(100, settings.OneOff(ExposureSource.Search, 100, 0, danger: 1).Value);
        Assert.Equal(100, settings.OneOff(ExposureSource.Loot, 100, 0, danger: 1).Value);
    }

    [Fact]
    public void Danger_never_pushes_exposure_past_the_game_without_the_opt_in()
    {
        Assert.Equal(2f, Halved.TimeRate(2f, danger: 10));
        Assert.Equal(100, Halved.OneOff(ExposureSource.Loot, 100, 0, danger: 10).Value);
        Assert.Equal(10f, (Halved with { IsRunningExposed = true }).RunRate(10f, danger: 10));
    }

    [Fact]
    public void A_slowdown_after_danger_never_drops_below_the_floor()
    {
        Assert.Equal(1, ExposureRules.Effective(2, danger: 10));
        Assert.Equal(ExposureRules.FasterFloor, ExposureRules.Effective(0.5, danger: 10, ExposureRules.FasterFloor));
    }

    [Fact]
    public void With_the_opt_in_a_slowdown_below_one_makes_exposure_rise_faster()
    {
        var settings = new ExposureSettings(0.5, IsRunningExposed: true, 0.5) { IsFasterThanGameAllowed = true };

        Assert.Equal(4f, settings.TimeRate(2f, danger: 1));
        Assert.Equal(200, settings.OneOff(ExposureSource.Loot, 100, 0, danger: 1).Value);
    }

    [Fact]
    public void Faster_than_the_game_stops_at_four_times_as_fast()
    {
        var settings = new ExposureSettings(0.1, IsRunningExposed: true, 0.1) { IsFasterThanGameAllowed = true };

        Assert.Equal(8f, settings.TimeRate(2f, danger: 1));
        Assert.Equal(400, settings.OneOff(ExposureSource.Loot, 100, 0, danger: 1).Value);
        Assert.Equal(40f, settings.RunRate(10f, danger: 10));
    }

    [Fact]
    public void Danger_divides_the_slowdown()
    {
        Assert.Equal(2f, Halved.TimeRate(2f, danger: 2));
        Assert.Equal(0.5f, Halved.TimeRate(2f, danger: 0.5));
        Assert.Equal(25, Halved.OneOff(ExposureSource.Loot, 100, 0, danger: 0.5).Value);
    }

    [Fact]
    public void Running_turned_off_adds_nothing_whatever_the_danger()
    {
        Assert.Equal(0f, Halved.RunRate(10f, danger: 10));
    }

    [Fact]
    public void Running_turned_on_keeps_the_game_rate_until_danger_is_forgiving()
    {
        var settings = Halved with { IsRunningExposed = true };

        Assert.Equal(10f, settings.RunRate(10f, danger: 1));
        Assert.Equal(5f, settings.RunRate(10f, danger: 0.5));
    }
}
