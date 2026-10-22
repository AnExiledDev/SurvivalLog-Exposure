using BepInEx.Unity.IL2CPP;
using Exposure.Core;
using GameCore.HotUpdate;
using GameCore.HotUpdate.Battle.Logic;

namespace Exposure.Adapters;

/// <summary>The game's side of a warning: the bar's fill, its exposure alarm, its pause, and Time Control's setting.</summary>
internal static class WarningsGame
{
    public const string TimeControlGuid = "com.dev.survivallog.timecontrol";

    /// <summary>Time Control's pause, the speed level the game's own pause key sends.</summary>
    private const int PausedLevel = 0;

    public static (double Exposure, double Max) ReadBar(ExploreManager manager) =>
        (manager._CurExposure_k__BackingField, manager.MaxExposure);

    /// <summary>The roar the game plays itself as the exposure alarm sounds.</summary>
    public static void PlayAlarm(ExploreManager manager) => manager.PlayExposureRoar();

    /// <summary>
    /// Asked only once a pause is due, so the game is not read on every check. A world with no time manager left is
    /// on its way out, and counts as a trip that is ending.
    /// </summary>
    public static PauseOutcome Pause(ExploreManager manager)
    {
        var time = LiveTime();
        var outcome = WarningRules.DecidePause(
            isDue: true,
            hasTimeControl: IL2CPPChainloader.Instance.Plugins.ContainsKey(TimeControlGuid),
            isTimeControlOnExpeditions: IsTimeControlOnExpeditions(),
            isPaused: time?.IsPausedScale() == true,
            isTripEnding: manager._hasEndProcessed || time == null);

        if (outcome == PauseOutcome.Pauses) time?.ChangeGameSpeed(PausedLevel);

        return outcome;
    }

    /// <summary>The live world's time manager, read only once the game has made the world.</summary>
    private static GameTimeManager? LiveTime() =>
        BaseSingleton<BattleLogicWorld>.IsInstanceCreated ? BaseSingleton<BattleLogicWorld>.Instance?._GameTimeManager : null;

    /// <summary>Time Control's own setting; a version without it does not control expeditions.</summary>
    private static bool IsTimeControlOnExpeditions() =>
        IL2CPPChainloader.Instance.Plugins.TryGetValue(TimeControlGuid, out var info) &&
        info.Instance is BasePlugin timeControl &&
        timeControl.Config.TryGetEntry<bool>("Expedition", "TimeControlOnExpeditions", out var entry) &&
        entry.Value;
}
