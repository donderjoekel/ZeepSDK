using System;
using System.IO;

namespace ZeepSDK.Utilities.UnityFolders;

internal class UnityFolder
{
    private readonly string path;
    private readonly string pathPrefix;

    public UnityFolder(string path)
    {
        if (path == null)
            throw new ArgumentNullException(nameof(path));
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("Folder path cannot be empty.", nameof(path));

        this.path = Path.GetFullPath(path);
        pathPrefix = this.path.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) +
                     Path.DirectorySeparatorChar;
    }

    private string ResolveDestinationPath(string filename)
    {
        if (filename == null)
            throw new ArgumentNullException(nameof(filename));
        if (string.IsNullOrWhiteSpace(filename))
            throw new ArgumentException("Filename cannot be empty.", nameof(filename));
        if (Path.IsPathRooted(filename))
            throw new ArgumentException("Filename must be relative to the Unity folder.", nameof(filename));

        string destinationPath = Path.GetFullPath(Path.Combine(path, filename));
        if (!destinationPath.StartsWith(pathPrefix, StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("Filename must stay within the Unity folder.", nameof(filename));

        return destinationPath;
    }

    private void EnsureDirectory(string destinationPath)
    {
        string directoryName = Path.GetDirectoryName(destinationPath);
        if (string.IsNullOrEmpty(directoryName))
            throw new InvalidOperationException("Directory name is null or empty.");
        if (!Directory.Exists(directoryName))
            Directory.CreateDirectory(directoryName);
    }

    public void CreateFile(string filename, byte[] contents)
    {
        string destinationPath = ResolveDestinationPath(filename);
        EnsureDirectory(destinationPath);
        File.WriteAllBytes(destinationPath, contents);
    }

    public void CreateFile(string filename, string contents)
    {
        string destinationPath = ResolveDestinationPath(filename);
        EnsureDirectory(destinationPath);
        File.WriteAllText(destinationPath, contents);
    }

    public void DeleteFile(string filename)
    {
        string destinationPath = ResolveDestinationPath(filename);
        if (File.Exists(destinationPath))
            File.Delete(destinationPath);
    }

    public void CopyFile(string sourcePath, string destinationFilename, bool overwrite)
    {
        string destinationPath = ResolveDestinationPath(destinationFilename);
        EnsureDirectory(destinationPath);
        File.Copy(sourcePath, destinationPath, overwrite);
    }

    public void MoveFile(string sourcePath, string destinationFilename, bool overwrite)
    {
        string destinationPath = ResolveDestinationPath(destinationFilename);
        EnsureDirectory(destinationPath);
        if (File.Exists(destinationPath) && overwrite)
            File.Delete(destinationPath);
        File.Move(sourcePath, destinationPath);
    }

    public bool Exists(string filename)
    {
        return File.Exists(ResolveDestinationPath(filename));
    }
}
