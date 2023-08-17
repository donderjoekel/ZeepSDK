using ZeepSDK.Multiplayer.Patches;

namespace ZeepSDK.Multiplayer;

/// <summary>
/// An API for interacting with the multiplayer side of Zeepkist
/// </summary>
public static class MultiplayerApi
{
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

    internal static void Initialize()
    {
        PhotonZeepkist_OnConnectedToGame.ConnectedToGame += () => ConnectedToGame?.Invoke();
        PhotonZeepkist_OnDisconnectedFromGame.DisconnectedFromGame += () => DisconnectedFromGame?.Invoke();
        PhotonZeepkist_OnCreatedRoom.CreatedRoom += () => CreatedRoom?.Invoke();
        PhotonZeepkist_OnJoinedRoom.JoinedRoom += () => JoinedRoom?.Invoke();
        PhotonZeepkist_OnPlayerEnteredRoom.PlayerEnteredRoom += player => PlayerJoined?.Invoke(player);
        PhotonZeepkist_OnPlayerLeftRoom.PlayerLeftRoom += player => PlayerLeft?.Invoke(player);
    }
}
