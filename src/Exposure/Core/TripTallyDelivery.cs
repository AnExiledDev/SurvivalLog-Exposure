namespace Exposure.Core;

/// <summary>What a check on a waiting trip summary calls for.</summary>
public enum DeliveryAction
{
    None,
    Show,
    GiveUp,
}

/// <param name="EndedAt">When the trip ended, in unscaled seconds.</param>
/// <param name="HomeSince">When home was first seen ready since then, if it has been.</param>
public readonly record struct SummaryWait(float EndedAt, float? HomeSince);

/// <param name="Wait">The wait to keep, or null once it is over.</param>
public readonly record struct DeliveryStep(SummaryWait? Wait, DeliveryAction Action);

/// <summary>
/// A trip summary waits for the trip home to load, and then a little more so it does not land in the middle of the
/// game's own arrival screens. If home never comes (the player quit to the menu), it gives up rather than showing
/// up on a later visit home.
/// </summary>
public static class TripTallyDelivery
{
    public const float SettleSeconds = 3f;
    public const float GiveUpSeconds = 300f;

    public static DeliveryStep Next(SummaryWait? wait, float now, bool isHomeReady)
    {
        if (wait is not { } waiting) return new DeliveryStep(null, DeliveryAction.None);

        if (now - waiting.EndedAt > GiveUpSeconds) return new DeliveryStep(null, DeliveryAction.GiveUp);

        if (!isHomeReady) return new DeliveryStep(waiting with { HomeSince = null }, DeliveryAction.None);

        if (waiting.HomeSince is not { } homeSince) return new DeliveryStep(waiting with { HomeSince = now }, DeliveryAction.None);

        return now - homeSince >= SettleSeconds
            ? new DeliveryStep(null, DeliveryAction.Show)
            : new DeliveryStep(waiting, DeliveryAction.None);
    }
}
