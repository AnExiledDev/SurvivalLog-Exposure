using Exposure.Core;
using Xunit;

namespace Exposure.Tests;

public class TripTallyTests
{
    private const char LineBreak = (char)10;

    [Fact]
    public void Exposure_gained_on_a_trip_adds_up_by_source()
    {
        var tally = Started();

        tally.Add(ExposureSource.Time, 4);
        tally.Add(ExposureSource.Time, 1.5);
        tally.Add(ExposureSource.Event, 10);

        Assert.Equal(5.5, tally.Of(ExposureSource.Time), 6);
        Assert.Equal(10, tally.Of(ExposureSource.Event), 6);
        Assert.Equal(15.5, tally.Total(), 6);
    }

    [Fact]
    public void Exposure_before_a_trip_starts_or_after_it_ends_is_not_counted()
    {
        var tally = new TripTally();

        tally.Add(ExposureSource.Time, 3);
        tally.Start();
        tally.Add(ExposureSource.Time, 2);
        tally.End();
        tally.Add(ExposureSource.Time, 7);

        Assert.Equal(2, tally.Total(), 6);
    }

    [Fact]
    public void A_new_trip_starts_from_nothing()
    {
        var tally = Started();

        tally.Add(ExposureSource.Running, 9);
        tally.End();
        tally.Start();

        Assert.Equal(0, tally.Total(), 6);
    }

    [Fact]
    public void Drops_and_broken_amounts_are_not_counted()
    {
        var tally = Started();

        tally.Add(ExposureSource.Time, -5);
        tally.Add(ExposureSource.Time, 0);
        tally.Add(ExposureSource.Time, double.NaN);
        tally.Add(ExposureSource.Time, double.PositiveInfinity);
        tally.Add((ExposureSource)99, 4);

        Assert.Equal(0, tally.Total(), 6);
    }

    [Fact]
    public void The_summary_lists_each_source_that_added_exposure_and_leaves_out_the_rest()
    {
        var tally = Started();

        tally.Add(ExposureSource.Time, 12.4);
        tally.Add(ExposureSource.Event, 3.2);

        var lines = Lines(TripTallyText.Body(tally, null, isChinese: false));

        Assert.Equal(new[] { "Time: 12", "Events: 3.2" }, lines);
    }

    [Fact]
    public void Searching_looting_and_doors_share_one_line()
    {
        var tally = Started();

        tally.Add(ExposureSource.Search, 2);
        tally.Add(ExposureSource.Loot, 3);
        tally.Add(ExposureSource.Door, 1);

        Assert.Equal(new[] { "Searching, looting and doors: 6" }, Lines(TripTallyText.Body(tally, null, isChinese: false)));
    }

    [Fact]
    public void What_came_home_is_the_first_line_so_it_shows_before_the_notice_is_opened()
    {
        var tally = Started();

        tally.Add(ExposureSource.Running, 4);

        var lines = Lines(TripTallyText.Body(tally, new TripLoot(2, 5, 120), isChinese: false));

        Assert.Equal(new[] { "Brought home: 5 items (2 kinds), worth 120$", "Running: 4" }, lines);
    }

    [Fact]
    public void A_trip_that_added_no_exposure_says_so()
    {
        var lines = Lines(TripTallyText.Body(Started(), TripLoot.Nothing, isChinese: false));

        Assert.Equal(new[] { "Brought home: nothing new", "No exposure added on this trip." }, lines);
    }

    [Fact]
    public void Loot_that_could_not_be_read_gets_no_line_rather_than_a_false_nothing()
    {
        var tally = Started();

        tally.Add(ExposureSource.Time, 1);

        Assert.DoesNotContain("Brought home", TripTallyText.Body(tally, null, isChinese: false));
        Assert.Contains("could not be read", TripTallyText.LogLine(tally, null));
    }

