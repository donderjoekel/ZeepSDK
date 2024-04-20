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
    private static readonly ManualLogSource _logger = LoggerFactory.GetLogger(typeof(LeaderboardHandler));

    private readonly List<IMultiplayerLeaderboardTab> _multiplayerTabs = [];
    private readonly List<ISingleplayerLeaderboardTab> _singleplayerTabs = [];
    private int _currentTabIndex;

    private int CurrentTabCount => ZeepkistNetwork.IsConnectedToGame ? _multiplayerTabs.Count : _singleplayerTabs.Count;

    private ILeaderboardTab CurrentLeaderboardTab => ZeepkistNetwork.IsConnectedToGame
        ? _multiplayerTabs[_currentTabIndex]
        : _singleplayerTabs[_currentTabIndex];

    private void Start()
    {
        AddTab(new RoundLeaderboardTab());
        AddTab(new ChampionshipLeaderboardTab());

        OnlineTabLeaderboardUI_OnOpen.OnOpen += OnOpen;
        OnlineTabLeaderboardUI_OnClose.OnClose += OnClose;
        OnlineTabLeaderboardUI_Update.Update += OnUpdate;
    }

    internal void AddTab(ILeaderboardTab tab)
    {
        if (tab is IMultiplayerLeaderboardTab multiplayerLeaderboardTab)
        {
            _multiplayerTabs.Add(multiplayerLeaderboardTab);
        }

        if (tab is ISingleplayerLeaderboardTab singleplayerLeaderboardTab)
        {
            _singleplayerTabs.Add(singleplayerLeaderboardTab);
        }
    }

    internal void InsertTab(int index, ILeaderboardTab tab)
    {
        if (tab is IMultiplayerLeaderboardTab multiplayerLeaderboardTab)
        {
            _multiplayerTabs.Insert(index, multiplayerLeaderboardTab);
        }

        if (tab is ISingleplayerLeaderboardTab singleplayerLeaderboardTab)
        {
            _singleplayerTabs.Insert(index, singleplayerLeaderboardTab);
        }
    }

    internal void RemoveTab(ILeaderboardTab tab)
    {
        _ = tab switch
        {
            IMultiplayerLeaderboardTab multiplayerLeaderboardTab => _multiplayerTabs.Remove(multiplayerLeaderboardTab),
            ISingleplayerLeaderboardTab singleplayerLeaderboardTab => _singleplayerTabs.Remove(singleplayerLeaderboardTab),
            _ => throw new ArgumentException("Tab must be either a multiplayer or singleplayer tab"),
        };
    }

    private void OnOpen(OnlineTabLeaderboardUI sender)
    {
        try
        {
            sender.PauseHandler.Pause();
            _currentTabIndex = 0;
            CurrentLeaderboardTab.Enable(sender);
            CurrentLeaderboardTab.Draw();
        }
        catch (Exception e)
        {
            _logger.LogError($"Unhandled exception in {nameof(OnOpen)}: " + e);
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
            _logger.LogError($"Unhandled exception in {nameof(OnClose)}: " + e);
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
                _currentTabIndex = (_currentTabIndex + 1) % CurrentTabCount;
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
            {
                return;
            }

            string timeNoMilliSeconds = (ZeepkistNetwork.CurrentLobby.RoundTime -
                                         (ZeepkistNetwork.Time - ZeepkistNetwork.CurrentLobby.LevelLoadedAtTime))
                .GetFormattedTimeNoMilliSeconds();
            sender.timeLeftLeaderboard.text = ZeepkistNetwork.CurrentLobby.GameState == 0 ? timeNoMilliSeconds : "";
        }
        catch (Exception e)
        {
            _logger.LogError($"Unhandled exception in {nameof(OnUpdate)}: " + e);
        }
    }
}
