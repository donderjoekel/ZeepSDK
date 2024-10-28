namespace ZeepSDK.Leaderboard.Pages;

internal class ChampionshipLeaderboardTab : OfficialGameLeaderboardTab
{
    /// <inheritdoc />
    protected override string GetLeaderboardTitle()
    {
        return I2.Loc.LocalizationManager.GetTranslation("Online/Leaderboard/ChampionshipLB");
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        Instance.drawChampionShipLeaderboard = true;
    }
}
