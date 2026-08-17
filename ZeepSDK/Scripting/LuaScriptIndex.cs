using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ZeepSDK.Scripting;

internal sealed class LuaScriptIndex
{
    private const int MaximumIndexedScripts = 10_000;
    private readonly string rootPath;
    private Dictionary<string, List<string>> pathsByName =
        new(StringComparer.OrdinalIgnoreCase);

    public LuaScriptIndex(string rootPath)
    {
        if (string.IsNullOrWhiteSpace(rootPath))
            throw new ArgumentException("Script root cannot be empty.", nameof(rootPath));

        this.rootPath = Path.GetFullPath(rootPath);
    }

    public void Refresh()
    {
        Dictionary<string, List<string>> refreshed = new(StringComparer.OrdinalIgnoreCase);
        if (!Directory.Exists(rootPath))
        {
            pathsByName = refreshed;
            return;
        }

        int indexedCount = 0;
        foreach (string path in Directory.EnumerateFiles(rootPath, "*", SearchOption.AllDirectories))
        {
            if (!string.Equals(Path.GetExtension(path), ".lua", StringComparison.OrdinalIgnoreCase))
                continue;
            if (++indexedCount > MaximumIndexedScripts)
                throw new InvalidDataException($"Script index exceeds {MaximumIndexedScripts} Lua files.");

            string canonicalPath = Path.GetFullPath(path);
            string name = Path.GetFileNameWithoutExtension(canonicalPath);
            if (!refreshed.TryGetValue(name, out List<string> matches))
            {
                matches = new List<string>(2);
                refreshed.Add(name, matches);
            }

            if (matches.Count < 2 && !matches.Contains(canonicalPath, StringComparer.OrdinalIgnoreCase))
                matches.Add(canonicalPath);
        }

        pathsByName = refreshed;
    }

    public bool TryResolve(string name, out string path, out string error)
    {
        path = null;
        error = null;

        if (string.IsNullOrWhiteSpace(name) ||
            !string.Equals(Path.GetFileName(name), name, StringComparison.Ordinal) ||
            name.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
        {
            error = "Lua script name must be a literal filename without a path.";
            return false;
        }

        if (!pathsByName.TryGetValue(name, out List<string> matches))
        {
            error = $"No Lua script named '{name}' was found.";
            return false;
        }

        if (matches.Count != 1)
        {
            error = $"Lua script name '{name}' is ambiguous.";
            return false;
        }

        path = matches[0];
        return true;
    }

    public static string CanonicalizePath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("Lua script path cannot be empty.", nameof(path));

        return Path.GetFullPath(path);
    }
}
