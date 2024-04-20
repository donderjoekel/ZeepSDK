using System;
using JetBrains.Annotations;

namespace ZeepSDK.Communication;

/// <summary>
/// A receiver that can listen for messages from senders
/// </summary>
[PublicAPI]
public interface IComReceiver : IEquatable<IComReceiver>
{
    /// <summary>
    /// The identifier of this receiver
    /// </summary>
    Guid Identifier
    {
        get;
    }

    /// <summary>
    /// The identifier of the mod that is sending the message
    /// </summary>
    string ModIdentifier
    {
        get;
    }

    /// <summary>
    /// The event that is triggered when a message is received
    /// </summary>
    event EventHandler<MessageReceivedEventArgs> MessageReceived;

    /// <summary>
    /// The method that will be invoked to process, and in turn dispatch, the message
    /// </summary>
    /// <param name="message">The message to process</param>
    void ProcessMessage(string message);
}
