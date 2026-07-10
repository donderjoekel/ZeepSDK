using System;
using System.IO;
using Xunit;
using ZeepSDK.Utilities;

namespace ZeepSDK.Tests;

public sealed class AtomicFileTests : IDisposable
{
    private readonly string directory = Path.Combine(Path.GetTempPath(), "zeepsdk-atomic-file-tests", Guid.NewGuid().ToString("N"));

    public void Dispose()
    {
        if (Directory.Exists(directory))
            Directory.Delete(directory, true);
    }

    [Fact]
    public void ReplacesExistingFile()
    {
        string path = Path.Combine(directory, "settings.json");
        Directory.CreateDirectory(directory);
        File.WriteAllText(path, "old");

        AtomicFile.WriteAllText(path, "new");

        Assert.Equal("new", File.ReadAllText(path));
    }

    [Fact]
    public void FailedWritePreservesExistingFileAndCleansTemporaryFile()
    {
        string path = Path.Combine(directory, "settings.json");
        Directory.CreateDirectory(directory);
        File.WriteAllText(path, "old");

        Assert.Throws<IOException>(() => AtomicFile.Write(path, temporaryPath =>
        {
            File.WriteAllText(temporaryPath, "partial");
            throw new IOException("simulated write failure");
        }));

        Assert.Equal("old", File.ReadAllText(path));
        Assert.Equal(new[] { path }, Directory.GetFiles(directory));
    }
}