    [Fact]
    public void The_chinese_summary_has_the_same_lines_in_chinese()
    {
        var tally = Started();

        tally.Add(ExposureSource.Time, 2);

        var english = Lines(TripTallyText.Body(tally, new TripLoot(1, 1, 0), isChinese: false));
        var chinese = Lines(TripTallyText.Body(tally, new TripLoot(1, 1, 0), isChinese: true));

        Assert.Equal(english.Length, chinese.Length);
        Assert.NotEqual(english[0], chinese[0]);
        Assert.NotEqual(TripTallyText.Title(isChinese: false), TripTallyText.Title(isChinese: true));
    }

    [Fact]
    public void One_item_reads_as_one_item_and_no_worth_is_shown_for_worthless_loot()
    {
        Assert.Equal("Brought home: 1 item (1 kind)", TripTallyText.LootLine(new TripLoot(1, 1, 0), isChinese: false));
    }

    [Fact]
    public void Loot_counts_every_new_item_and_its_price_and_skips_entries_of_none()
    {
        var loot = TripLoot.FromItems(new[] { (3, 10), (1, 250), (0, 999), (-2, 50), (2, -4) });

        Assert.Equal(new TripLoot(Kinds: 3, Count: 6, Value: 280), loot);
    }

    [Fact]
    public void The_log_line_keeps_every_source_and_the_total()
    {
        var tally = Started();

        tally.Add(ExposureSource.Door, 0.5);
        tally.Add(ExposureSource.Other, 8);

        var line = TripTallyText.LogLine(tally, new TripLoot(1, 2, 30));

        Assert.Contains("door 0.5", line);
        Assert.Contains("other 8", line);
        Assert.Contains("time 0", line);
        Assert.Contains("total 8.5", line);
        Assert.Contains("brought home 2 items (1 kinds) worth 30$", line);
    }

    [Fact]
    public void The_summary_waits_for_home_and_then_a_few_seconds_more()
    {
        var wait = new SummaryWait(EndedAt: 100, HomeSince: null);

        var away = TripTallyDelivery.Next(wait, 110, isHomeReady: false);
        var arrived = TripTallyDelivery.Next(away.Wait, 120, isHomeReady: true);
        var settling = TripTallyDelivery.Next(arrived.Wait, 120 + TripTallyDelivery.SettleSeconds - 0.5f, isHomeReady: true);
        var settled = TripTallyDelivery.Next(settling.Wait, 120 + TripTallyDelivery.SettleSeconds, isHomeReady: true);

        Assert.Equal(DeliveryAction.None, away.Action);
        Assert.Equal(DeliveryAction.None, arrived.Action);
        Assert.Equal(DeliveryAction.None, settling.Action);
        Assert.Equal(DeliveryAction.Show, settled.Action);
        Assert.Null(settled.Wait);
    }

    [Fact]
    public void Home_going_away_again_restarts_the_settling_wait()
    {
        var arrived = TripTallyDelivery.Next(new SummaryWait(0, null), 10, isHomeReady: true);
        var gone = TripTallyDelivery.Next(arrived.Wait, 11, isHomeReady: false);
        var back = TripTallyDelivery.Next(gone.Wait, 12, isHomeReady: true);

        Assert.Equal(DeliveryAction.None, TripTallyDelivery.Next(back.Wait, 14, isHomeReady: true).Action);
        Assert.Equal(DeliveryAction.Show, TripTallyDelivery.Next(back.Wait, 15, isHomeReady: true).Action);
    }

    [Fact]
    public void A_summary_whose_home_never_loads_is_dropped_rather_than_shown_on_a_later_visit()
    {
        var wait = new SummaryWait(EndedAt: 0, HomeSince: null);

        var step = TripTallyDelivery.Next(wait, TripTallyDelivery.GiveUpSeconds + 1, isHomeReady: true);

        Assert.Equal(DeliveryAction.GiveUp, step.Action);
        Assert.Null(step.Wait);
    }

    [Fact]
    public void Nothing_happens_with_no_summary_waiting()
    {
        var step = TripTallyDelivery.Next(null, 50, isHomeReady: true);

        Assert.Equal(DeliveryAction.None, step.Action);
        Assert.Null(step.Wait);
    }

    private static TripTally Started()
    {
        var tally = new TripTally();

        tally.Start();

        return tally;
    }

    private static string[] Lines(string body) => body.Split(LineBreak);
}
