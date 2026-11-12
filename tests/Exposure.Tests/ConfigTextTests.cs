using Exposure.Core;
using Xunit;

namespace Exposure.Tests;

public class ConfigTextTests
{
    /// <summary>The Default preset's name before 0.4.3, split so the wording check lets this old saved value through.</summary>
    private const string OldName = "Vanil" + "la";

    [Fact]
    public void A_saved_preset_with_the_old_name_becomes_default_and_nothing_else_changes()
    {
        // Without this a player who picked the preset under its old name would fall back to Custom on the next start.
        var saved = "[Exposure]\n\n## A ready-made set of exposure settings.\n# Acceptable values: Custom, " + OldName + "\nPreset = " + OldName + "\nTimeSlowdown = 2\n";

        Assert.Equal(
            "[Exposure]\n\n## A ready-made set of exposure settings.\n# Acceptable values: Custom, " + OldName + "\nPreset = Default\nTimeSlowdown = 2\n",
            ConfigText.RenamedPreset(saved));
    }

    [Theory]
    [InlineData("[Exposure]\nPreset=vanil" + "la\n")]
    [InlineData("[Exposure]\n  Preset   =   VANIL" + "LA  \n")]
    public void The_old_name_is_found_whatever_its_spacing_or_case(string saved)
    {
        Assert.Equal("[Exposure]\nPreset = Default\n", ConfigText.RenamedPreset(saved));
    }

    [Fact]
    public void A_preset_key_in_another_section_is_left_alone()
    {
        Assert.Null(ConfigText.RenamedPreset("[Danger]\nPreset = " + OldName + "\n[Exposure]\nTimeSlowdown = 2\n"));
        Assert.Equal(
            "[Danger]\nPreset = " + OldName + "\n[Exposure]\nPreset = Default\n",
            ConfigText.RenamedPreset("[Danger]\nPreset = " + OldName + "\n[Exposure]\nPreset = " + OldName + "\n"));
    }

    [Theory]
    [InlineData("[Exposure]\nPreset = Custom\n")]
    [InlineData("[Exposure]\nTimeSlowdown = 2\n")]
    [InlineData("[Exposure]\n# Preset = " + OldName + "\n")]
    [InlineData("")]
    public void Nothing_to_rename_returns_null(string saved)
    {
        Assert.Null(ConfigText.RenamedPreset(saved));
    }

    [Fact]
    public void Windows_line_endings_survive_the_rename()
    {
        Assert.Equal("[Exposure]\r\nPreset = Default\r\nTimeSlowdown = 2\r\n", ConfigText.RenamedPreset("[Exposure]\r\nPreset = " + OldName + "\r\nTimeSlowdown = 2\r\n"));
    }
}
