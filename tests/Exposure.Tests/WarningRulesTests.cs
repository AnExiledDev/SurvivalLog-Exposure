using Exposure.Core;
using Xunit;

namespace Exposure.Tests;

public class WarningRulesTests
{
    private static readonly WarningLevels Default = new(new[] { 50, 75, 90 }, PauseAt: 0);

    [Fact]
    public void The_default_list_reads_as_three_percents()
    {
        Assert.Equal(new[] { 50, 75, 90 }, WarningRules.ParseThresholds("50,75,90"));
    }

    [Fact]
    public void A_list_in_any_order_with_spaces_percent_signs_and_repeats_reads_lowest_first_once_each()
    {
        Assert.Equal(new[] { 25, 60, 80 }, WarningRules.ParseThresholds(" 80%, 25 ;60 80 "));
    }

    [Fact]
    public void Percents_split_by_chinese_commas_are_read()
    {
        var text = "30" + (char)0xFF0C + "60" + (char)0x3001 + "90";

        Assert.Equal(new[] { 30, 60, 90 }, WarningRules.ParseThresholds(text));
    }

    [Fact]
    public void Words_zero_and_percents_past_the_bar_are_skipped()
    {
        Assert.Equal(new[] { 1, 100 }, WarningRules.ParseThresholds("half, 0, 1, 100, 101, -5, 50.5"));
    }

    [Fact]
    public void An_empty_list_means_no_warnings()
    {
        Assert.Empty(WarningRules.ParseThresholds(""));
        Assert.Empty(WarningRules.ParseThresholds(null));
    }

    [Fact]
    public void A_pause_percent_below_one_is_off_and_one_past_the_bar_is_the_full_bar()
    {
        Assert.Equal(0, WarningRules.ClampPauseAt(-10));
        Assert.Equal(0, WarningRules.ClampPauseAt(0));
        Assert.Equal(80, WarningRules.ClampPauseAt(80));
        Assert.Equal(100, WarningRules.ClampPauseAt(250));
    }

    [Fact]
    public void The_fill_is_a_percent_of_the_bar_and_a_bar_with_no_room_reads_empty()
    {
        Assert.Equal(75.0, WarningRules.Percent(750, 1000), 6);
        Assert.Equal(0.0, WarningRules.Percent(10, 0));
    }

    [Fact]
    public void The_first_reading_of_a_trip_does_not_warn_even_past_a_percent()
    {
        var step = WarningRules.Next(WarningWatch.Start, 80, Default);

        Assert.Null(step.ReachedAt);
    }

    [Fact]
    public void Rising_to_a_percent_warns_for_it()
    {
        var watch = Reading(WarningWatch.Start, 49.9);

        var step = WarningRules.Next(watch, 50, Default);

        Assert.Equal(50, step.ReachedAt);
    }

    [Fact]
    public void Staying_below_the_next_percent_does_not_warn()
    {
        var watch = Reading(Reading(WarningWatch.Start, 10), 40);

        Assert.Null(WarningRules.Next(watch, 49.99, Default).ReachedAt);
    }

    [Fact]
    public void A_percent_warns_once_per_trip_even_after_a_drop_and_a_second_rise()
    {
        var watch = Reading(Reading(WarningWatch.Start, 40), 55);
        watch = Reading(watch, 30);

        Assert.Null(WarningRules.Next(watch, 60, Default).ReachedAt);
    }

    [Fact]
    public void One_jump_past_several_percents_warns_once_for_the_highest()
    {
        var watch = Reading(WarningWatch.Start, 40);

        var step = WarningRules.Next(watch, 95, Default);

        Assert.Equal(90, step.ReachedAt);
        Assert.Null(WarningRules.Next(step.Watch, 99, Default).ReachedAt);
    }

    [Fact]
    public void A_new_trip_warns_again_for_every_percent()
    {
        var oldTrip = Reading(Reading(WarningWatch.Start, 0), 95);
        var newTrip = Reading(WarningWatch.Start, 0);

        Assert.Null(WarningRules.Next(oldTrip, 96, Default).ReachedAt);
        Assert.Equal(50, WarningRules.Next(newTrip, 52, Default).ReachedAt);
    }

