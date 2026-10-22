namespace Exposure.Core;

/// <summary>Where a rise in exposure came from. One-off adds the game cannot tell apart count as <see cref="Other"/>.</summary>
public enum ExposureSource
{
    Time,
    Running,
    Search,
    Loot,
    Door,
    Event,
    Other,
}
