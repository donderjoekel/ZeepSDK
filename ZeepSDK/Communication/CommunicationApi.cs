using System;
using BepInEx;
using JetBrains.Annotations;

namespace ZeepSDK.Communication;

/// <summary>
/// An api that enables communication between mods.
/// </summary>
/// <remarks>
/// A mod creates a sender with <see cref="CreateSender"/> and sends messages on named topics. Other mods
/// create receivers with <see cref="CreateReceiver"/> for that mod's GUID, optionally filtered on a topic.
/// Receivers are invoked synchronously on the sending thread, and only receive messages sent after they
/// were created.
/// </remarks>
[PublicAPI]
public static class CommunicationApi
{
    private static readonly ComRegistry registry = new();

    /// <summary>
    /// Creates a sender that sends messages as the given plugin
    /// </summary>
    /// <param name="plugin">The plugin that will be sending messages</param>
    /// <returns>A <see cref="IComSender"/> for the given plugin</returns>
    public static IComSender CreateSender(BaseUnityPlugin plugin)
    {
        if (plugin == null)
            throw new ArgumentNullException(nameof(plugin));

        return new ComSender(registry, plugin.Info.Metadata.GUID);
    }

    /// <summary>
    /// Creates a receiver that listens to messages sent by the given mod. Dispose the receiver to stop listening.
    /// </summary>
    /// <param name="sourceModGuid">The <see cref="BepInPlugin.GUID">GUID</see> of the mod to listen to</param>
    /// <param name="topic">The topic to listen to, or null to receive messages on all topics</param>
    /// <returns>A <see cref="IComReceiver"/> for the given mod</returns>
    public static IComReceiver CreateReceiver(string sourceModGuid, string topic = null)
    {
        if (string.IsNullOrEmpty(sourceModGuid))
            throw new ArgumentException("Mod GUID cannot be null or empty", nameof(sourceModGuid));

        ComReceiver receiver = new(registry, sourceModGuid, topic);
        registry.Add(receiver);
        return receiver;
    }
}
