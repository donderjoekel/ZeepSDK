using BepInEx.Logging;
using JetBrains.Annotations;
using ZeepkistClient;

namespace ZeepSDK.Leaderboard.Pages;

/// <summary>
/// A base implementation that can be used for creating a custom tab for the leaderboard
/// </summary>
[PublicAPI]
public abstract class BaseLeaderboardTab : ILeaderboardTab
{
    /// <summary>
    /// A logger that can be used to log messages
    /// </summary>
    protected ManualLogSource Logger { get; private set; }

    /// <summary>
    /// The instance of the leaderboard UI
    /// </summary>
    protected OnlineTabLeaderboardUI Instance { get; private set; }

    /// <summary>
    /// The index of the current page
    /// </summary>
    protected int CurrentPage { get; private set; }

    /// <summary>
    /// The maximum amount of pages
    /// </summary>
    protected int MaxPages { get; set; }

    /// <summary>
    /// A boolean representing if the page is currently active
    /// </summary>
    protected bool IsActive { get; private set; }

    /// <summary>
    /// Constructor
    /// </summary>
    protected BaseLeaderboardTab()
    {
        Logger = Plugin.CreateLogger(GetType().Name);
    }

    /// <inheritdoc />
    public void Enable(OnlineTabLeaderboardUI sender)
    {
        IsActive = true;
        Instance = sender;

        Instance.playersLeaderboard.text = I2.Loc.LocalizationManager.GetTranslation("Online/Leaderboard/PlayerCount")
            .Replace("{[PLAYERS]}", ZeepkistNetwork.PlayerList.Count.ToString())
            .Replace("{[MAXPLAYERS]}", ZeepkistNetwork.CurrentLobby.MaxPlayerCount.ToString());

        Instance.leaderboardTitle.text = GetLeaderboardTitle();

        CurrentPage = 0;
        UpdatePageNumber();

        OnEnable();
    }

    /// <inheritdoc />
    public void Disable()
    {
        OnDisable();
        IsActive = false;
    }

    /// <inheritdoc />
    public void GoToPreviousPage()
    {
        if (!IsActive)
            return;

        CurrentPage = CurrentPage - 1 < 0 ? MaxPages : CurrentPage - 1;
        UpdatePageNumber();
    }

    /// <inheritdoc />
    public void GoToNextPage()
    {
        if (!IsActive)
            return;

        CurrentPage = CurrentPage + 1 > MaxPages ? 0 : CurrentPage + 1;
        UpdatePageNumber();
    }

    /// <inheritdoc />
    public void Draw()
    {
        if (!IsActive)
            return;

        ClearLeaderboard();
        OnDraw();
    }

    /// <summary>
    /// This can be called to update the page number that is visible in the UI
    /// </summary>
    protected void UpdatePageNumber()
    {
        Instance.Page.text = I2.Loc.LocalizationManager.GetTranslation("Online/Lobby/Page")
            .Replace("{[PAGE]}", (CurrentPage + 1).ToString() + "/" + (MaxPages + 1).ToString());
    }

    private void ClearLeaderboard()
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
