using Exposure.Core;
using Xunit;

namespace Exposure.Tests;

public class TripTrackerTests
{
    [Fact]
    public void An_expedition_starts_once_and_ends_once()
    {
        var trip = new TripTracker();

        Assert.True(trip.TryStart(hasGameEnded: false));
        Assert.False(trip.TryStart(hasGameEnded: false));
        Assert.True(trip.TryEnd());
        Assert.False(trip.TryEnd());
    }

    [Fact]
    public void An_expedition_does_not_start_again_after_it_ended()
    {
        var trip = new TripTracker();

        trip.TryStart(hasGameEnded: false);
        trip.TryEnd();

        Assert.False(trip.TryStart(hasGameEnded: false));
    }

    [Fact]
    public void An_expedition_the_game_already_finished_neither_starts_nor_ends()
    {
        var trip = new TripTracker();

        Assert.False(trip.TryStart(hasGameEnded: true));
        Assert.False(trip.TryEnd());
    }

    [Fact]
    public void Nothing_ends_before_an_expedition_starts()
    {
        Assert.False(new TripTracker().TryEnd());
    }

    [Fact]
    public void After_a_reset_the_next_expedition_starts_and_ends()
    {
        var trip = new TripTracker();

        trip.TryStart(hasGameEnded: false);
        trip.TryEnd();
        trip.Reset();

        Assert.True(trip.TryStart(hasGameEnded: false));
        Assert.True(trip.TryEnd());
    }
}
