using Exposure.Core;
using Xunit;

namespace Exposure.Tests;

public class ExposureSourcesTests
{
    [Theory]
    [InlineData(1101, ExposureSource.Loot)]
    [InlineData(1102, ExposureSource.Door)]
    [InlineData(1502, ExposureSource.Door)]
    [InlineData(1104, ExposureSource.Search)]
    [InlineData(1505, ExposureSource.Search)]
    [InlineData(1603, ExposureSource.Search)]
    [InlineData(1932, ExposureSource.Search)]
    [InlineData(4242, ExposureSource.Other)]
    public void An_actions_effect_is_told_apart_by_its_effect_id(int effectId, ExposureSource expected)
    {
        Assert.Equal(expected, ExposureSources.ForEffect(effectId));
    }

    [Theory]
    [InlineData(9101, ExposureSource.Search)]
    [InlineData(9102, ExposureSource.Search)]
    [InlineData(9103, ExposureSource.Door)]
    [InlineData(9106, ExposureSource.Door)]
    [InlineData(9211, ExposureSource.Door)]
    [InlineData(9277, ExposureSource.Door)]
    public void A_search_or_break_in_reward_set_counts_as_such_whatever_runs_it(int rewardSetId, ExposureSource expected)
    {
        Assert.Equal(expected, ExposureSources.ForRewardSet(rewardSetId, null));
        Assert.Equal(expected, ExposureSources.ForRewardSet(rewardSetId, 1106));
    }

    [Fact]
    public void A_shared_reward_set_counts_as_the_furniture_action_that_ran_it()
    {
        Assert.Equal(ExposureSource.Loot, ExposureSources.ForRewardSet(9216, 1106));
        Assert.Equal(ExposureSource.Door, ExposureSources.ForRewardSet(9216, 1945));
        Assert.Equal(ExposureSource.Search, ExposureSources.ForRewardSet(9216, 1925));
        Assert.Equal(ExposureSource.Other, ExposureSources.ForRewardSet(9216, 4242));
    }

    [Fact]
    public void A_reward_set_outside_any_furniture_action_is_an_event()
    {
        Assert.Equal(ExposureSource.Event, ExposureSources.ForRewardSet(9216, null));
        Assert.Equal(ExposureSource.Event, ExposureSources.ForRewardSet(70001, null));
    }

    [Theory]
    [InlineData(1936, ExposureSource.Loot)]
    [InlineData(199, ExposureSource.Door)]
    [InlineData(1928, ExposureSource.Door)]
    [InlineData(1943, ExposureSource.Door)]
    [InlineData(1100, ExposureSource.Search)]
    [InlineData(1107, ExposureSource.Search)]
    [InlineData(1925, ExposureSource.Search)]
    [InlineData(4242, ExposureSource.Other)]
    public void A_furniture_action_is_told_apart_by_its_action_id(int actionId, ExposureSource expected)
    {
        Assert.Equal(expected, ExposureSources.ForAction(actionId));
    }
}
