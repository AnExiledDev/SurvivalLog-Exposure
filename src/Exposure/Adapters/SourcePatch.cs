using System;
using ExiledToolkit.Game;
using Exposure.Core;
using GameCore.HotUpdate.Battle.Logic;
using Il2CppInterop.Runtime;

namespace Exposure.Adapters;

/// <summary>
/// Notes what is adding exposure while the game runs it, so AddExposure, which is only given an amount, knows its
/// source. An action's effect adds through AE_AddExploreByConfig.Run, which reads its Config_Effect and calls
/// AddExposure with the effect's ExposureValue. A reward set adds through RewardSetManager.ExecuteRewardSet; a
/// furniture reward runs it from ActionManager.TryExecuteFurnitureFuncReward with the action the character did.
/// Each hook puts back the source it found, after the game's call or when it fails, so a nested call wins only inside.
/// </summary>
internal static class SourcePatch
{
    private const string Failure = "reading an exposure source, it counts as other";

    private static PatchKit? _hooks;
    private static ExposureSource? _source;
    private static int? _furnitureActionId;

    public struct Saved
    {
        public ExposureSource? Source;
        public int? FurnitureActionId;
    }

    /// <summary>What is adding exposure now; anything the hooks did not see counts as other.</summary>
    public static ExposureSource Current => _source ?? ExposureSource.Other;

    public static void Apply(PatchKit hooks)
    {
        const string consequence = "its exposure uses the loot setting";

        _hooks = hooks;

        hooks.Patch(typeof(SourcePatch), typeof(AE_AddExploreByConfig), nameof(AE_AddExploreByConfig.Run), prefix: nameof(BeforeEffect), finalizer: nameof(Restore),
            done: "hooked AE_AddExploreByConfig.Run", consequence: consequence);

        hooks.Patch(typeof(SourcePatch), typeof(ActionManager), "TryExecuteFurnitureFuncReward", prefix: nameof(BeforeFurnitureReward), finalizer: nameof(Restore),
            done: "hooked ActionManager.TryExecuteFurnitureFuncReward", consequence: consequence);

        hooks.Patch(typeof(SourcePatch), typeof(RewardSetManager), nameof(RewardSetManager.ExecuteRewardSet), prefix: nameof(BeforeRewardSet), finalizer: nameof(Restore),
            done: "hooked RewardSetManager.ExecuteRewardSet", consequence: consequence);
    }

    private static void BeforeEffect(EffectBtArgs args, out Saved __state)
    {
        __state = Save();

        try
        {
            _source = EffectId(args) is { } effectId ? ExposureSources.ForEffect(effectId) : ExposureSource.Other;
        }
        catch (Exception ex)
        {
            _source = ExposureSource.Other;
            _hooks?.Fail(Failure, ex);
        }
    }

    private static void BeforeFurnitureReward(AgentAction action, out Saved __state)
    {
        __state = Save();

        try
        {
            _furnitureActionId = action?._ActionId_k__BackingField;
        }
        catch (Exception ex)
        {
            _furnitureActionId = null;
            _hooks?.Fail(Failure, ex);
        }
    }

    private static void BeforeRewardSet(int rewardSetId, out Saved __state)
    {
        __state = Save();
        _source = ExposureSources.ForRewardSet(rewardSetId, _furnitureActionId);
    }

    private static void Restore(Saved __state)
    {
        _source = __state.Source;
        _furnitureActionId = __state.FurnitureActionId;
    }

    private static Saved Save() => new() { Source = _source, FurnitureActionId = _furnitureActionId };

    /// <summary>The effect's config, as EffectBtArgsExtension.GetComponent&lt;EffectConfigArgs&gt; finds it in ChildArgsMap.</summary>
    private static int? EffectId(EffectBtArgs? args)
    {
        var map = args?.ChildArgsMap;
        var key = Il2CppType.Of<EffectConfigArgs>();

        if (map == null || !map.ContainsKey(key)) return null;

        return map[key]?.TryCast<EffectConfigArgs>()?.Config?._ID_k__BackingField;
    }
}
