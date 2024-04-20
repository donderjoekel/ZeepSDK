using System.Collections.Generic;

namespace ZeepSDK.Communication;

internal class ReceiverRepository
{
    private static readonly Dictionary<string, List<IComReceiver>> _receivers = [];

    public static void AddReceiver(IComReceiver receiver)
    {
        _ = _receivers.TryAdd(receiver.ModIdentifier, []);
        _receivers[receiver.ModIdentifier].Add(receiver);
    }

    public static void RemoveReceiver(IComReceiver receiver)
    {
        if (_receivers.TryGetValue(receiver.ModIdentifier, out List<IComReceiver> list))
        {
            _ = list.Remove(receiver);
        }
    }

    public static List<IComReceiver> GetReceivers(string modIdentifier)
    {
        return _receivers.TryGetValue(modIdentifier, out List<IComReceiver> list) ? list : [];
    }
}
