using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace ZeepSDK.Utilities;

internal static class ComponentCache
{
    private static readonly Dictionary<Type, Object> cache = new Dictionary<Type, Object>();

    public static T Get<T>(bool includeInactive = false) where T : Object
    {
        Type type = typeof(T);
        if (cache.TryGetValue(type, out Object value))
        {
            if (value != null)
            {
                return (T)value;
            }
        }

        T instance = Object.FindObjectOfType<T>(includeInactive);
        if (instance != null)
        {
            cache[type] = instance;
            return instance;
        }

        if (cache.ContainsKey(type))
            cache.Remove(type);

        return null;
    }
}
