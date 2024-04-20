using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace ZeepSDK.Utilities;

internal static class ComponentCache
{
    private static readonly Dictionary<Type, Object> _cache = [];

    public static T Get<T>(bool includeInactive = false) where T : Object
    {
        Type type = typeof(T);
        if (_cache.TryGetValue(type, out Object value))
        {
            if (value != null)
            {
                return (T)value;
            }
        }

        T instance = Object.FindObjectOfType<T>(includeInactive);
        if (instance != null)
        {
            _cache[type] = instance;
            return instance;
        }

        _ = _cache.Remove(type);
        return null;
    }
}
