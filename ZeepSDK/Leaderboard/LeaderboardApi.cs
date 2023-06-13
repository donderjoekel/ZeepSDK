using JetBrains.Annotations;
using UnityEngine;

namespace ZeepSDK.Leaderboard;

/// <summary>
/// An API that allows you to add custom tabs to the leaderboard
/// </summary>
[PublicAPI]
public static class LeaderboardApi
{
    private static LeaderboardHandler leaderboardHandler;

    internal static void Initialize(GameObject gameObject)
    {
        leaderboardHandler = gameObject.AddComponent<LeaderboardHandler>();
    }

    /// <summary>
    /// Allows you to add a tab to the leaderboard
    /// </summary>
    /// <typeparam name="TTab">The type of the tab to create an instance of and add to the leaderboard</typeparam>
    /// <returns>The tab that was added to the leaderboard</returns>
    public static TTab AddTab<TTab>()
        where TTab : ILeaderboardTab, new()
    {
        TTab tab = new TTab();
        leaderboardHandler.AddTab(tab);
        return tab;
    }

    /// <summary>
    /// Allows you to add the given tab to the leaderboard
    /// </summary>
    /// <param name="tab">The tab to add</param>
    public static void AddTab(ILeaderboardTab tab)
    {
        leaderboardHandler.AddTab(tab);
    }

    /// <summary>
    /// Allows you to remove a tab from the leaderboard
    /// </summary>
    /// <param name="tab">The tab to remove</param>
    public static void RemoveTab(ILeaderboardTab tab)
    {
        leaderboardHandler.RemoveTab(tab);
    }
}
