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

    private readonly List<IMultiplayerLeaderboardTab> multiplayerTabs = new();
    private readonly List<ISingleplayerLeaderboardTab> singleplayerTabs = new();
    private int currentTabIndex;

    private int CurrentTabCount => ZeepkistNetwork.IsConnectedToGame ? multiplayerTabs.Count : singleplayerTabs.Count;

    private ILeaderboardTab CurrentLeaderboardTab => ZeepkistNetwork.IsConnectedToGame
        ? multiplayerTabs[currentTabIndex]
        : singleplayerTabs[currentTabIndex];

    private void Start()
    {
        InsertTab(0, new ChampionshipLeaderboardTab());
        InsertTab(0, new RoundLeaderboardTab());

        OnlineTabLeaderboardUI_OnOpen.OnOpen += OnOpen;
        OnlineTabLeaderboardUI_OnClose.OnClose += OnClose;
        OnlineTabLeaderboardUI_Update.Update += OnUpdate;
    }

    internal void AddTab(ILeaderboardTab tab)
    {
        if (tab is IMultiplayerLeaderboardTab multiplayerLeaderboardTab)
            multiplayerTabs.Add(multiplayerLeaderboardTab);

        if (tab is ISingleplayerLeaderboardTab singleplayerLeaderboardTab)
            singleplayerTabs.Add(singleplayerLeaderboardTab);
    }

    internal void InsertTab(int index, ILeaderboardTab tab)
    {
        if (tab is IMultiplayerLeaderboardTab multiplayerLeaderboardTab)
            multiplayerTabs.Insert(index, multiplayerLeaderboardTab);

        if (tab is ISingleplayerLeaderboardTab singleplayerLeaderboardTab)
            singleplayerTabs.Insert(index, singleplayerLeaderboardTab);
    }

    internal void RemoveTab(ILeaderboardTab tab)
    {
        switch (tab)
        {
            case IMultiplayerLeaderboardTab multiplayerLeaderboardTab:
                multiplayerTabs.Remove(multiplayerLeaderboardTab);
                break;
            case ISingleplayerLeaderboardTab singleplayerLeaderboardTab:
                singleplayerTabs.Remove(singleplayerLeaderboardTab);
                break;
            default:
                throw new ArgumentException("Tab must be either a multiplayer or singleplayer tab");
        }
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
            if (sender.LeaderboardAction.buttonDown || sender.EscapeAction.buttonDown)
            {
                sender.Close(true);
            }

            if (sender.SwitchAction.buttonDown)
            {
                CurrentLeaderboardTab.Disable();
                currentTabIndex = (currentTabIndex + 1) % CurrentTabCount;
                CurrentLeaderboardTab.Enable(sender);
                CurrentLeaderboardTab.Draw();
            }

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

            if (!ZeepkistNetwork.IsConnectedToGame)
                return;

            string timeNoMilliSeconds = (ZeepkistNetwork.CurrentLobby.RoundTime -
                                         (ZeepkistNetwork.Time - ZeepkistNetwork.CurrentLobby.LevelLoadedAtTime))
                .GetFormattedTimeNoMilliSeconds();
            sender.timeLeftLeaderboard.text = ZeepkistNetwork.CurrentLobby.GameState == 0 ? timeNoMilliSeconds : "";
        }
        catch (Exception e)
        {
            logger.LogError($"Unhandled exception in {nameof(OnUpdate)}: " + e);
        }
    }
}
