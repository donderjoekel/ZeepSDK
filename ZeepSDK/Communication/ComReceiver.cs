using System;
using BepInEx.Logging;
using ZeepSDK.Utilities;

namespace ZeepSDK.Communication;

internal sealed class ComReceiver : IComReceiver
{
    private static readonly ManualLogSource logger = LoggerFactory.GetLogger<ComReceiver>();

    private readonly ComRegistry registry;
    private volatile bool isDisposed;

    public string SourceModGuid { get; }
    public string Topic { get; }
    public bool IsDisposed => isDisposed;

    public event MessageReceivedDelegate MessageReceived;

    public ComReceiver(ComRegistry registry, string sourceModGuid, string topic)
    {
        this.registry = registry;
        SourceModGuid = sourceModGuid;
        Topic = topic;
    }

    internal void Dispatch(ComMessage message)
    {
        // The sender iterates a snapshot, so a receiver disposed mid-dispatch can still show up here.
        if (isDisposed)
            return;

        if (Topic != null && !string.Equals(Topic, message.Topic, StringComparison.Ordinal))
            return;

        MessageReceivedDelegate handlers = MessageReceived;
        if (handlers == null)
            return;

        foreach (Delegate handler in handlers.GetInvocationList())
        {
            try
            {
                ((MessageReceivedDelegate)handler).Invoke(this, message);
            }
            catch (Exception e)
            {
                logger.LogError(
                    $"Receiver for '{SourceModGuid}' (topic '{Topic ?? "*"}') threw while handling message " +
                    $"on topic '{message.Topic}': {e}");
            }
        }
    }

    public void Dispose()
    {
        if (isDisposed)
            return;

        isDisposed = true;
        registry.Remove(this);
        MessageReceived = null;
    }
}
