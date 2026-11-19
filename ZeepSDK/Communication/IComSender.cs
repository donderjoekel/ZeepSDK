using JetBrains.Annotations;

namespace ZeepSDK.Communication;

/// <summary>
/// Sends messages on behalf of a mod to every receiver listening to that mod
/// </summary>
/// <remarks>
/// Receivers are invoked synchronously on the calling thread. Exceptions thrown by receivers are
/// logged and never propagate to the sender.
/// </remarks>
[PublicAPI]
public interface IComSender
{
    /// <summary>
    /// The <see cref="BepInEx.BepInPlugin.GUID">GUID</see> of the mod this sender sends as
    /// </summary>
    string ModGuid { get; }

    /// <summary>
    /// Sends a message to all receivers listening to this mod
    /// </summary>
    /// <param name="topic">The topic of the message, used by receivers to distinguish kinds of messages</param>
    /// <param name="payload">The payload of the message</param>
    void Send(string topic, string payload);

    /// <summary>
    /// Serializes the given data to json and sends it to all receivers listening to this mod
    /// </summary>
    /// <param name="topic">The topic of the message, used by receivers to distinguish kinds of messages</param>
    /// <param name="data">The data to serialize</param>
    void SendJson<T>(string topic, T data);
}
