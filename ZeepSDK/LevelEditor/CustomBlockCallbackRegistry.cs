using System.Collections.Generic;

namespace ZeepSDK.LevelEditor;

internal static class CustomBlockCallbackRegistry
{
    private static readonly Dictionary<int, CustomBlockCallback> _blockIdToCallback = [];

    public static void Register(int blockId, CustomBlockCallback callback)
    {
        _blockIdToCallback.Add(blockId, callback);
    }

    public static bool TryInvoke(int blockId)
    {
        if (_blockIdToCallback.TryGetValue(blockId, out CustomBlockCallback callback))
        {
            callback?.Invoke();
            return true;
        }

        return false;
    }
}
