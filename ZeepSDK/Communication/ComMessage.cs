using System;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace ZeepSDK.Communication;

/// <summary>
/// A message sent from one mod to any mods listening to it
/// </summary>
[PublicAPI]
public sealed class ComMessage
{
    /// <summary>
    /// The <see cref="BepInEx.BepInPlugin.GUID">GUID</see> of the mod that sent this message
    /// </summary>
    public string SourceModGuid { get; }

    /// <summary>
    /// The topic this message was sent on, used to distinguish different kinds of messages from the same mod
    /// </summary>
    public string Topic { get; }

    /// <summary>
    /// The raw payload of this message. Can be null.
    /// </summary>
    public string Payload { get; }

    internal ComMessage(string sourceModGuid, string topic, string payload)
    {
        SourceModGuid = sourceModGuid;
        Topic = topic;
        Payload = payload;
    }

    /// <summary>
    /// Deserializes the payload from json
    /// </summary>
    /// <typeparam name="T">The type to deserialize to</typeparam>
    /// <returns>The deserialized payload</returns>
    /// <exception cref="JsonException">The payload is not valid json for <typeparamref name="T"/></exception>
    public T ReadJson<T>()
    {
        return JsonConvert.DeserializeObject<T>(Payload ?? "null");
    }

    /// <summary>
    /// Tries to deserialize the payload from json
    /// </summary>
    /// <typeparam name="T">The type to deserialize to</typeparam>
    /// <param name="value">The deserialized payload, or default if deserialization failed</param>
    /// <returns>True if the payload was deserialized successfully</returns>
    public bool TryReadJson<T>(out T value)
    {
        try
        {
            value = ReadJson<T>();
            return true;
        }
        catch (Exception e) when (e is JsonException or ArgumentException or InvalidCastException)
        {
            value = default;
            return false;
        }
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"[{SourceModGuid}/{Topic}] {Payload}";
    }
}
