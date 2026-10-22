using System;
using System.Collections.Generic;

namespace Exposure.Core;

/// <summary>What a trip brought home, as the game lists it on its reward screen.</summary>
/// <param name="Kinds">How many different items.</param>
/// <param name="Count">How many items in all.</param>
/// <param name="Value">The items' total price, in the game's $.</param>
public sealed record TripLoot(int Kinds, int Count, long Value)
{
    public static readonly TripLoot Nothing = new(0, 0, 0);

    /// <summary>The game lists one entry per item kind, with how many of it are new; an entry of none adds nothing.</summary>
    public static TripLoot FromItems(IEnumerable<(int Count, int Price)> items)
    {
        var kinds = 0;
        var count = 0;
        var value = 0L;

        foreach (var (itemCount, price) in items)
        {
            if (itemCount <= 0) continue;

            kinds++;
            count += itemCount;
            value += (long)Math.Max(price, 0) * itemCount;
        }

        return new TripLoot(kinds, count, value);
    }
}

/// <summary>
/// The exposure one trip added, by source, from its start to its end. Only rises count: a drop is not taken off the
/// source that added the exposure, as the game does not say which one it lowers.
/// </summary>
public sealed class TripTally
{
    private static readonly int SourceCount = Enum.GetValues(typeof(ExposureSource)).Length;

    private readonly double[] _gained = new double[SourceCount];

    public bool IsCollecting { get; private set; }

    public void Start()
    {
        Array.Clear(_gained, 0, _gained.Length);
        IsCollecting = true;
    }

    public void End() => IsCollecting = false;

    public void Add(ExposureSource source, double amount)
    {
        if (!IsCollecting || !(amount > 0) || double.IsInfinity(amount)) return;

        var index = (int)source;

        if (index < 0 || index >= SourceCount) return;

        _gained[index] += amount;
    }

    public double Of(ExposureSource source)
    {
        var index = (int)source;

        return index >= 0 && index < SourceCount ? _gained[index] : 0;
    }

    public double Total()
    {
        var total = 0.0;

        foreach (var amount in _gained) total += amount;

        return total;
    }
}
