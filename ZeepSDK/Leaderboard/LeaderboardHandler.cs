using System;
using System.Collections.Generic;
using BepInEx.Logging;
using ZeepkistClient;
using ZeepSDK.Leaderboard.Pages;
using ZeepSDK.Leaderboard.Patches;
using ZeepSDK.Utilities;

namespace ZeepSDK.Leaderboard;

internal class LeaderboardHandler : MonoBehaviourWithLogging
{
    private static readonly ManualLogSource logger = LoggerFactory.GetLogger(typeof(LeaderboardHandler));
    
    private readonly List<ILeaderboardTab> tabs = new();
    private int currentTabIndex;
    private ILeaderboardTab CurrentLeaderboardTab => tabs[currentTabIndex];

    private void Start()
    {
        tabs.Add(new RoundLeaderboardTab());
        tabs.Add(new ChampionshipLeaderboardTab());

        OnlineTabLeaderboardUI_OnOpen.OnOpen += OnOpen;
        OnlineTabLeaderboardUI_OnClose.OnClose += OnClose;
        OnlineTabLeaderboardUI_Update.Update += OnUpdate;
    }

    internal void AddTab(ILeaderboardTab tab)
    {
        tabs.Add(tab);
    }

    internal void InsertTab(int index, ILeaderboardTab tab)
    {
        tabs.Insert(index, tab);
    }

    internal void RemoveTab(ILeaderboardTab tab)
    {
        tabs.Remove(tab);
    }

    private void OnOpen(OnlineTabLeaderboardUI sender)
    {
        try
        {
            sender.PauseHandler.Pause();
            currentTabIndex = 0;
            CurrentLeaderboardTab.Enable(sender);
            CurrentLeaderboardTab.Draw();
        }
        catch (Exception e)
        {
            logger.LogError($"Unhandled exception in {nameof(OnOpen)}: " + e);
        }
    }

    private void OnClose(OnlineTabLeaderboardUI sender)
    {
        try
        {
            sender.PauseHandler.Unpause();
            CurrentLeaderboardTab.Disable();
        }
        catch (Exception e)
        {
            logger.LogError($"Unhandled exception in {nameof(OnClose)}: " + e);
        }
    }

    private void OnUpdate(OnlineTabLeaderboardUI sender)
    {
        try
        {
            if (!ZeepkistNetwork.IsConnected || ZeepkistNetwork.CurrentLobby == null)
            {
                return;
            }

            if (sender.LeaderboardAction.buttonDown || sender.EscapeAction.buttonDown)
            {
                sender.Close(true);
            }

            if (sender.SwitchAction.buttonDown)
            {
                CurrentLeaderboardTab.Disable();
                currentTabIndex = (currentTabIndex + 1) % tabs.Count;
                CurrentLeaderboardTab.Enable(sender);
                CurrentLeaderboardTab.Draw();
            }

            string timeNoMilliSeconds =
                (ZeepkistNetwork.CurrentLobby.RoundTime -
                 (ZeepkistNetwork.Time - ZeepkistNetwork.CurrentLobby.LevelLoadedAtTime)).GetFormattedTimeNoMilliSeconds();
            sender.timeLeftLeaderboard.text = ZeepkistNetwork.CurrentLobby.GameState == 0 ? timeNoMilliSeconds : "";

            if (sender.MenuLeftAction.buttonDown)
            {
                CurrentLeaderboardTab.GoToPreviousPage();
                CurrentLeaderboardTab.Draw();
            }

            if (sender.MenuRightAction.buttonDown)
            {
                CurrentLeaderboardTab.GoToNextPage();
                CurrentLeaderboardTab.Draw();
            }
        }
        catch (Exception e)
        {
            logger.LogError($"Unhandled exception in {nameof(OnUpdate)}: " + e);
        }
    }
}
