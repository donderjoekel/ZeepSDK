using System;
using BepInEx.Logging;
using JetBrains.Annotations;
using UnityEngine;
using ZeepSDK.Utilities;

namespace ZeepSDK.Leaderboard;

/// <summary>
/// An API that allows you to add custom tabs to the leaderboard
/// </summary>
[PublicAPI]
public static class LeaderboardApi
{
    private static LeaderboardHandler leaderboardHandler;
    private static readonly ManualLogSource logger = LoggerFactory.GetLogger(typeof(LeaderboardApi));

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
        try
        {
            TTab tab = new TTab();
            leaderboardHandler.AddTab(tab);
            return tab;
        }
        catch (Exception e)
        {
            logger.LogError($"Unhandled exception in {nameof(AddTab)}: " + e);
            return default;
        }
    }

    /// <summary>
    /// Allows you to add the given tab to the leaderboard
    /// </summary>
    /// <param name="tab">The tab to add</param>
    public static void AddTab(ILeaderboardTab tab)
    {
        try
        {
            leaderboardHandler.AddTab(tab);
        }
        catch (Exception e)
        {
            logger.LogError($"Unhandled exception in {nameof(AddTab)}: " + e);
        }
    }

    /// <summary>
    /// Allows you to insert a tab into the leaderboard at the given index
    /// </summary>
    /// <param name="index">The position where the tab should be inserted</param>
    /// <typeparam name="TTab">The type of the tab to create an instance of and insert to the leaderboard</typeparam>
    /// <returns>The tab that was inserted in to the leaderboard</returns>
    public static TTab InsertTab<TTab>(int index)
        where TTab : ILeaderboardTab, new()
    {
        try
        {
            TTab tab = new TTab();
            leaderboardHandler.InsertTab(index, tab);
            return tab;
        }
        catch (Exception e)
        {
            logger.LogError($"Unhandled exception in {nameof(InsertTab)}: " + e);
            return default;
        }
    }

    /// <summary>
    /// Allows you to insert the given tab into the leaderboard at the given index
    /// </summary>
    /// <param name="index">The position where the tab should be inserted</param>
    /// <param name="tab">The tab to insert</param>
    public static void InsertTab(int index, ILeaderboardTab tab)
    {
        try
        {
            leaderboardHandler.InsertTab(index, tab);
        }
        catch (Exception e)
        {
            logger.LogError($"Unhandled exception in {nameof(InsertTab)}: " + e);
        }
    }

    /// <summary>
    /// Allows you to remove a tab from the leaderboard
    /// </summary>
    /// <param name="tab">The tab to remove</param>
    public static void RemoveTab(ILeaderboardTab tab)
    {
        try
        {
            leaderboardHandler.RemoveTab(tab);
        }
        catch (Exception e)
        {
            logger.LogError($"Unhandled exception in {nameof(RemoveTab)}: " + e);
        }
    }
}
