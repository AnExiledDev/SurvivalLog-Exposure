using System;

namespace Exposure.Core;

/// <summary>What a warning says to the player, in English or Simplified Chinese, and what it writes to the log.</summary>
public static class WarningText
{
    public const string Source = "Exposure";

    public static string Title(int percent, bool isChinese) =>
        isChinese ? $"暴露值已达 {percent}%" : $"Exposure at {percent}%";

    /// <summary>Only a pause, or a pause that could not be made, has more to say than the title.</summary>
    public static string? Body(PauseOutcome outcome, bool isChinese) => outcome switch
    {
        PauseOutcome.Pauses => isChinese
            ? "游戏已暂停。点击速度按钮或按暂停键继续。"
            : "Game paused. Press a speed button or the pause key to carry on.",
        PauseOutcome.NeedsTimeControl => isChinese
            ? "未暂停：探索时暂停需要安装 Time Control。"
            : "Not paused: pausing on expeditions needs Time Control.",
        PauseOutcome.NeedsTimeControlOnExpeditions => isChinese
            ? "未暂停：请在 Time Control 中开启 TimeControlOnExpeditions。"
            : "Not paused: turn on TimeControlOnExpeditions in Time Control.",
        PauseOutcome.NotDue or PauseOutcome.AlreadyPaused or PauseOutcome.TripEnding => null,
        _ => throw new ArgumentOutOfRangeException(nameof(outcome), outcome, "a pause outcome with no text"),
    };

    public static string LogLine(int percent, double exposure, double maxExposure, PauseOutcome outcome) =>
        FormattableString.Invariant($"exposure reached {percent}% of the bar ({exposure:0.#} of {maxExposure:0.#}){LogPause(outcome)}");

    private static string LogPause(PauseOutcome outcome) => outcome switch
    {
        PauseOutcome.NotDue => "",
        PauseOutcome.Pauses => ", paused the game",
        PauseOutcome.AlreadyPaused => ", the game was already paused",
        PauseOutcome.TripEnding => ", not paused as the trip is ending",
        PauseOutcome.NeedsTimeControl => ", not paused: Time Control is not installed, and without it the game cannot be resumed on an expedition",
        PauseOutcome.NeedsTimeControlOnExpeditions => ", not paused: Time Control's TimeControlOnExpeditions is off, so it could not be resumed",
        _ => throw new ArgumentOutOfRangeException(nameof(outcome), outcome, "a pause outcome with no log text"),
    };
}
