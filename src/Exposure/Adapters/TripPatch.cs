using System;
using GameCore.HotUpdate;
using GameCore.HotUpdate.Battle.Logic;
using HarmonyLib;

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
    private static readonly FailureLog Failure = new("expedition start or end could not be read");

    public static void Apply(Harmony harmony)
    {
        Hooks.Patch(harmony, typeof(ExploreManager), nameof(ExploreManager.Initialize), typeof(TripPatch), null, nameof(AfterInitialize), "a new expedition is only noticed on its first update", new[] { typeof(GameSaveData), typeof(GameMode) });
        Hooks.Patch(harmony, typeof(ExploreManager), "OnExploreEnd", typeof(TripPatch), nameof(BeforeExploreEnd), null, "the end of an expedition is not reported");
        Hooks.Patch(harmony, typeof(BattleLogicWorld), "OnDestroy", typeof(TripPatch), nameof(BeforeWorldDestroy), null, "an expedition left by quitting is not reported as ended", Type.EmptyTypes);
    }

    private static void AfterInitialize(ExploreManager __instance)
    {
        try
        {
            ExposurePatch.UseManager(__instance);
        }
        catch (Exception ex)
        {
            Failure.Report(ex);
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
            Failure.Report(ex);
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
            Failure.Report(ex);
        }
    }
}
