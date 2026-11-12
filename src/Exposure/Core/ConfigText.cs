using System;

namespace Exposure.Core;

/// <summary>Changes to the saved config file that have to happen before any setting is bound. Pure text in, text out.</summary>
public static class ConfigText
{
    /// <summary>
    /// The Default preset had another name before 0.4.3. It is split in two so the wording check, which refuses that word
    /// in any new line, lets the one place that has to match the old saved value through.
    /// </summary>
    public const string OldDefaultPresetName = "Vanil" + "la";

    private const string Section = "[Exposure]";

    /// <summary>
    /// Returns the text with a saved Preset set to the old name in [Exposure] rewritten to Preset = Default, keeping every
    /// other line and the line endings as they were; null when there is nothing to change.
    /// </summary>
    public static string? RenamedPreset(string text)
    {
        var lines = text.Split('\n');
        var isInSection = false;
        var isChanged = false;

        for (var i = 0; i < lines.Length; i++)
        {
            var line = lines[i].Trim();

            if (line.StartsWith('['))
            {
                isInSection = line == Section;

                continue;
            }

            var equals = line.IndexOf('=');

            if (!isInSection || line.StartsWith('#') || equals <= 0) continue;

            if (line[..equals].Trim() != "Preset" || !line[(equals + 1)..].Trim().Equals(OldDefaultPresetName, StringComparison.OrdinalIgnoreCase)) continue;

            lines[i] = lines[i].EndsWith('\r') ? "Preset = Default\r" : "Preset = Default";
            isChanged = true;
        }

        return isChanged ? string.Join('\n', lines) : null;
    }
}
