using System;

namespace ZeepSDK.Leaderboard.Pages;

/// <summary>
/// [Obsolete] A base implementation that can be used for creating a custom tab for the leaderboard
/// </summary>
[Obsolete(
    "This class is obsolete and will be removed in a future version. Use BaseSingleplayerLeaderboardTab or BaseMultiplayerLeaderboardTab instead.")]
public abstract class BaseLeaderboardTab : BaseMultiplayerLeaderboardTab
{
}
