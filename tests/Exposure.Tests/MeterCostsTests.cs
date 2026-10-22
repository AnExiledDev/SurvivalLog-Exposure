using Exposure.Core;
using Xunit;

namespace Exposure.Tests;

public class MeterCostsTests
{
    private const string Bar = "Action/Furniture_Bar_Default";

    private static int GameAmount(ExposureSource source, int value) => value;

    [Fact]
    public void A_search_costs_its_bar_and_its_reward_set()
    {
        var parts = MeterCosts.Parts(120, new[] { new ActionStep(Bar, 50) });

        Assert.Equal(170, MeterCosts.Preview(parts, GameAmount));
    }

    [Fact]
    public void Opening_the_bag_after_a_search_adds_nothing()
    {
        var parts = MeterCosts.Parts(0, new[] { new ActionStep(Bar, 20), new ActionStep(MeterCosts.OpenBagTree, 50) });

        Assert.Equal(20, MeterCosts.Preview(parts, GameAmount));
    }

    [Fact]
    public void A_button_whose_reward_set_carries_the_exposure_still_has_a_cost()
    {
        var parts = MeterCosts.Parts(400, new[] { new ActionStep("Action/Furniture_Bar_Demolish", 0) });

        Assert.Equal(400, MeterCosts.Preview(parts, GameAmount));
    }

    [Fact]
    public void A_button_that_adds_no_exposure_has_no_parts()
    {
        Assert.Empty(MeterCosts.Parts(0, new[] { new ActionStep(Bar, 0), new ActionStep(MeterCosts.OpenBagTree, 50) }));
    }

    [Fact]
    public void Bars_count_as_searching_and_reward_sets_as_looting()
    {
        var parts = MeterCosts.Parts(150, new[] { new ActionStep(Bar, 100) });

        Assert.Equal(new[] { new CostPart(ExposureSource.Search, 100), new CostPart(ExposureSource.Loot, 150) }, parts);
    }

    [Fact]
    public void The_cost_shown_is_what_the_mod_lets_through_for_each_add()
    {
        var parts = MeterCosts.Parts(120, new[] { new ActionStep(Bar, 50) });

        Assert.Equal(85, MeterCosts.Preview(parts, (_, value) => value / 2));
    }
}
