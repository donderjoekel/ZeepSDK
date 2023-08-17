using ZeepkistClient;

namespace ZeepSDK.Multiplayer;

/// <summary>
/// 
/// </summary>
public delegate void ConnectedToGameDelegate();

/// <summary>
/// 
/// </summary>
public delegate void DisconnectedFromGameDelegate();

/// <summary>
/// 
/// </summary>
public delegate void CreatedRoomDelegate();

/// <summary>
/// 
/// </summary>
public delegate void JoinedRoomDelegate();

/// <summary>
/// 
/// </summary>
public delegate void PlayerJoinedDelegate(ZeepkistNetworkPlayer player);

/// <summary>
/// 
/// </summary>
public delegate void PlayerLeftDelegate(ZeepkistNetworkPlayer player);
