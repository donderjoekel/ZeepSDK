using System;
using System.IO;

namespace ZeepSDK.Utilities;

internal static class AtomicFile
{
    public static void WriteAllText(string path, string contents)
    {
        Write(path, temporaryPath => File.WriteAllText(temporaryPath, contents));
    }

    public static void WriteAllBytes(string path, byte[] contents)
    {
        Write(path, temporaryPath => File.WriteAllBytes(temporaryPath, contents));
    }

    internal static void Write(string path, Action<string> writeTemporaryFile)
    {
        if (path == null)
            throw new ArgumentNullException(nameof(path));
        if (writeTemporaryFile == null)
            throw new ArgumentNullException(nameof(writeTemporaryFile));

        string fullPath = Path.GetFullPath(path);
        string directory = Path.GetDirectoryName(fullPath);
        if (string.IsNullOrEmpty(directory))
            throw new ArgumentException("Destination must have a parent directory.", nameof(path));

        Directory.CreateDirectory(directory);
        string temporaryPath = Path.Combine(
            directory,
            $".{Path.GetFileName(fullPath)}.{Guid.NewGuid():N}.tmp");

        try
        {
            writeTemporaryFile(temporaryPath);

            if (File.Exists(fullPath))
                File.Replace(temporaryPath, fullPath, null);
            else
                File.Move(temporaryPath, fullPath);
        }
        finally
        {
            if (File.Exists(temporaryPath))
                File.Delete(temporaryPath);
        }
    }
}
