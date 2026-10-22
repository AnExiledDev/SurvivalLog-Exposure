using System;
using System.Collections.Generic;
using Exposure.Core;
using GameCore.HotUpdate;

namespace Exposure.Adapters;

/// <summary>
/// Every furniture button that adds exposure, by its function id, read from the game's tables: the actions it runs
/// (Config_FurnitureFunc.ActionIds), each action's effect (Config_Action.EffectConfigID) with that effect's behaviour
/// tree and exposure, and the button's reward set (Config_RewardSet.ExposureValue). The furniture menu is sent each
/// button's function id, so page.js can find a button's cost by it. The tables are read straight, since the game's
/// Get_Config_* lookups log an error for an id they do not have.
/// </summary>
internal sealed class MeterCostTable
{
    private readonly Dictionary<int, IReadOnlyList<CostPart>> _parts;

    private MeterCostTable(Dictionary<int, IReadOnlyList<CostPart>> parts) => _parts = parts;

    /// <summary>Null until the game has made its config manager and loaded these tables.</summary>
    public static MeterCostTable? Read(ConfigManager? config)
    {
        var funcs = config?._Config_FurnitureFunc_Dict;
        var actions = config?._Config_Action_Dict;
        var effects = config?._Config_Effect_Dict;
        var rewards = config?._Config_RewardSet_Dict;

        if (funcs == null || actions == null || effects == null || rewards == null || funcs.Count == 0) return null;

        var parts = new Dictionary<int, IReadOnlyList<CostPart>>();

        foreach (var func in funcs.Values)
        {
            if (func == null) continue;

            var reward = func.RewardSetId > 0 && rewards.TryGetValue(func.RewardSetId, out var set) && set != null ? set.ExposureValue : 0;
            var funcParts = MeterCosts.Parts(reward, Steps(func, actions, effects));

            if (funcParts.Count > 0) parts[func.ID] = funcParts;
        }

        return new MeterCostTable(parts);
    }

    /// <summary>Each button's cost now, through the mod's change to one-off adds.</summary>
    public Dictionary<int, int> Preview(Func<ExposureSource, int, int> previewOneOff)
    {
        var costs = new Dictionary<int, int>(_parts.Count);

        foreach (var (funcId, parts) in _parts) costs[funcId] = MeterCosts.Preview(parts, previewOneOff);

        return costs;
    }

    private static List<ActionStep> Steps(Config_FurnitureFunc func, Il2CppSystem.Collections.Generic.Dictionary<int, Config_Action> actions,
        Il2CppSystem.Collections.Generic.Dictionary<int, Config_Effect> effects)
    {
        var steps = new List<ActionStep>();
        var ids = func.ActionIds;

        if (ids == null) return steps;

        for (var i = 0; i < ids.Count; i++)
        {
            var effect = EffectOf(ids[i], actions, effects);

            if (effect != null) steps.Add(new ActionStep(effect.ConfigName ?? "", effect.ExposureValue));
        }

        return steps;
    }

    private static Config_Effect? EffectOf(int actionId, Il2CppSystem.Collections.Generic.Dictionary<int, Config_Action> actions,
        Il2CppSystem.Collections.Generic.Dictionary<int, Config_Effect> effects) =>
        actions.TryGetValue(actionId, out var action) && action != null && effects.TryGetValue(action.EffectConfigID, out var effect) ? effect : null;
}
