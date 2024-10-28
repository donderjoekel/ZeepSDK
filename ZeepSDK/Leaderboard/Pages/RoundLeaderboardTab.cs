namespace ZeepSDK.Leaderboard.Pages;

internal class RoundLeaderboardTab : OfficialGameLeaderboardTab
{
    /// <inheritdoc />
    protected override string GetLeaderboardTitle()
    {
        return I2.Loc.LocalizationManager.GetTranslation("Online/Leaderboard/RoundLB");
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        Instance.drawChampionShipLeaderboard = false;
    }
}
