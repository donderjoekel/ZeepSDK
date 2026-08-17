using System;
using System.Linq;
using Xunit;
using ZeepSDK.Scripting.ZUA;

namespace ZeepSDK.Tests;

public class LuaApiDescriptorCacheTests
{
    [Fact]
    public void CachesDescriptorsAndFactories()
    {
        var first = LuaApiDescriptorCache.Functions;
        var second = LuaApiDescriptorCache.Functions;
        LuaApiDescriptor<ILuaFunction> descriptor = first.Single(item => item.Type == typeof(TestFunction));

        Assert.Same(first, second);
        Assert.IsType<TestFunction>(descriptor.Create());
    }

    [Fact]
    public void ExcludesTypesWithoutParameterlessConstructors()
    {
        Assert.DoesNotContain(
            LuaApiDescriptorCache.Functions,
            descriptor => descriptor.Type == typeof(FunctionWithoutDefaultConstructor));
    }

    private sealed class TestFunction : ILuaFunction
    {
        public string Namespace => "test";
        public string Name => "cached";
        public Delegate CreateFunction() => new Action(() => { });
    }

    private sealed class FunctionWithoutDefaultConstructor : ILuaFunction
    {
        public FunctionWithoutDefaultConstructor(string value) { }
        public string Namespace => "test";
        public string Name => "excluded";
        public Delegate CreateFunction() => new Action(() => { });
    }
}
