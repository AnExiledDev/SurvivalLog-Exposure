using System.Collections.Generic;
using Exposure.Core;
using Xunit;

namespace Exposure.Tests;

public class DangerRulesTests
{
    private static readonly Dictionary<int, double> NoLocations = new();

    [Fact]
    public void The_default_danger_changes_nothing_on_any_day_or_location()
    {
        Assert.Equal(1, DangerRules.None.Multiplier(1, 1000));
        Assert.Equal(1, DangerRules.None.Multiplier(90, 1005));
        Assert.Equal(1, new DangerRules(1, 1, 30, NoLocations).Multiplier(45, 1002));
    }

    [Fact]
    public void Early_trips_use_the_start_and_late_ones_the_end()
    {
        var danger = new DangerRules(0.5, 2, 11, NoLocations);

        Assert.Equal(0.5, danger.Multiplier(1, 1000));
        Assert.Equal(1.25, danger.Multiplier(6, 1000), 6);
        Assert.Equal(2, danger.Multiplier(11, 1000));
        Assert.Equal(2, danger.Multiplier(40, 1000));
    }

    [Fact]
    public void A_ramp_of_one_day_uses_the_end_from_the_first_day()
    {
        Assert.Equal(3, new DangerRules(1, 3, 1, NoLocations).Multiplier(1, 1000));
    }

    [Fact]
    public void An_unknown_day_is_neither_early_nor_late()
    {
        Assert.Equal(1, new DangerRules(0.5, 2, 10, NoLocations).Multiplier(null, 1000));
    }

    [Fact]
    public void A_location_multiplier_applies_on_top_of_the_day_and_only_there()
    {
        var danger = new DangerRules(2, 2, 1, new Dictionary<int, double> { [1005] = 1.5 });

        Assert.Equal(3, danger.Multiplier(5, 1005));
        Assert.Equal(2, danger.Multiplier(5, 1002));
    }

    [Fact]
    public void Danger_stays_between_a_tenth_and_ten()
    {
        Assert.Equal(10, new DangerRules(5, 5, 1, new Dictionary<int, double> { [1005] = 5 }).Multiplier(1, 1005));
        Assert.Equal(0.1, new DangerRules(0, 0, 1, NoLocations).Multiplier(1, 1000));
        Assert.Equal(1, new DangerRules(double.NaN, double.NaN, 1, NoLocations).Multiplier(1, 1000));
    }

    [Fact]
    public void A_location_list_reads_ids_and_multipliers()
    {
        var list = LocationList.Parse(" 1002=1.5, 1005 : 2;1000=0.5 ");

        Assert.Equal(new Dictionary<int, double> { [1002] = 1.5, [1005] = 2, [1000] = 0.5 }, list.Multipliers);
        Assert.Empty(list.Ignored);
    }

    [Fact]
    public void Entries_that_cannot_be_read_are_reported_and_the_rest_still_apply()
    {
        var list = LocationList.Parse("school=2, 1002, 1003=fast, 1004=NaN, 1005=2");

        Assert.Equal(new Dictionary<int, double> { [1005] = 2 }, list.Multipliers);
        Assert.Equal(new[] { "school=2", "1002", "1003=fast", "1004=NaN" }, list.Ignored);
    }

    [Fact]
    public void An_empty_list_and_stray_commas_are_not_mistakes()
    {
        Assert.Empty(LocationList.Parse("").Multipliers);
        Assert.Empty(LocationList.Parse(null).Ignored);
        Assert.Empty(LocationList.Parse(" , ;").Ignored);
    }

    [Fact]
    public void A_later_entry_for_the_same_location_replaces_the_earlier_one()
    {
        Assert.Equal(2, LocationList.Parse("1005=1.5, 1005=2").Multipliers[1005]);
    }

    [Fact]
    public void A_location_multiplier_out_of_range_is_kept_in_range()
    {
        Assert.Equal(10, LocationList.Parse("1005=50").Multipliers[1005]);
    }
}
