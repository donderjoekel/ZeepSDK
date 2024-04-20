using JetBrains.Annotations;

namespace ZeepSDK.Leaderboard.Pages;

/// <summary>
/// A base implementation that can be used for creating a custom multiplayer tab for the leaderboard
/// </summary>
[PublicAPI]
public abstract class BaseMultiplayerLeaderboardTab : BaseCoreLeaderboardTab, IMultiplayerLeaderboardTab
{
}
