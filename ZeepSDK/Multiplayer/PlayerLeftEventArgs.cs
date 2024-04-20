using System;
using ZeepkistClient;

namespace ZeepSDK.Multiplayer;

/// <summary>
/// The arguments for when a player leaves the room/game
/// </summary>
public class PlayerLeftEventArgs : EventArgs
{
    /// <inheritdoc />
    public PlayerLeftEventArgs(ZeepkistNetworkPlayer player)
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
