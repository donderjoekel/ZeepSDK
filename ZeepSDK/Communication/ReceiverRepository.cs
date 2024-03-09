using System.Collections.Generic;

namespace ZeepSDK.Communication;

internal class ReceiverRepository
{
    private static readonly Dictionary<string, List<IComReceiver>> receivers = new();

    public static void AddReceiver(IComReceiver receiver)
    {
        receivers.TryAdd(receiver.ModIdentifier, new List<IComReceiver>());
        receivers[receiver.ModIdentifier].Add(receiver);
    }

    public static void RemoveReceiver(IComReceiver receiver)
    {
        if (receivers.TryGetValue(receiver.ModIdentifier, out List<IComReceiver> list))
            list.Remove(receiver);
    }

    public static List<IComReceiver> GetReceivers(string modIdentifier)
    {
        return receivers.TryGetValue(modIdentifier, out List<IComReceiver> list) ? list : new List<IComReceiver>();
    }
}
