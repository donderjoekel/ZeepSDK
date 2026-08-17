using System;
using System.IO;
using Xunit;
using ZeepSDK.Utilities.UnityFolders;

namespace ZeepSDK.Tests;

public sealed class UnityFolderTests : IDisposable
{
    private readonly string root = Path.Combine(Path.GetTempPath(), "zeepsdk-unity-folder-tests", Guid.NewGuid().ToString("N"));
    private readonly string source = Path.Combine(Path.GetTempPath(), "zeepsdk-unity-folder-tests", Guid.NewGuid().ToString("N"));

    public void Dispose()
    {
        if (Directory.Exists(root))
            Directory.Delete(root, true);
        if (File.Exists(source))
            File.Delete(source);
    }

    [Theory]
    [InlineData("../escape.txt")]
    [InlineData("sub/../../escape.txt")]
    public void CreateFileRejectsPathsOutsideRoot(string filename)
    {
        UnityFolder folder = new(root);

        Assert.Throws<ArgumentException>(() => folder.CreateFile(filename, "content"));
    }

    [Fact]
    public void CreateFileRejectsRootedPath()
    {
        UnityFolder folder = new(root);
        string rootedPath = Path.Combine(Path.GetPathRoot(root)!, "escape.txt");

        Assert.Throws<ArgumentException>(() => folder.CreateFile(rootedPath, "content"));
    }

    [Fact]
    public void CreateFileAllowsNestedRelativePath()
    {
        UnityFolder folder = new(root);

        folder.CreateFile("nested/file.txt", "content");

        Assert.Equal("content", File.ReadAllText(Path.Combine(root, "nested", "file.txt")));
    }

    [Fact]
    public void MoveFileOverwritesResolvedDestination()
    {
        Directory.CreateDirectory(root);
        File.WriteAllText(source, "new");
        File.WriteAllText(Path.Combine(root, "destination.txt"), "old");
        UnityFolder folder = new(root);

        folder.MoveFile(source, "destination.txt", true);

        Assert.Equal("new", File.ReadAllText(Path.Combine(root, "destination.txt")));
    }
}
