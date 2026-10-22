namespace Exposure.Core;

/// <summary>
/// Which kind of one-off add a game id stands for, from the game's tables (Config_Effect, Config_Action,
/// Config_RewardSet and Config_FurnitureFunc). A one-off add comes either from an action's effect
/// (AE_AddExploreByConfig) or from a reward set, and furniture rewards run their reward set for the action the
/// character did. An id these tables do not name counts as <see cref="ExposureSource.Other"/>.
/// </summary>
public static class ExposureSources
{
    public static ExposureSource ForEffect(int effectId) => effectId switch
    {
        1101 => ExposureSource.Loot,
        1102 or 1103 or 1501 or 1502 => ExposureSource.Door,
        1104 or >= 1503 and <= 1506 or >= 1601 and <= 1606 or 1932 => ExposureSource.Search,
        _ => ExposureSource.Other,
    };

    public static ExposureSource ForAction(int actionId) => actionId switch
    {
        1101 or 1106 or 1936 => ExposureSource.Loot,
        199 or 1102 or 1103 or 1501 or 1502 or >= 1926 and <= 1930 or 1943 or 1945 => ExposureSource.Door,
        1100 or 1104 or 1107 or >= 1503 and <= 1506 or >= 1601 and <= 1606 or 1925 or 1932 => ExposureSource.Search,
        _ => ExposureSource.Other,
    };

    /// <param name="furnitureActionId">The action whose furniture reward runs this set; null outside one, where only events and stories run reward sets.</param>
    public static ExposureSource ForRewardSet(int rewardSetId, int? furnitureActionId) => rewardSetId switch
    {
        9101 or 9102 => ExposureSource.Search,
        9103 or 9104 or 9106 => ExposureSource.Door,
        >= 9211 and <= 9277 and not 9216 => ExposureSource.Door,
        _ => furnitureActionId is { } actionId ? ForAction(actionId) : ExposureSource.Event,
    };
}
