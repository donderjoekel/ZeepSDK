using System;
using JetBrains.Annotations;

namespace ZeepSDK.Communication;

/// <summary>
/// The arguments for when a message is received
/// </summary>
[PublicAPI]
public class MessageReceivedEventArgs : EventArgs
{
    /// <inheritdoc />
    public MessageReceivedEventArgs(IComReceiver receiver, string message)
    {
        Receiver = receiver;
        Message = message;
    }

    /// <summary>
    /// The receiver that received the message
    /// </summary>
    public IComReceiver Receiver
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
