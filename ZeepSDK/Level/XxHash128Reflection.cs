using System;
using System.Reflection;

namespace ZeepSDK.Level;

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
