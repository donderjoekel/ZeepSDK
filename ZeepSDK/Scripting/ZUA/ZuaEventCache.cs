using System;
using System.Collections.Generic;

namespace ZeepSDK.Scripting.ZUA;

internal static class ZuaEventCache
{
    private static readonly List<Type> cache = [];

    public static void CacheEvent<TEvent>()
        where TEvent : ILuaEvent, new()
    {
        cache.Add(typeof(TEvent));
    }

    public static void RegisterCachedEvents(Zua zua)
    {
        foreach (Type type in cache)
        {
            zua.RegisterEvent(type);
        }
    }
}
