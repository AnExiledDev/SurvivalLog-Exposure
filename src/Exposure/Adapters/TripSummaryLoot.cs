using System;
using System.Collections.Generic;
using ExiledToolkit.Game;
using Exposure.Core;
using GameCore.HotUpdate;
using GameCore.HotUpdate.Battle.Logic;

namespace Exposure.Adapters;

/// <summary>
/// What a trip brought home, read the moment the game works it out. When a trip ends, ExploreManager's
/// CalculateAndSaveReward compares the bag and the vehicle's trunk with what they held at the start and saves every
/// item there is more of as the trip's reward (the list the game's reward screen shows at home), counting items
/// picked up while fleeing a full bar too. The value is each item's price, as its details panel shows it.
/// </summary>
internal static class TripSummaryLoot
{
    private const string Method = nameof(ExploreManager.CalculateAndSaveReward);

    private static Action<TripLoot?> _capture = _ => { };
    private static PatchKit? _hooks;

    public static void Apply(PatchKit hooks, Action<TripLoot?> capture)
    {
        _capture = capture;
        _hooks = hooks;

        hooks.Patch(typeof(TripSummaryLoot), typeof(ExploreManager), Method, postfix: nameof(AfterRewardSaved),
            done: $"hooked ExploreManager.{Method}", consequence: "the trip summary leaves out what was brought home");
    }

    private static void AfterRewardSaved()
    {
        try
        {
            _capture(Read());
        }
        catch (Exception ex)
        {
            _capture(null);
            _hooks?.Fail("reading what the trip brought home, the trip summary leaves it out", ex);
        }
    }

    /// <summary>Null when the game saved no reward, so the summary says nothing rather than "nothing new".</summary>
    private static TripLoot? Read()
    {
        var save = GameSingleton.Existing<GameSaveManager>()?._gameSave_k__BackingField;
        var reward = save?._ExploreReward_k__BackingField;

        if (reward == null) return null;

        var items = reward._NewItems_k__BackingField;

        if (items == null) return TripLoot.Nothing;

        var config = GameSingleton.Existing<ConfigManager>();
        var entries = new List<(int Count, int Price)>(items.Count);

        for (var i = 0; i < items.Count; i++)
        {
            var item = items[i];

            if (item == null) continue;

            entries.Add((item._ItemCount_k__BackingField, config?.Get_Config_Item(item._ItemConfigId_k__BackingField)?.price ?? 0));
        }

        return TripLoot.FromItems(entries);
    }
}
