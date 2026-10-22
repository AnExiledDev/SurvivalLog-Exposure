using System;
using Exposure.Core;
using GameCore.HotUpdate.Battle.Logic;

namespace Exposure.Adapters;

/// <summary>
/// What the exposure hooks tell the rest of the mod. The hooks raise every rise after the mod's change, with its
/// source, and set how a one-off add would come out now; the meter, warnings and trip summary only listen. Everything
/// here runs on the main thread, inside the game's update.
/// </summary>
internal static class ExposureFeed
{
    /// <summary>A rise in exposure, in points, after the mod's change. Per-frame time and running rises are raised each frame.</summary>
    public static event Action<ExposureSource, double>? Gained;

    /// <summary>After each of the game's explore updates that was not locked, with the manager and the frame's unscaled seconds.</summary>
    public static event Action<ExploreManager, float>? Ticked;

    /// <summary>An expedition started (the explore manager's first update after none was running).</summary>
    public static event Action? TripStarted;

    /// <summary>An expedition ended, by walking out or by a full bar.</summary>
    public static event Action? TripEnded;

    /// <summary>
    /// The points a one-off add of the game's <c>value</c> from <c>source</c> would add now, without using up any carried
    /// fraction. Set by the hooks; until then it is the game's own amount.
    /// </summary>
    public static Func<ExposureSource, int, int> PreviewOneOff { get; set; } = (_, value) => value;

    public static void RaiseGained(ExposureSource source, double amount) => Gained?.Invoke(source, amount);

    public static void RaiseTicked(ExploreManager manager, float unscaledSeconds) => Ticked?.Invoke(manager, unscaledSeconds);

    public static void RaiseTripStarted() => TripStarted?.Invoke();

    public static void RaiseTripEnded() => TripEnded?.Invoke();
}
