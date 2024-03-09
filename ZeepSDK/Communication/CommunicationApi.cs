using System;
using BepInEx;
using JetBrains.Annotations;

namespace ZeepSDK.Communication;

/// <summary>
/// The delegate for when a message is received
/// </summary>
public delegate void MessageReceivedDelegate(IComReceiver receiver, string message);

/// <summary>
/// An api that enables communication between mods
/// </summary>
[PublicAPI]
public static class CommunicationApi
{
    /// <summary>
    /// Creates a new communication sender based on the given plugin
    /// </summary>
    /// <param name="plugin">The plugin that will be sending messages</param>
    /// <returns>A <see cref="IComSender"/> for the given plugin</returns>
    public static IComSender CreateSender(BaseUnityPlugin plugin)
    {
        return new ComSender(plugin);
    }

    /// <summary>
    /// Creates a new communication receiver based on the given mod identifier
    /// </summary>
    /// <param name="modIdentifier">The <see cref="BepInPlugin.GUID">identifier</see> of the mod</param>
    /// <returns>A <see cref="IComReceiver"/> for the given identifier</returns>
    public static IComReceiver CreateReceiver(string modIdentifier)
    {
        ComReceiver comReceiver = new(Guid.NewGuid(), modIdentifier);
        ReceiverRepository.AddReceiver(comReceiver);
        return comReceiver;
    }

    /// <summary>
    /// Removes the given receiver from the available receivers, preventing it from receiving messages
    /// </summary>
    /// <param name="receiver">The receiver to remove</param>
    public static void RemoveReceiver(IComReceiver receiver)
    {
        ReceiverRepository.RemoveReceiver(receiver);
    }
}
