using System;

namespace ZeepSDK.Chat;

/// <summary>
/// The arguments for when a chat message is received
/// </summary>
public class ChatMessageReceivedEventArgs : EventArgs
{
    /// <summary>
    /// Creates a new instance of ChatMessageReceivedEventArgs
    /// </summary>
    /// <param name="playerId"></param>
    /// <param name="username"></param>
    /// <param name="message"></param>
    public ChatMessageReceivedEventArgs(ulong playerId, string username, string message)
    {
        PlayerId = playerId;
        Username = username;
        Message = message;
    }

    /// <summary>
    /// The steam id of the player who sent the message
    /// </summary>
    public ulong PlayerId
    {
        get;
    }

    /// <summary>
    /// The username of the player who sent the message
    /// </summary>
    public string Username
    {
        get;
    }

    /// <summary>
    /// The contents of the message
    /// </summary>
    public string Message
    {
        get;
    }
}
