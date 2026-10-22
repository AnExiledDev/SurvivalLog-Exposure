using System.Collections.Generic;

namespace Exposure.Core;

/// <summary>
/// How fast the bar fills, in points per real second, from the exposure read after each of the game's explore
/// updates. Only the steady rise from time and running counts: a jump bigger than the steady rise can make in one
/// update is a search or loot add, and is left out so the time to full assumes the player keeps doing what they do.
/// A drop is left out too. The rise is averaged over the last few seconds so a single frame does not swing it.
/// </summary>
public sealed class MeterRate
{
    /// <summary>The rise is averaged over this many real seconds.</summary>
    public const double WindowSeconds = 8;

    /// <summary>With no steady rise for this long (a pause, a locked bar), the bar reads as not rising.</summary>
    public const double StillSeconds = 1;

    /// <summary>Less than this much time read gives no rate yet.</summary>
    public const double MinSpanSeconds = 1;

    /// <summary>A rise faster than this within one update is a one-off add, not the steady rise.</summary>
    public const double StepPointsPerSecond = 300;

    private readonly Queue<(double Seconds, double Rise)> _samples = new();
    private double? _lastExposure;
    private double _span;
    private double _risen;
    private double _sinceRise;

    /// <summary>One explore update: the real seconds it took and the exposure after it.</summary>
    public void Add(double seconds, double exposure)
    {
        if (_lastExposure is not { } last)
        {
            _lastExposure = exposure;

            return;
        }

        var change = exposure - last;
        var isStep = change > 0 && (seconds <= 0 || change / seconds > StepPointsPerSecond);
        var rise = change > 0 && !isStep ? change : 0;

        _lastExposure = exposure;
        _sinceRise = rise > 0 ? 0 : _sinceRise + seconds;

        if (seconds <= 0) return;

        _samples.Enqueue((seconds, rise));
        (_span, _risen) = (_span + seconds, _risen + rise);
        DropOld();
    }

    /// <summary>The steady rise in points per real second, or null when the bar is not rising.</summary>
    public double? PointsPerSecond
    {
        get
        {
            if (_span < MinSpanSeconds || _sinceRise >= StillSeconds || _risen <= 0) return null;

            return _risen / _span;
        }
    }

    /// <summary>A new expedition, or a new explore manager: nothing read before says anything about now.</summary>
    public void Reset()
    {
        _samples.Clear();
        _lastExposure = null;
        (_span, _risen, _sinceRise) = (0, 0, 0);
    }

    private void DropOld()
    {
        while (_samples.Count > 1 && _span - _samples.Peek().Seconds >= WindowSeconds)
        {
            var (seconds, rise) = _samples.Dequeue();

            (_span, _risen) = (_span - seconds, _risen - rise);
        }
    }
}
