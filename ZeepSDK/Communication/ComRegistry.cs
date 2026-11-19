using System;
using System.Collections.Generic;

namespace ZeepSDK.Communication;

/// <summary>
/// Thread-safe registry of receivers, keyed by the GUID of the mod they listen to.
/// </summary>
internal sealed class ComRegistry
{
    private static readonly ComReceiver[] Empty = Array.Empty<ComReceiver>();

    private readonly object gate = new();
    private readonly Dictionary<string, List<ComReceiver>> receivers = new(StringComparer.Ordinal);

    public void Add(ComReceiver receiver)
    {
        lock (gate)
        {
            if (!receivers.TryGetValue(receiver.SourceModGuid, out List<ComReceiver> list))
            {
                list = new List<ComReceiver>();
                receivers.Add(receiver.SourceModGuid, list);
            }

            list.Add(receiver);
        }
    }

    public void Remove(ComReceiver receiver)
    {
        lock (gate)
        {
            if (!receivers.TryGetValue(receiver.SourceModGuid, out List<ComReceiver> list))
                return;

            list.Remove(receiver);
            if (list.Count == 0)
                receivers.Remove(receiver.SourceModGuid);
        }
    }

    /// <summary>
    /// Returns a snapshot so receivers can (un)subscribe while a message is being dispatched.
    /// </summary>
    public ComReceiver[] GetReceivers(string sourceModGuid)
    {
        lock (gate)
        {
            return receivers.TryGetValue(sourceModGuid, out List<ComReceiver> list) ? list.ToArray() : Empty;
        }
    }
}