    [Fact]
    public void Reaching_the_pause_percent_calls_for_a_pause_once()
    {
        var levels = Default with { PauseAt = 75 };
        var watch = Reading(WarningWatch.Start, 70);

        var step = WarningRules.Next(watch, 76, levels);

        Assert.True(step.IsPauseDue);
        Assert.Equal(75, step.ReachedAt);
        Assert.False(WarningRules.Next(Reading(step.Watch, 60), 80, levels).IsPauseDue);
    }

    [Fact]
    public void A_pause_percent_not_in_the_list_still_warns_when_it_pauses()
    {
        var levels = new WarningLevels(new[] { 50 }, PauseAt: 80);
        var watch = Reading(WarningWatch.Start, 60);

        var step = WarningRules.Next(watch, 81, levels);

        Assert.True(step.IsPauseDue);
        Assert.Equal(80, step.ReachedAt);
    }

    [Fact]
    public void A_pause_percent_of_zero_never_pauses()
    {
        var watch = Reading(WarningWatch.Start, 0);

        Assert.False(WarningRules.Next(watch, 100, Default).IsPauseDue);
    }

    [Fact]
    public void A_due_pause_is_made_with_time_control_running_on_expeditions()
    {
        Assert.Equal(PauseOutcome.Pauses, Decide());
    }

    [Fact]
    public void No_pause_is_made_without_time_control_as_the_player_could_not_resume()
    {
        Assert.Equal(PauseOutcome.NeedsTimeControl, Decide(hasTimeControl: false));
    }

    [Fact]
    public void No_pause_is_made_when_time_control_leaves_expeditions_alone()
    {
        Assert.Equal(PauseOutcome.NeedsTimeControlOnExpeditions, Decide(isTimeControlOnExpeditions: false));
    }

    [Fact]
    public void No_pause_is_made_while_the_trip_is_ending()
    {
        Assert.Equal(PauseOutcome.TripEnding, Decide(isTripEnding: true));
    }

    [Fact]
    public void A_pause_already_standing_is_left_alone()
    {
        Assert.Equal(PauseOutcome.AlreadyPaused, Decide(isPaused: true));
    }

    [Fact]
    public void Nothing_is_paused_when_no_pause_is_due()
    {
        Assert.Equal(PauseOutcome.NotDue, Decide(isDue: false));
    }

    [Fact]
    public void A_pause_tells_the_player_how_to_carry_on_and_a_refused_one_says_why()
    {
        Assert.Contains("pause key", WarningText.Body(PauseOutcome.Pauses, isChinese: false));
        Assert.Contains("Time Control", WarningText.Body(PauseOutcome.NeedsTimeControl, isChinese: false));
        Assert.Contains("TimeControlOnExpeditions", WarningText.Body(PauseOutcome.NeedsTimeControlOnExpeditions, isChinese: true));
        Assert.Null(WarningText.Body(PauseOutcome.NotDue, isChinese: false));
    }

    [Fact]
    public void The_warning_title_names_the_percent_in_the_game_language()
    {
        Assert.Equal("Exposure at 75%", WarningText.Title(75, isChinese: false));
        Assert.Contains("75%", WarningText.Title(75, isChinese: true));
        Assert.NotEqual(WarningText.Title(75, isChinese: false), WarningText.Title(75, isChinese: true));
    }

    private static WarningWatch Reading(WarningWatch watch, double percent) => WarningRules.Next(watch, percent, Default).Watch;

    private static PauseOutcome Decide(bool isDue = true, bool hasTimeControl = true, bool isTimeControlOnExpeditions = true, bool isPaused = false, bool isTripEnding = false) =>
        WarningRules.DecidePause(isDue, hasTimeControl, isTimeControlOnExpeditions, isPaused, isTripEnding);
}
