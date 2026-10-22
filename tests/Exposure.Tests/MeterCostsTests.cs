using Exposure.Core;
using Xunit;

namespace Exposure.Tests;

public class MeterCostsTests
{
    private const string Bar = "Action/Furniture_Bar_Default";
    private const int SearchEffect = 1104;
    private const int LootEffect = 1101;
    private const int DoorEffect = 1102;
    private const int SearchRewardSet = 9101;
    private const int DoorRewardSet = 9103;
    private const int SharedRewardSet = 9216;

    private static int GameAmount(ExposureSource source, int value) => value;

    private static ActionStep Step(int actionId, int effectId, int exposure, string tree = Bar) => new(actionId, effectId, tree, exposure);

    [Fact]
    public void A_search_costs_its_bar_and_its_reward_set()
    {
        var parts = MeterCosts.Parts(new RewardStep(SearchRewardSet, 120), new[] { Step(1104, SearchEffect, 50) });

        Assert.Equal(170, MeterCosts.Preview(parts, GameAmount));
    }

    [Fact]
    public void Opening_the_bag_after_a_search_adds_nothing()
    {
        var parts = MeterCosts.Parts(new RewardStep(0, 0), new[] { Step(1104, SearchEffect, 20), Step(1104, SearchEffect, 50, MeterCosts.OpenBagTree) });

        Assert.Equal(20, MeterCosts.Preview(parts, GameAmount));
    }

    [Fact]
    public void A_button_whose_reward_set_carries_the_exposure_still_has_a_cost()
    {
        var parts = MeterCosts.Parts(new RewardStep(DoorRewardSet, 400), new[] { Step(1102, DoorEffect, 0, "Action/Furniture_Bar_Demolish") });

        Assert.Equal(400, MeterCosts.Preview(parts, GameAmount));
    }

    [Fact]
    public void A_button_that_adds_no_exposure_has_no_parts()
    {
        Assert.Empty(MeterCosts.Parts(new RewardStep(0, 0), new[] { Step(1104, SearchEffect, 0), Step(1104, SearchEffect, 50, MeterCosts.OpenBagTree) }));
    }

    [Fact]
    public void Each_add_is_sorted_the_way_the_hooks_sort_it_in_play()
    {
        var search = MeterCosts.Parts(new RewardStep(SearchRewardSet, 150), new[] { Step(1104, SearchEffect, 100) });
        var door = MeterCosts.Parts(new RewardStep(DoorRewardSet, 30), new[] { Step(1102, DoorEffect, 40) });
        var pickup = MeterCosts.Parts(new RewardStep(0, 0), new[] { Step(1101, LootEffect, 10) });

        Assert.Equal(new[] { new CostPart(ExposureSource.Search, 100), new CostPart(ExposureSource.Search, 150) }, search);
        Assert.Equal(new[] { new CostPart(ExposureSource.Door, 40), new CostPart(ExposureSource.Door, 30) }, door);
        Assert.Equal(new[] { new CostPart(ExposureSource.Loot, 10) }, pickup);
    }

    [Fact]
    public void A_shared_reward_set_follows_the_action_that_finishes_the_button()
    {
        var pickup = MeterCosts.Parts(new RewardStep(SharedRewardSet, 60), new[] { Step(1104, SearchEffect, 0), Step(1106, LootEffect, 0) });
        var breakIn = MeterCosts.Parts(new RewardStep(SharedRewardSet, 60), new[] { Step(1104, SearchEffect, 0), Step(1945, DoorEffect, 0) });

        Assert.Equal(new[] { new CostPart(ExposureSource.Loot, 60) }, pickup);
        Assert.Equal(new[] { new CostPart(ExposureSource.Door, 60) }, breakIn);
    }

    [Fact]
    public void The_cost_shown_is_what_the_mod_lets_through_for_each_add()
    {
        var parts = MeterCosts.Parts(new RewardStep(SearchRewardSet, 120), new[] { Step(1104, SearchEffect, 50) });

        Assert.Equal(85, MeterCosts.Preview(parts, (_, value) => value / 2));
    }

    [Fact]
    public void A_per_kind_slowdown_changes_only_that_kind_of_cost()
    {
        var parts = MeterCosts.Parts(new RewardStep(DoorRewardSet, 100), new[] { Step(1104, SearchEffect, 50) });

        Assert.Equal(75, MeterCosts.Preview(parts, (source, value) => source == ExposureSource.Door ? value / 4 : value));
    }
}
