using JetBrains.Annotations;

namespace ZeepSDK.Leaderboard;

/// <summary>
/// Either <see cref="IMultiplayerLeaderboardTab"/> or <see cref="ISingleplayerLeaderboardTab"/> should be implemented
/// </summary>
[PublicAPI]
public interface ILeaderboardTab
{
    /// <summary>
    /// Called when the tab is enabled
    /// </summary>
    /// <param name="sender"></param>
    void Enable(OnlineTabLeaderboardUI sender);

    /// <summary>
    /// Called when the tab is disabled
    /// </summary>
    void Disable();

    /// <summary>
    /// Called when the user wants to go to the previous page
    /// </summary>
    void GoToPreviousPage();

    /// <summary>
    /// Called when the user wants to go to the next page
    /// </summary>
    void GoToNextPage();

    /// <summary>
    /// Called whenever the tab needs to (re)draw
    /// </summary>
    void Draw();
}
