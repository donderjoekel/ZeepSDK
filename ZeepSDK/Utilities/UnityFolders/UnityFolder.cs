using System;
using System.IO;

namespace ZeepSDK.Utilities.UnityFolders;

internal class UnityFolder
{
    private readonly string _path;

    public UnityFolder(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            throw new ArgumentNullException(path);
        }

        _path = path;
    }

    private static void EnsureDirectory(string destinationPath)
    {
        string directoryName = Path.GetDirectoryName(destinationPath);
        if (string.IsNullOrEmpty(directoryName))
        {
            throw new InvalidOperationException("Directory name is null or empty.");
        }

        if (!Directory.Exists(directoryName))
        {
            _ = Directory.CreateDirectory(directoryName);
        }
    }

    public void CreateFile(string filename, byte[] contents)
    {
        string destinationPath = Path.Combine(_path, filename);
        EnsureDirectory(destinationPath);
        File.WriteAllBytes(destinationPath, contents);
    }

    public void CreateFile(string filename, string contents)
    {
        string destinationPath = Path.Combine(_path, filename);
        EnsureDirectory(destinationPath);
        File.WriteAllText(destinationPath, contents);
    }

    public void DeleteFile(string filename)
    {
        string destinationPath = Path.Combine(_path, filename);
        if (File.Exists(destinationPath))
        {
            File.Delete(destinationPath);
        }
    }

    public void CopyFile(string sourcePath, string destinationFilename, bool overwrite)
    {
        string destinationPath = Path.Combine(_path, destinationFilename);
        EnsureDirectory(destinationPath);
        File.Copy(sourcePath, destinationPath, overwrite);
    }

    public void MoveFile(string sourcePath, string destinationFilename, bool overwrite)
    {
        string destinationPath = Path.Combine(_path, destinationFilename);
        EnsureDirectory(destinationPath);
        if (File.Exists(destinationFilename) && overwrite)
        {
            File.Delete(destinationFilename);
        }

        File.Move(sourcePath, destinationPath);
    }

    public bool Exists(string filename)
    {
        return File.Exists(Path.Combine(_path, filename));
    }
}
