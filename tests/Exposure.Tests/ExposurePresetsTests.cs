using Exposure.Core;
using Xunit;

namespace Exposure.Tests;

public class ExposurePresetsTests
{
    /// <summary>The settings exactly as a fresh config writes them.</summary>
    private static readonly ExposureChoices Defaults = new(
        "Custom", 2, false, 2, 0, 0, 0, 0, false, 1, 1, 30, "");

    /// <summary>A Custom set with every per-source and danger setting moved away from its default.</summary>
    private static readonly ExposureChoices Tuned = Defaults with
    {
        SearchSlowdown = 4,
        PickupSlowdown = 5,
        BreakInSlowdown = 10,
        EventSlowdown = 20,
        DangerStart = 0.5,
        DangerEnd = 3,
        RampDays = 10,
        Locations = "1005=2",
    };

    [Fact]
    public void The_default_settings_slow_exposure_as_before_on_every_day_and_location()
    {
        var settings = ExposurePresets.Resolve(Defaults).Settings;

        Assert.Equal(1f, settings.TimeRate(2f, settings.Danger.Multiplier(1, 1000)));
        Assert.Equal(1f, settings.TimeRate(2f, settings.Danger.Multiplier(60, 1005)));
        Assert.Equal(0f, settings.RunRate(10f, settings.Danger.Multiplier(60, 1005)));

        foreach (var source in new[] { ExposureSource.Search, ExposureSource.Loot, ExposureSource.Door, ExposureSource.Event, ExposureSource.Other })
            Assert.Equal(5, settings.OneOff(source, 10, 0, settings.Danger.Multiplier(60, 1005)).Value);
    }

    [Fact]
    public void Custom_uses_the_players_own_settings()
    {
        var resolved = ExposurePresets.Resolve(Tuned);
        var settings = resolved.Settings;

        Assert.Equal(ExposurePreset.Custom, resolved.Preset);
        Assert.Equal(4, settings.OneOffSlowdown(ExposureSource.Search));
        Assert.Equal(5, settings.OneOffSlowdown(ExposureSource.Loot));
        Assert.Equal(10, settings.OneOffSlowdown(ExposureSource.Door));
        Assert.Equal(20, settings.OneOffSlowdown(ExposureSource.Event));
        Assert.Equal(6, settings.Danger.Multiplier(10, 1005), 6);
    }

    [Fact]
    public void A_per_source_slowdown_of_zero_or_less_follows_the_loot_slowdown()
    {
        var settings = ExposurePresets.Resolve(Defaults with { LootSlowdown = 3, SearchSlowdown = -1, EventSlowdown = double.NaN }).Settings;

        Assert.Equal(3, settings.OneOffSlowdown(ExposureSource.Search));
        Assert.Equal(3, settings.OneOffSlowdown(ExposureSource.Event));
    }

    [Fact]
    public void Custom_keeps_slowdowns_at_the_game_rate_or_slower_without_the_opt_in()
    {
        var settings = ExposurePresets.Resolve(Defaults with { TimeSlowdown = 0.5, LootSlowdown = 0.25, SearchSlowdown = 0.5 }).Settings;

        Assert.Equal(1, settings.TimeSlowdown);
        Assert.Equal(1, settings.LootSlowdown);
        Assert.Equal(1, settings.OneOffSlowdown(ExposureSource.Search));
    }

    [Fact]
    public void Custom_with_the_opt_in_goes_down_to_a_quarter()
    {
        var settings = ExposurePresets.Resolve(Defaults with { TimeSlowdown = 0.5, LootSlowdown = 0.1, IsFasterThanGameAllowed = true }).Settings;

        Assert.Equal(0.5, settings.TimeSlowdown);
        Assert.Equal(0.25, settings.LootSlowdown);
    }

    [Fact]
    public void A_ramp_of_zero_days_or_less_reads_as_one_day()
    {
        var settings = ExposurePresets.Resolve(Defaults with { DangerEnd = 2, RampDays = -5 }).Settings;

        Assert.Equal(2, settings.Danger.Multiplier(1, 1000));
    }

    [Fact]
    public void Vanilla_is_the_games_own_exposure_whatever_else_is_set()
    {
        var settings = ExposurePresets.Resolve(Tuned with { Preset = "Vanilla" }).Settings;
        var danger = settings.Danger.Multiplier(40, 1005);

        Assert.Equal(2f, settings.TimeRate(2f, danger));
        Assert.Equal(10f, settings.RunRate(10f, danger));

        foreach (var source in new[] { ExposureSource.Search, ExposureSource.Loot, ExposureSource.Door, ExposureSource.Event, ExposureSource.Other })
            Assert.Equal(7, settings.OneOff(source, 7, 0, danger).Value);
    }

    [Fact]
    public void Relaxed_matches_the_default_custom_settings()
    {
        var relaxed = ExposurePresets.Resolve(Tuned with { Preset = "Relaxed" }).Settings;
        var defaults = ExposurePresets.Resolve(Defaults).Settings;

        Assert.Equal(defaults.TimeRate(2f, 1), relaxed.TimeRate(2f, relaxed.Danger.Multiplier(40, 1005)));
        Assert.Equal(defaults.RunRate(10f, 1), relaxed.RunRate(10f, 1));
        Assert.Equal(defaults.OneOff(ExposureSource.Search, 10, 0, 1), relaxed.OneOff(ExposureSource.Search, 10, 0, 1));
    }

    [Fact]
    public void Tourist_is_much_slower_than_relaxed_and_running_adds_nothing()
    {
        var tourist = ExposurePresets.Resolve(Defaults with { Preset = "Tourist" }).Settings;
        var relaxed = ExposurePresets.Resolve(Defaults with { Preset = "Relaxed" }).Settings;

        Assert.True(tourist.TimeRate(2f, 1) < relaxed.TimeRate(2f, 1));
        Assert.True(tourist.OneOff(ExposureSource.Loot, 100, 0, 1).Value < relaxed.OneOff(ExposureSource.Loot, 100, 0, 1).Value);
        Assert.Equal(0f, tourist.RunRate(10f, 1));
    }

    [Fact]
    public void Hardcore_is_faster_than_the_game_without_the_separate_opt_in()
    {
        var settings = ExposurePresets.Resolve(Defaults with { Preset = "Hardcore", IsFasterThanGameAllowed = false }).Settings;

        Assert.True(settings.TimeRate(2f, 1) > 2f);
        Assert.True(settings.OneOff(ExposureSource.Loot, 100, 0, 1).Value > 100);
        Assert.Equal(10f, settings.RunRate(10f, 1));
    }

    [Fact]
    public void Preset_names_are_read_whatever_their_case_and_spacing()
    {
        Assert.Equal(ExposurePreset.Tourist, ExposurePresets.Parse(" tourist "));
        Assert.Equal(ExposurePreset.Hardcore, ExposurePresets.Parse("HARDCORE"));
    }

    [Fact]
    public void An_unknown_preset_uses_the_players_own_settings()
    {
        Assert.Equal(ExposurePreset.Custom, ExposurePresets.Parse("Nightmare"));
        Assert.Equal(ExposurePreset.Custom, ExposurePresets.Parse(null));
        Assert.Equal(4, ExposurePresets.Resolve(Tuned with { Preset = "Nightmare" }).Settings.OneOffSlowdown(ExposureSource.Search));
    }

    [Fact]
    public void Unreadable_locations_are_reported_even_under_a_preset()
    {
        Assert.Equal(new[] { "school=2" }, ExposurePresets.Resolve(Defaults with { Preset = "Vanilla", Locations = "school=2" }).IgnoredLocations);
    }
}
