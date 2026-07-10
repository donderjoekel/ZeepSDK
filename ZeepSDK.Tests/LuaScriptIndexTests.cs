using System;
using System.IO;
using Xunit;
using ZeepSDK.Scripting;

namespace ZeepSDK.Tests;

public class LuaScriptIndexTests : IDisposable
{
    private readonly string root = Path.Combine(Path.GetTempPath(), $"zeepsdk-lua-{Guid.NewGuid():N}");

    [Fact]
    public void ResolvesLiteralNamesCaseInsensitively()
    {
        Directory.CreateDirectory(Path.Combine(root, "nested"));
        string expected = Path.Combine(root, "nested", "Example.lua");
        File.WriteAllText(expected, "return true");
        LuaScriptIndex index = new(root);
        index.Refresh();

        bool found = index.TryResolve("example", out string actual, out string error);

        Assert.True(found, error);
        Assert.Equal(Path.GetFullPath(expected), actual, StringComparer.OrdinalIgnoreCase);
    }

    [Fact]
    public void RejectsPathsAndAmbiguousNames()
    {
        Directory.CreateDirectory(Path.Combine(root, "one"));
        Directory.CreateDirectory(Path.Combine(root, "two"));
        File.WriteAllText(Path.Combine(root, "one", "same.lua"), "return true");
        File.WriteAllText(Path.Combine(root, "two", "same.lua"), "return true");
        LuaScriptIndex index = new(root);
        index.Refresh();

        Assert.False(index.TryResolve("one/same", out _, out _));
        Assert.False(index.TryResolve("same", out _, out string error));
        Assert.Contains("ambiguous", error);
    }

    [Fact]
    public void RefreshControlsVisibilityOfNewFiles()
    {
        Directory.CreateDirectory(root);
        LuaScriptIndex index = new(root);
        index.Refresh();
        File.WriteAllText(Path.Combine(root, "later.lua"), "return true");

        Assert.False(index.TryResolve("later", out _, out _));

        index.Refresh();
        Assert.True(index.TryResolve("later", out _, out _));
    }

    public void Dispose()
    {
        if (Directory.Exists(root))
            Directory.Delete(root, true);
    }
}
