using System;
using JetBrains.Annotations;

namespace ZeepSDK.Communication;

/// <summary>
/// The delegate for when a message is received
/// </summary>
/// <param name="receiver">The receiver that received the message</param>
/// <param name="message">The received message</param>
public delegate void MessageReceivedDelegate(IComReceiver receiver, ComMessage message);

/// <summary>
/// Listens for messages sent by a specific mod. Dispose it to stop listening.
/// </summary>
/// <remarks>
/// Messages are delivered synchronously on the thread that sent them. Messages sent before
/// the receiver was created are not delivered.
/// </remarks>
[PublicAPI]
public interface IComReceiver : IDisposable
{
    /// <summary>
    /// The <see cref="BepInEx.BepInPlugin.GUID">GUID</see> of the mod this receiver listens to
    /// </summary>
    string SourceModGuid { get; }

    /// <summary>
    /// The topic this receiver is filtered on, or null if it receives messages on all topics
    /// </summary>
    string Topic { get; }

    /// <summary>
    /// Whether this receiver has been disposed and no longer receives messages
    /// </summary>
    bool IsDisposed { get; }

    /// <summary>
    /// Triggered when a message is received
    /// </summary>
    event MessageReceivedDelegate MessageReceived;
}
