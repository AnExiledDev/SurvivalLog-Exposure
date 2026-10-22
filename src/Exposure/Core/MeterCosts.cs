using System;
using System.Collections.Generic;

namespace Exposure.Core;

/// <summary>One action a furniture button runs: its id, its effect's id, the behaviour tree the effect plays and the effect's exposure.</summary>
public readonly record struct ActionStep(int ActionId, int EffectId, string Tree, int Exposure);

/// <summary>The reward set a furniture button runs when it is done, and the exposure that set adds.</summary>
public readonly record struct RewardStep(int RewardSetId, int Exposure);

/// <summary>One add of exposure a button makes when it is done, as the game's amount before the mod's change.</summary>
public readonly record struct CostPart(ExposureSource Source, int GameValue);

/// <summary>
/// What a furniture button (search, open, unlock, pry open, pick up) adds to the bar. Every amount is a fixed number
/// in the game's tables, so the cost is known before the button is pressed: each action's effect adds its exposure
/// once its bar finishes, and the button's reward set adds its own. The open-bag action's effect also carries an
/// exposure number, but its behaviour tree only opens the bag and never adds it. Each add is sorted the way the hooks
/// sort it in play, so a per-kind slowdown shows in the cost: an effect by its id, and the reward set by its id and the
/// button's last action, which is the one that finishes and runs the reward.
/// </summary>
public static class MeterCosts
{
    public const string OpenBagTree = "Action/Default_OpenBag";

    /// <summary>Each add the button makes, in the order the game makes them. Empty when the button adds nothing.</summary>
    public static IReadOnlyList<CostPart> Parts(RewardStep reward, IReadOnlyList<ActionStep> actions)
    {
        var parts = new List<CostPart>();

        foreach (var action in actions)
        {
            if (action.Exposure > 0 && action.Tree != OpenBagTree) parts.Add(new CostPart(ExposureSources.ForEffect(action.EffectId), action.Exposure));
        }

        int? finishingAction = actions.Count > 0 ? actions[actions.Count - 1].ActionId : null;

        if (reward.Exposure > 0) parts.Add(new CostPart(ExposureSources.ForRewardSet(reward.RewardSetId, finishingAction), reward.Exposure));

        return parts;
    }

    /// <summary>The points the button would add now, each add passed through the mod's change on its own as the game makes it.</summary>
    public static int Preview(IReadOnlyList<CostPart> parts, Func<ExposureSource, int, int> previewOneOff)
    {
        var total = 0;

        foreach (var part in parts) total += previewOneOff(part.Source, part.GameValue);

        return total;
    }
}
