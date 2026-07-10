using System;
using System.IO;
using Xunit;
using ZeepSDK.Playlist;

namespace ZeepSDK.Tests;

public class PlaylistPathTests
{
    private readonly string root = Path.Combine(Path.GetTempPath(), "zeepsdk-playlist-tests");

    [Theory]
    [InlineData("../escape")]
    [InlineData("sub/playlist")]
    [InlineData(".")]
    [InlineData("..")]
    [InlineData("name.")]
    [InlineData("name ")]
    [InlineData("CON")]
    [InlineData("nul.json")]
    public void ResolveRejectsUnsafePlaylistName(string name)
    {
        Assert.Throws<ArgumentException>(() => PlaylistPath.Resolve(root, name));
    }

    [Fact]
    public void ResolveRejectsRootedPlaylistName()
    {
        string rootedName = Path.Combine(Path.GetPathRoot(root)!, "escape");

        Assert.Throws<ArgumentException>(() => PlaylistPath.Resolve(root, rootedName));
    }

    [Fact]
    public void ResolveKeepsValidUnicodeNameInsidePlaylistRoot()
    {
        string result = PlaylistPath.Resolve(root, "Médailles 旅行");

        Assert.Equal(Path.Combine(root, "Médailles 旅行.zeeplist"), result);
    }
}
