using System;
using System.Collections.Generic;

namespace ZeepSDK.Scripting.ZUA;

internal static class ZuaFunctionCache
{
    private static readonly List<Type> cache = []; 
    
    public static void CacheFunction<TFunction>()
        where TFunction : ILuaFunction, new()
    {
        cache.Add(typeof(TFunction));
    }

    public static void RegisterCachedFunctions(Zua zua)
    {
        foreach (Type type in cache)
        {
            zua.RegisterFunction(type);
        }
    }
}
