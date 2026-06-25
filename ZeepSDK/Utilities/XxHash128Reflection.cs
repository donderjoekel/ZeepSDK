using System;
using System.Reflection;

namespace ZeepSDK.Utilities;

/// <summary>
/// Calls System.IO.Hashing.XxHash128 without referencing Span-based APIs at compile time.
/// ZeepSDK targets .NET Framework for Unity/BepInEx; direct System.IO.Hashing calls pull in
/// System.Memory compile-time types that conflict with Unity profile assemblies. Reflection keeps
/// the dependency runtime-only while preserving identical XXH128 output.
/// </summary>
internal static class XxHash128Reflection
{
    private static readonly Func<byte[], byte[]> HashFunction = ResolveHashFunction();

    internal static byte[] Hash(byte[] bytes)
    {
        return HashFunction(bytes);
    }

    private static Func<byte[], byte[]> ResolveHashFunction()
    {
        Type xxHash128Type = Type.GetType("System.IO.Hashing.XxHash128, System.IO.Hashing", throwOnError: true);
        MethodInfo hashMethod = xxHash128Type.GetMethod("Hash", new[] { typeof(byte[]) });
        if (hashMethod == null)
            throw new MissingMethodException("System.IO.Hashing.XxHash128", "Hash(byte[])");
        return (Func<byte[], byte[]>)Delegate.CreateDelegate(typeof(Func<byte[], byte[]>), hashMethod);
    }
}

