using System;
using ZeepkistClient;

namespace ZeepSDK.Multiplayer;

/// <summary>
/// The arguments for when a player joins the room/game
/// </summary>
public class PlayerJoinedEventArgs : EventArgs
{
    /// <inheritdoc />
    public PlayerJoinedEventArgs(ZeepkistNetworkPlayer player)
    {
        Player = player;
    }

    /// <summary>
    /// The player in question
    /// </summary>
    public ZeepkistNetworkPlayer Player
    {
        get;
    }
}
