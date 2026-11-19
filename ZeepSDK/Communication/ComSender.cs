using System;
using Newtonsoft.Json;

namespace ZeepSDK.Communication;

internal sealed class ComSender : IComSender
{
    private readonly ComRegistry registry;

    public string ModGuid { get; }

    public ComSender(ComRegistry registry, string modGuid)
    {
        this.registry = registry;
        ModGuid = modGuid;
    }

    public void Send(string topic, string payload)
    {
        if (string.IsNullOrEmpty(topic))
            throw new ArgumentException("Topic cannot be null or empty", nameof(topic));

        ComMessage message = new(ModGuid, topic, payload);

        foreach (ComReceiver receiver in registry.GetReceivers(ModGuid))
        {
            receiver.Dispatch(message);
        }
    }

    public void SendJson<T>(string topic, T data)
    {
        Send(topic, JsonConvert.SerializeObject(data));
    }
}
