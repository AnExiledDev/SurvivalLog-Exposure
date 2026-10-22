namespace Exposure.Core;

public static class ExposureGains
{
    /// <summary>
    /// Shares one frame's rise between time and running by their rates, since the game adds both in one step.
    /// Running only counts while the character runs. A frame with no positive rate puts the whole rise on time.
    /// </summary>
    public static (double Time, double Running) SplitFrame(double gained, float timeRate, float runRate, bool isRunning)
    {
        if (!(gained > 0)) return (0, 0);

        var time = timeRate > 0 ? timeRate : 0.0;
        var running = isRunning && runRate > 0 ? runRate : 0.0;
        var total = time + running;

        if (total <= 0) return (gained, 0);

        return (gained * time / total, gained * running / total);
    }
}
