using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace ZeepSDK.Utilities;

/// <summary>
/// A cache for components that live somewhere in the project.
/// It uses Object.FindObjectOfType to find the component so this is not particularly efficient.
/// </summary>
public static class ComponentCache
{
    private static readonly Dictionary<Type, Object> cache = new();

    /// <summary>
    /// Returns the component of type T. If it already exists in the cache and is not null it will return that.
    /// If it does not exist it will try to find it using Object.FindObjectOfType.
    /// If it finds it, it will cache it and return it.
    /// If it does not find it, it will return null.
    /// </summary>
    /// <param name="includeInactive">Should inactive objects be included</param>
    /// <typeparam name="T">The type of the component</typeparam>
    /// <returns></returns>
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
