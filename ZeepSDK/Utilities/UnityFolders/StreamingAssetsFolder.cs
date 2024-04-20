using JetBrains.Annotations;
using UnityEngine;

namespace ZeepSDK.Utilities.UnityFolders;

/// <summary>
/// An interface for accessing the game's StreamingAssets folder.
/// </summary>
[PublicAPI]
public static class StreamingAssetsFolder
{
    private static readonly UnityFolder _folder = new(Application.streamingAssetsPath);

    /// <summary>
    /// Creates a file with the given contents
    /// </summary>
    /// <param name="filename">The filename including extension</param>
    /// <param name="contents">The contents to write</param>
    public static void CreateFile(string filename, byte[] contents)
    {
        _folder.CreateFile(filename, contents);
    }

    /// <summary>
    /// Creates a file with the given contents
    /// </summary>
    /// <param name="filename">The filename including extension</param>
    /// <param name="contents">The contents to write</param>
    public static void CreateFile(string filename, string contents)
    {
        _folder.CreateFile(filename, contents);
    }

    /// <summary>
    /// Deletes a file
    /// </summary>
    /// <param name="filename">The filename, including extension, to delete</param>
    public static void DeleteFile(string filename)
    {
        _folder.DeleteFile(filename);
    }

    /// <summary>
    /// Copies a file to the destination
    /// </summary>
    /// <param name="sourcePath">The full path of the source file</param>
    /// <param name="destinationFilename">The destination filename including extension</param>
    /// <param name="overwrite">If a file with the name already exists, should it be overwritten</param>
    public static void CopyFile(string sourcePath, string destinationFilename, bool overwrite)
    {
        _folder.CopyFile(sourcePath, destinationFilename, overwrite);
    }

    /// <summary>
    /// Moves a file to the destination
    /// </summary>
    /// <param name="sourcePath">The full path of the source file</param>
    /// <param name="destinationFilename">The destination filename including extension</param>
    /// <param name="overwrite">If a file with the name already exists, should it be overwritten</param>
    public static void MoveFile(string sourcePath, string destinationFilename, bool overwrite)
    {
        _folder.MoveFile(sourcePath, destinationFilename, overwrite);
    }

    /// <summary>
    /// Checks if the given file exists
    /// </summary>
    /// <param name="filename">The name of the file, including extension</param>
    /// <returns>True if it exists, false if it doesn't</returns>
    public static bool Exists(string filename)
    {
        return _folder.Exists(filename);
    }
}
