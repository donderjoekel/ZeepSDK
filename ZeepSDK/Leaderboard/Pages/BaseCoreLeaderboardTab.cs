using System;
using System.Globalization;
using BepInEx.Logging;
using JetBrains.Annotations;
using ZeepkistClient;
using ZeepSDK.Utilities;

namespace ZeepSDK.Leaderboard.Pages;

/// <summary>
/// A base implementation that can be used for creating a custom tab for the leaderboard
/// <remarks>This is a replacement for <see cref="BaseLeaderboardTab"/> to keep backward compatibility</remarks>
/// </summary>
[PublicAPI]
public abstract class BaseCoreLeaderboardTab : ILeaderboardTab
{
    /// <summary>
    /// A logger that can be used to log messages
    /// </summary>
    protected ManualLogSource Logger
    {
        get;
        private set;
    }

    /// <summary>
    /// The instance of the leaderboard UI
    /// </summary>
    protected OnlineTabLeaderboardUI Instance
    {
        get;
        private set;
    }

    /// <summary>
    /// The index of the current page
    /// </summary>
    protected int CurrentPage
    {
        get;
        private set;
    }

    /// <summary>
    /// The maximum amount of pages
    /// </summary>
    protected int MaxPages
    {
        get;
        set;
    }

    /// <summary>
    /// A boolean representing if the page is currently active
    /// </summary>
    protected bool IsActive
    {
        get;
        private set;
    }

    /// <summary>
    /// Constructor
    /// </summary>
    protected BaseCoreLeaderboardTab()
    {
        Logger = LoggerFactory.GetLogger(this);
    }

    /// <inheritdoc />
    public void Enable(OnlineTabLeaderboardUI sender)
    {
        IsActive = true;
        Instance = sender;

        ClearLeaderboard();

        try
        {
            if (ZeepkistNetwork.IsConnectedToGame)
            {
                Instance.playersLeaderboard.text = I2.Loc.LocalizationManager
                    .GetTranslation("Online/Leaderboard/PlayerCount")
                    .Replace("{[PLAYERS]}",
                        ZeepkistNetwork.PlayerList.Count.ToString(CultureInfo.InvariantCulture),
                        StringComparison.Ordinal)
                    .Replace("{[MAXPLAYERS]}",
                        ZeepkistNetwork.CurrentLobby.MaxPlayerCount.ToString(CultureInfo.InvariantCulture),
                        StringComparison.Ordinal);
            }
            else
            {
                Instance.playersLeaderboard.text = string.Empty;
            }

            Instance.leaderboardTitle.text = GetLeaderboardTitle();

            CurrentPage = 0;
            UpdatePageNumber();
        }
        catch (Exception e)
        {
            Logger.LogError($"Unhandled exception in {nameof(Enable)}: " + e);
        }

        try
        {
            OnEnable();
        }
        catch (Exception e)
        {
            Logger.LogError($"Unhandled exception in {nameof(OnEnable)}: " + e);
        }
    }

    /// <inheritdoc />
    public void Disable()
    {
        ClearLeaderboard();

        try
        {
            OnDisable();
        }
        catch (Exception e)
        {
            Logger.LogError($"Unhandled exception in {nameof(OnDisable)}: " + e);
        }

        IsActive = false;
    }

    /// <inheritdoc />
    public void GoToPreviousPage()
    {
        if (!IsActive)
        {
            return;
        }

        CurrentPage = CurrentPage - 1 < 0 ? MaxPages : CurrentPage - 1;
        UpdatePageNumber();
    }

    /// <inheritdoc />
    public void GoToNextPage()
    {
        if (!IsActive)
        {
            return;
        }

        CurrentPage = CurrentPage + 1 > MaxPages ? 0 : CurrentPage + 1;
        UpdatePageNumber();
    }

    /// <inheritdoc />
    public void Draw()
    {
        if (!IsActive)
        {
            return;
        }

        ClearLeaderboard();

        try
        {
            OnDraw();
        }
        catch (Exception e)
        {
            Logger.LogError($"Unhandled exception in {nameof(OnDraw)}: " + e);
        }
    }

    /// <summary>
    /// This can be called to update the page number that is visible in the UI
    /// </summary>
    protected void UpdatePageNumber()
    {
        try
        {
            Instance.Page.text = I2.Loc.LocalizationManager.GetTranslation("Online/Lobby/Page")
                .Replace("{[PAGE]}", CurrentPage + 1 + "/" + (MaxPages + 1), StringComparison.Ordinal);
        }
        catch (Exception e)
        {
            Logger.LogError($"Unhandled exception in {nameof(UpdatePageNumber)}: " + e);
        }
    }

    private void ClearLeaderboard()
    {
        try
        {
            foreach (GUI_OnlineLeaderboardPosition item in Instance.leaderboard_tab_positions)
            {
                item.DrawLeaderboard(0, "");
                item.time.text = "";
                item.position.gameObject.SetActive(false);
                item.pointsCurrent.text = "";
                item.pointsWon.text = "";
            }
        }
        catch (Exception e)
        {
            Logger.LogError($"Unhandled exception in {nameof(ClearLeaderboard)}: " + e);
        }
    }

    /// <summary>
    /// This should return the title of the leaderboard that gets displayed in the UI
    /// </summary>
    protected abstract string GetLeaderboardTitle();

    /// <summary>
    /// Called when the tab gets enabled
    /// </summary>
    protected abstract void OnEnable();

    /// <summary>
    /// Called when te tab gets disabled
    /// </summary>
    protected abstract void OnDisable();

    /// <summary>
    /// Called when the tab needs to (re)draw
    /// </summary>
    protected abstract void OnDraw();
}
