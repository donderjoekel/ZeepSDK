using System;
using System.Globalization;
using UnityEngine;
using ZeepkistClient;

namespace ZeepSDK.Leaderboard.Pages;

internal abstract class OfficialGameLeaderboardTab : BaseMultiplayerLeaderboardTab
{
    /// <inheritdoc />
    protected sealed override void OnEnable()
    {
        ZeepkistNetwork.LeaderboardUpdated += OnLeaderboardUpdated;
        ZeepkistNetwork.PlayerResultsChanged += OnPlayerResultsChanged;

        MaxPages = (ZeepkistNetwork.PlayerList.Count - 1) / 16;
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
            MaxPages = (ZeepkistNetwork.PlayerList.Count - 1) / 16;
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
            MaxPages = (ZeepkistNetwork.PlayerList.Count - 1) / 16;
        }
        catch (Exception e)
        {
            Logger.LogError($"Unhandled exception in {nameof(OnLeaderboardUpdated)}: " + e);
        }
    }

    /// <inheritdoc />
    protected sealed override void OnDraw()
    {
        ZeepkistNetworkPlayer[] players;

        try
        {
            players = GetOrderedPlayers();
        }
        catch (Exception e)
        {
            Logger.LogError($"Unhandled exception in {nameof(GetOrderedPlayers)}: " + e);
            return;
        }

        try
        {
            for (int i = 0; i < Instance.leaderboard_tab_positions.Count; ++i)
            {
                int index = CurrentPage * 16 + i;
                if (index >= players.Length)
                    continue;

                ZeepkistNetworkPlayer player = players[index];
                GUI_OnlineLeaderboardPosition item = Instance.leaderboard_tab_positions[i];

                item.position.text = (index + 1).ToString(CultureInfo.InvariantCulture);

                string formattedTime = player.CurrentResult != null
                    ? player.CurrentResult.Time.GetFormattedTime()
                    : string.Empty;

                if (player.ChampionshipPoints.x > 0)
                {
                    Vector2Int championshipPoints = player.ChampionshipPoints;
                    item.pointsCurrent.text = I2.Loc.LocalizationManager.GetTranslation("Online/Leaderboard/Points")
                        .Replace(
                            "{[POINTS]}",
                            Mathf.Round(championshipPoints.x).ToString(CultureInfo.InvariantCulture));

                    if (championshipPoints.y != 0)
                    {
                        item.pointsWon.text =
                            "(+" + Mathf.Round(championshipPoints.y).ToString(CultureInfo.InvariantCulture) + ")";
                    }
                }

                if (player.isHost)
                {
                    item.DrawLeaderboard(
                        player.SteamID,
                        string.Format(
                            "<link=\"{0}\"><sprite=\"achievement 2\" name=\"host_client\"><#FFC980>{1}</color></link>",
                            player.SteamID,
                            Instance.Filter(
                                player.GetTaggedUsername().NoParse(),
                                Steam_TheAchiever.FilterPurpose.player)));
                }
                else
                {
                    item.DrawLeaderboard(
                        player.SteamID,
                        string.Format(
                            "<link=\"{0}\">{1}</link>",
                            player.SteamID,
                            Instance.Filter(
                                player.GetTaggedUsername().NoParse(),
                                Steam_TheAchiever.FilterPurpose.player)));
                }

                item.time.text = formattedTime;
                if (ShouldShowPosition(player))
                {
                    item.position.gameObject.SetActive(true);
                    item.position.color =
                        PlayerManager.Instance.GetColorFromPosition(index + 1);
                }
                else
                {
                    item.position.gameObject.SetActive(false);
                }
            }
        }
        catch (Exception e)
        {
            Logger.LogError($"Unhandled exception in {nameof(OnDraw)}: " + e);
        }
    }

    protected abstract ZeepkistNetworkPlayer[] GetOrderedPlayers();

    protected abstract bool ShouldShowPosition(ZeepkistNetworkPlayer player);
}
