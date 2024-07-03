using System;
using BepInEx.Logging;
using JetBrains.Annotations;
using ZeepkistClient;
using ZeepkistNetworking;
using ZeepSDK.Extensions;
using ZeepSDK.Multiplayer.Patches;
using ZeepSDK.Utilities;

namespace ZeepSDK.Multiplayer;

/// <summary>
/// An API for interacting with the multiplayer side of Zeepkist
/// </summary>
[PublicAPI]
public static class MultiplayerApi
{
    private static readonly ManualLogSource logger = LoggerFactory.GetLogger(typeof(MultiplayerApi));

    /// <summary>
    /// An event that gets fired whenever you connect to a game
    /// </summary>
    public static event ConnectedToGameDelegate ConnectedToGame;

    /// <summary>
    /// An event that gets fired whenever you disconnect from a game
    /// </summary>
    public static event DisconnectedFromGameDelegate DisconnectedFromGame;

    /// <summary>
    /// An event that gets fired whenever you have created a room/game
    /// </summary>
    public static event CreatedRoomDelegate CreatedRoom;

    /// <summary>
    /// An even that gets fired whenever you have joined a room/game
    /// </summary>
    public static event JoinedRoomDelegate JoinedRoom;

    /// <summary>
    /// An even that gets fired whenever a player joins the room/game
    /// </summary>
    public static event PlayerJoinedDelegate PlayerJoined;

    /// <summary>
    /// An even that gets fired whenever a player leaves the room/game
    /// </summary>
    public static event PlayerLeftDelegate PlayerLeft;

    /// <summary>
    /// Is the player currently in an online game or not
    /// </summary>
    public static bool IsPlayingOnline => ZeepkistNetwork.IsConnectedToGame;

    internal static void Initialize()
    {
        PhotonZeepkist_OnConnectedToGame.ConnectedToGame += () => ConnectedToGame.InvokeSafe();
        PhotonZeepkist_OnDisconnectedFromGame.DisconnectedFromGame += () => DisconnectedFromGame.InvokeSafe();
        PhotonZeepkist_OnCreatedRoom.CreatedRoom += () => CreatedRoom.InvokeSafe();
        PhotonZeepkist_OnJoinedRoom.JoinedRoom += () => JoinedRoom.InvokeSafe();
        PhotonZeepkist_OnPlayerEnteredRoom.PlayerEnteredRoom += player => PlayerJoined.InvokeSafe(player);
        PhotonZeepkist_OnPlayerLeftRoom.PlayerLeftRoom += player => PlayerLeft.InvokeSafe(player);
    }

    /// <summary>
    /// Adds a level to the playlist. Once you're done adding levels, call <see cref="UpdateServerPlaylist"/> to update the server
    /// </summary>
    /// <param name="playlistItem">The item to add</param>
    /// <param name="setAsPlayNext">Should this item be the next one that will be played?</param>
    [PublicAPI]
    public static int AddLevelToPlaylist(PlaylistItem playlistItem, bool setAsPlayNext)
    {
        try
        {
            if (ZeepkistNetwork.CurrentLobby == null)
                return -1;

            OnlineZeeplevel onlineZeepLevel = playlistItem.ToOnlineZeepLevel();
            ZeepkistNetwork.CurrentLobby.Playlist.Add(onlineZeepLevel);
            int index = ZeepkistNetwork.CurrentLobby.Playlist.Count - 1;

            if (setAsPlayNext)
            {
                ZeepkistNetwork.CurrentLobby.NextPlaylistIndex = index;
            }

            return index;
        }
        catch (Exception e)
        {
            logger.LogError($"Unhandled exception in {nameof(AddLevelToPlaylist)}: " + e);
            return -1;
        }
    }

    /// <summary>
    /// Updates the playlist on the server
    /// </summary>
    [PublicAPI]
    public static void UpdateServerPlaylist()
    {
        try
        {
            ZeepkistNetwork.NetworkClient?.SendPacket(
                new ChangeLobbyPlaylistPacket()
                {
                    NewTime = ZeepkistNetwork.CurrentLobby.RoundTime,
                    IsRandom = ZeepkistNetwork.CurrentLobby.PlaylistRandom,
                    playlist_all = ZeepkistNetwork.CurrentLobby.Playlist,
                    CurrentIndex = ZeepkistNetwork.CurrentLobby.CurrentPlaylistIndex,
                    NextIndex = ZeepkistNetwork.CurrentLobby.NextPlaylistIndex
                });
        }
        catch (Exception e)
        {
            logger.LogError($"Unabled exception in {nameof(UpdateServerPlaylist)}: " + e);
        }
    }

    /// <summary>
    /// Sets the level that should be played next
    /// </summary>
    /// <param name="index">The (zero-based) index of the level</param>
    public static void SetNextLevelIndex(int index)
    {
        try
        {
            ZeepkistNetwork.CurrentLobby.NextPlaylistIndex = index;
            ZeepkistNetwork.NetworkClient?.SendPacket(
                new ChangeLobbyPlaylistPacket()
                {
                    NewTime = ZeepkistNetwork.CurrentLobby.RoundTime,
                    IsRandom = ZeepkistNetwork.CurrentLobby.PlaylistRandom,
                    playlist_all = ZeepkistNetwork.CurrentLobby.Playlist,
                    CurrentIndex = ZeepkistNetwork.CurrentLobby.CurrentPlaylistIndex,
                    NextIndex = ZeepkistNetwork.CurrentLobby.NextPlaylistIndex
                });
        }
        catch (Exception e)
        {
            logger.LogError($"Unhandled exception in {nameof(SetNextLevelIndex)}: " + e);
        }
    }

    /// <summary>
    /// Gets the level that is currently being played
    /// </summary>
    /// <returns>
    /// The level or null if the level cannot be found
    /// <br/><br/>
    /// The level can be null due to not being connected, not playing an online match, or issues with downloading the level from the workshop
    /// </returns>
    [Obsolete("Use LevelApi.CurrentLevel instead")]
    public static LevelScriptableObject GetCurrentLevel()
    {
        if (!ZeepkistNetwork.IsConnected || !ZeepkistNetwork.IsConnectedToGame)
            return null;

        if (ZeepkistNetwork.CurrentLobby == null)
            return null;

        if (PlayerManager.Instance == null)
            return null;

        if (PlayerManager.Instance.loader == null)
            return null;

        return PlayerManager.Instance.loader.GlobalLevel;
    }
}
