using ZeepSDK.Multiplayer.Patches;

namespace ZeepSDK.Multiplayer;

public static class MultiplayerApi
{
    public static event ConnectedToGameDelegate ConnectedToGame;
    public static event DisconnectedFromGameDelegate DisconnectedFromGame;
    public static event CreatedRoomDelegate CreatedRoom;
    public static event JoinedRoomDelegate JoinedRoom;
    public static event PlayerJoinedDelegate PlayerJoined;
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
