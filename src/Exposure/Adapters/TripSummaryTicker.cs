using System;
using UnityEngine;

namespace Exposure.Adapters;

/// <summary>Twice a second, on real time so a paused game still gets its summary, checks whether a waiting trip summary is due.</summary>
public sealed class TripSummaryTicker : MonoBehaviour
{
    private const float CheckSeconds = 0.5f;

    private float _nextCheckAt;

    public TripSummaryTicker(IntPtr pointer) : base(pointer)
    {
    }

    private void Update()
    {
        var now = Time.unscaledTime;

        if (now < _nextCheckAt) return;

        _nextCheckAt = now + CheckSeconds;
        TripSummary.Check(now);
    }
}
