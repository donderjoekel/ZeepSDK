using JetBrains.Annotations;
using UnityEngine;

namespace ZeepSDK.Leaderboard;

[PublicAPI]
public static class LeaderboardApi
{
    private static LeaderboardHandler leaderboardHandler;

    internal static void Initialize(GameObject gameObject)
    {
        leaderboardHandler = gameObject.AddComponent<LeaderboardHandler>();
    }

    public static void AddTab<TTab>()
        where TTab : ILeaderboardTab, new()
    {
        leaderboardHandler.AddTab(new TTab());
    }

    public static void AddTab(ILeaderboardTab tab)
    {
        leaderboardHandler.AddTab(tab);
    }
}
