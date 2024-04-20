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
    private static readonly ManualLogSource _logger = LoggerFactory.GetLogger(typeof(MultiplayerApi));

    /// <summary>
    /// An event that gets fired whenever you connect to a game
    /// </summary>
    public static event EventHandler ConnectedToGame;

    /// <summary>
    /// An event that gets fired whenever you disconnect from a game
    /// </summary>
    public static event EventHandler DisconnectedFromGame;

    /// <summary>
    /// An event that gets fired whenever you have created a room/game
    /// </summary>
    public static event EventHandler CreatedRoom;

    /// <summary>
    /// An even that gets fired whenever you have joined a room/game
    /// </summary>
    public static event EventHandler JoinedRoom;

    /// <summary>
    /// An even that gets fired whenever a player joins the room/game
    /// </summary>
    public static event EventHandler<PlayerJoinedEventArgs> PlayerJoined;

    /// <summary>
    /// An even that gets fired whenever a player leaves the room/game
    /// </summary>
    public static event EventHandler<PlayerLeftEventArgs> PlayerLeft;

    /// <summary>
    /// Gets the level that is currently being played
    /// </summary>
    /// <returns>
    /// The level or null if the level cannot be found
    /// <br/><br/>
    /// The level can be null due to not being connected, not playing an online match, or issues with downloading the level from the workshop
    /// </returns>
    public static LevelScriptableObject CurrentLevel => !ZeepkistNetwork.IsConnected || !ZeepkistNetwork.IsConnectedToGame
                ? null
                : ZeepkistNetwork.CurrentLobby == null
                ? null
                : PlayerManager.Instance?.loader?.GlobalLevel;

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
    /// Adds a level to the playlist
    /// </summary>
    /// <param name="playlistItem">The item to add</param>
    /// <param name="setAsPlayNext">Should this item be the next one that will be played?</param>
    [PublicAPI]
    public static int AddLevelToPlaylist(PlaylistItem playlistItem, bool setAsPlayNext)
    {
        if (playlistItem == null)
        {
            _logger.LogError("AddLevelToPlaylist expects a non-null playlistItem");
            return -1;
        }

        try
        {
            if (ZeepkistNetwork.CurrentLobby == null)
            {
                return -1;
            }

            OnlineZeeplevel onlineZeepLevel = playlistItem.ToOnlineZeepLevel();
            ZeepkistNetwork.CurrentLobby.Playlist.Add(onlineZeepLevel);
            int index = ZeepkistNetwork.CurrentLobby.Playlist.Count - 1;

            if (setAsPlayNext)
            {
                ZeepkistNetwork.CurrentLobby.NextPlaylistIndex = index;
            }

            ZeepkistNetwork.NetworkClient?.SendPacket(new ChangeLobbyPlaylistPacket()
            {
                NewTime = ZeepkistNetwork.CurrentLobby.RoundTime,
                IsRandom = ZeepkistNetwork.CurrentLobby.PlaylistRandom,
                Playlist = ZeepkistNetwork.CurrentLobby.Playlist,
                CurrentIndex = ZeepkistNetwork.CurrentLobby.CurrentPlaylistIndex,
                NextIndex = ZeepkistNetwork.CurrentLobby.NextPlaylistIndex
            });

            return index;
        }
        catch (Exception e)
        {
            _logger.LogError($"Unhandled exception in {nameof(AddLevelToPlaylist)}: " + e);
            return -1;
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
            ZeepkistNetwork.NetworkClient?.SendPacket(new ChangeLobbyPlaylistPacket()
            {
                NewTime = ZeepkistNetwork.CurrentLobby.RoundTime,
                IsRandom = ZeepkistNetwork.CurrentLobby.PlaylistRandom,
                Playlist = ZeepkistNetwork.CurrentLobby.Playlist,
                CurrentIndex = ZeepkistNetwork.CurrentLobby.CurrentPlaylistIndex,
                NextIndex = ZeepkistNetwork.CurrentLobby.NextPlaylistIndex
            });
        }
        catch (Exception e)
        {
            _logger.LogError($"Unhandled exception in {nameof(SetNextLevelIndex)}: " + e);
        }
    }
}
