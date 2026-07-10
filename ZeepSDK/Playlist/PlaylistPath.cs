using System;
using System.IO;
using System.Linq;

namespace ZeepSDK.Playlist;

internal static class PlaylistPath
{
    private const string Extension = ".zeeplist";

    public static string DirectoryPath => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "Zeepkist",
        "Playlists");

    public static string Resolve(string name)
    {
        return Resolve(DirectoryPath, name);
    }

    internal static string Resolve(string directoryPath, string name)
    {
        ValidateName(name);

        string root = Path.GetFullPath(directoryPath)
            .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        string rootPrefix = root + Path.DirectorySeparatorChar;
        string candidate = Path.GetFullPath(Path.Combine(root, name + Extension));

        if (!candidate.StartsWith(rootPrefix, StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("Playlist name must stay within the playlist directory.", nameof(name));

        return candidate;
    }

    private static void ValidateName(string name)
    {
        if (name == null)
            throw new ArgumentNullException(nameof(name));
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Playlist name cannot be empty.", nameof(name));
        if (Path.IsPathRooted(name) || name != Path.GetFileName(name))
            throw new ArgumentException("Playlist name cannot contain a path.", nameof(name));
        if (name is "." or ".." || name.EndsWith(".", StringComparison.Ordinal) ||
            name.EndsWith(" ", StringComparison.Ordinal))
            throw new ArgumentException("Playlist name is not a valid filename.", nameof(name));
        if (name.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            throw new ArgumentException("Playlist name contains invalid filename characters.", nameof(name));

        string deviceName = name.Split('.')[0];
        string[] reservedDeviceNames =
        [
            "CON", "PRN", "AUX", "NUL",
            "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9",
            "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9"
        ];

        if (reservedDeviceNames.Contains(deviceName, StringComparer.OrdinalIgnoreCase))
            throw new ArgumentException("Playlist name is reserved by the operating system.", nameof(name));
    }
}
