namespace Exposure.Core;

/// <summary>
/// One expedition from its first update to its end, so its start and its end are each told exactly once even when
/// several of the game's calls could end it (a full bar, walking out, quitting the game).
/// </summary>
public sealed class TripTracker
{
    private enum Stage
    {
        Waiting,
        Running,
        Ended,
    }

    private Stage _stage = Stage.Waiting;

    public bool IsRunning => _stage == Stage.Running;

    /// <summary>Ready for the next expedition.</summary>
    public void Reset() => _stage = Stage.Waiting;

    /// <param name="hasGameEnded">Whether the game already finished this expedition, so it is not started again.</param>
    /// <returns>Whether an expedition starts now.</returns>
    public bool TryStart(bool hasGameEnded)
    {
        if (_stage != Stage.Waiting) return false;

        _stage = hasGameEnded ? Stage.Ended : Stage.Running;

        return IsRunning;
    }

    /// <returns>Whether the running expedition ends now.</returns>
    public bool TryEnd()
    {
        if (!IsRunning) return false;

        _stage = Stage.Ended;

        return true;
    }
}
