using System;
using System.Collections.Generic;
using Exposure.Core;
using GameCore.HotUpdate;
using GameCore.HotUpdate.Battle.Logic;
using HarmonyLib;

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
    private static Action<string> _log = _ => { };
    private static bool _hasReportedFailure;

    public static void Apply(Harmony harmony, Action<TripLoot?> capture, Action<string> log)
    {
        _capture = capture;
        _log = log;

        try
        {
            harmony.Patch(AccessTools.Method(typeof(ExploreManager), Method), postfix: new HarmonyMethod(typeof(TripSummaryLoot), nameof(AfterRewardSaved)));
            log($"hooked ExploreManager.{Method}");
        }
        catch (Exception ex)
        {
            log($"could not hook ExploreManager.{Method}, the trip summary leaves out what was brought home: {ex.Message}");
        }
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

            if (_hasReportedFailure) return;

            _hasReportedFailure = true;
            _log($"could not read what the trip brought home, the trip summary leaves it out (logged once): {ex.Message}");
        }
    }

    /// <summary>Null when the game saved no reward, so the summary says nothing rather than "nothing new".</summary>
    private static TripLoot? Read()
    {
        var save = BaseSingleton<GameSaveManager>.IsInstanceCreated ? BaseSingleton<GameSaveManager>.Instance?._gameSave_k__BackingField : null;
        var reward = save?._ExploreReward_k__BackingField;

        if (reward == null) return null;

        var items = reward._NewItems_k__BackingField;

        if (items == null) return TripLoot.Nothing;

        var config = BaseSingleton<ConfigManager>.IsInstanceCreated ? BaseSingleton<ConfigManager>.Instance : null;
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
