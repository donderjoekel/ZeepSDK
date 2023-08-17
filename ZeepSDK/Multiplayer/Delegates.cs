using JetBrains.Annotations;
using ZeepkistClient;

#pragma warning disable CS1591

namespace ZeepSDK.Multiplayer;

[PublicAPI]
public delegate void ConnectedToGameDelegate();

[PublicAPI]
public delegate void DisconnectedFromGameDelegate();

[PublicAPI]
public delegate void CreatedRoomDelegate();

[PublicAPI]
public delegate void JoinedRoomDelegate();

[PublicAPI]
public delegate void PlayerJoinedDelegate(ZeepkistNetworkPlayer player);

[PublicAPI]
public delegate void PlayerLeftDelegate(ZeepkistNetworkPlayer player);
