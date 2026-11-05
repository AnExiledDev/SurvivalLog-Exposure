using System;
using ExiledToolkit.Game;
using GameCore.HotUpdate;
using GameCore.HotUpdate.Battle.Logic;

namespace Exposure.Adapters;

/// <summary>
/// The start and end of an expedition. Each world builds its own explore manager and initializes it for the game
/// mode; it is unlocked only on an expedition. Every end of an expedition, a full bar or walking out, goes through
/// OnExploreEnd, which runs once (it sets _hasEndProcessed) and before the game leaves the map. Quitting the game
/// from an expedition skips it: BattleMain's teardown clears the world, and BattleLogicWorld.OnDestroy destroys the
/// explore manager, so the end is told there too. The tracker in ExposurePatch tells only the first end of a trip.
/// </summary>
internal static class TripPatch
{
    private const string Failure = "reading an expedition's start or end";

    private static PatchKit? _hooks;

    public static void Apply(PatchKit hooks)
    {
        _hooks = hooks;

        hooks.Patch(typeof(TripPatch), typeof(ExploreManager), nameof(ExploreManager.Initialize), new[] { typeof(GameSaveData), typeof(GameMode) }, postfix: nameof(AfterInitialize),
            done: "hooked ExploreManager.Initialize", consequence: "a new expedition is only noticed on its first update");

        hooks.Patch(typeof(TripPatch), typeof(ExploreManager), "OnExploreEnd", prefix: nameof(BeforeExploreEnd),
            done: "hooked ExploreManager.OnExploreEnd", consequence: "the end of an expedition is not reported");

        hooks.Patch(typeof(TripPatch), typeof(BattleLogicWorld), "OnDestroy", Type.EmptyTypes, prefix: nameof(BeforeWorldDestroy),
            done: "hooked BattleLogicWorld.OnDestroy", consequence: "an expedition left by quitting is not reported as ended");
    }

    private static void AfterInitialize(ExploreManager __instance)
    {
        try
        {
            ExposurePatch.UseManager(__instance);
        }
        catch (Exception ex)
        {
            _hooks?.Fail(Failure, ex);
        }
    }

    /// <summary>Before the game's end of the trip, while the bar and the map are still there.</summary>
    private static void BeforeExploreEnd(ExploreManager __instance)
    {
        try
        {
            if (!__instance._hasEndProcessed) ExposurePatch.EndTrip(__instance);
        }
        catch (Exception ex)
        {
            _hooks?.Fail(Failure, ex);
        }
    }

    private static void BeforeWorldDestroy()
    {
        try
        {
            ExposurePatch.EndTrip(null);
        }
        catch (Exception ex)
        {
            _hooks?.Fail(Failure, ex);
        }
    }
}
