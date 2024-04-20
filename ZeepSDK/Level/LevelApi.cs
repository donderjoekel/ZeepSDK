﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using JetBrains.Annotations;

namespace ZeepSDK.Level;

/// <summary>
/// An API for interacting with levels
/// </summary>
[PublicAPI]
public static class LevelApi
{
    private static readonly Dictionary<string, string> _uidToHash = [];

    /// <summary>
    /// Creates a unique hash for the level based on the actual contents
    /// </summary>
    /// <param name="levelScriptableObject">The level to create the hash for</param>
    /// <returns>a hash</returns>
    public static string GetLevelHash(LevelScriptableObject levelScriptableObject)
    {
        if (levelScriptableObject == null)
        {
            throw new ArgumentNullException(nameof(levelScriptableObject));
        }

        if (_uidToHash.TryGetValue(levelScriptableObject.UID, out string hash))
        {
            return hash;
        }

        string textToHash = GetTextToHash(levelScriptableObject.LevelData);
        hash = Hash(textToHash);

        _uidToHash[levelScriptableObject.UID] = hash;
        return hash;
    }

    private static string GetTextToHash(string[] lines)
    {
        string[] splits = lines[2].Split(',');

        string skyboxAndBasePlate = splits.Length != 6
            ? "unknown,unknown"
            : splits[^2] + "," + splits[^1];

        return string.Join("\n", lines.Skip(3).Prepend(skyboxAndBasePlate));
    }

    private static string Hash(string input)
    {
#pragma warning disable CA5350 // Ignoring because this isn't used for security purposes
        using SHA1 sha1 = SHA1.Create();
#pragma warning restore CA5350

        byte[] hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
        StringBuilder sb = new(hash.Length * 2);

        foreach (byte b in hash)
        {
            _ = sb.Append(b.ToString("X2", CultureInfo.InvariantCulture));
        }

        return sb.ToString();
    }
}
