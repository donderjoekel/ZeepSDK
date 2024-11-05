using System;
using ZeepkistClient;

namespace ZeepSDK.Leaderboard.Pages;

internal abstract class OfficialGameLeaderboardTab : BaseMultiplayerLeaderboardTab
{
    /// <inheritdoc />
    protected override void OnEnable()
    {
        ZeepkistNetwork.LeaderboardUpdated += OnLeaderboardUpdated;
        ZeepkistNetwork.PlayerResultsChanged += OnPlayerResultsChanged;

        SetMaxPages((ZeepkistNetwork.PlayerList.Count - 1) / 16);
    }

    /// <inheritdoc />
    protected sealed override void OnDisable()
    {
        ZeepkistNetwork.LeaderboardUpdated -= OnLeaderboardUpdated;
        ZeepkistNetwork.PlayerResultsChanged -= OnPlayerResultsChanged;
    }

    private void OnPlayerResultsChanged(ZeepkistNetworkPlayer obj)
    {
        try
        {
            SetMaxPages((ZeepkistNetwork.PlayerList.Count - 1) / 16);
        }
        catch (Exception e)
        {
            Logger.LogError($"Unhandled exception in {nameof(OnPlayerResultsChanged)}: " + e);
        }
    }

    private void OnLeaderboardUpdated()
    {
        try
        {
            SetMaxPages((ZeepkistNetwork.PlayerList.Count - 1) / 16);
        }
        catch (Exception e)
        {
            Logger.LogError($"Unhandled exception in {nameof(OnLeaderboardUpdated)}: " + e);
        }
    }

    /// <inheritdoc />
    protected sealed override void OnDraw()
    {
        Logger.LogInfo("Drawing leaderboard -> forwarding to Instance.DrawTabLeaderboard()");
        Instance.currentPage = CurrentPage;
        Instance.DrawTabLeaderboard();
    }

    private void SetMaxPages(int value)
    {
        MaxPages = value;
        Instance.maxPages = MaxPages;
    }
}
