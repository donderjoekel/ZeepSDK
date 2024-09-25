using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using BepInEx.Logging;
using JetBrains.Annotations;
using ZeepSDK.Extensions;
using ZeepSDK.Level.Patches;
using ZeepSDK.Utilities;

namespace ZeepSDK.Level;

/// <summary>
/// An API for interacting with levels
/// </summary>
[PublicAPI]
public static class LevelApi
{
    private const string OldLevel05Hash = "56B1927C4A6BD753558C431D617ECCD68BCF4A39";
    private const string NewLevel05Hash = "2F6551106C2A82137CF7EE39FB71757DA38866A2";
    private const int PresentBlockID = 2264;
    private static readonly ManualLogSource logger = LoggerFactory.GetLogger(typeof(LevelApi));
    private static readonly Vector3Comparer _vector3Comparer = new();
    private static readonly IntComparer _intSequenceComparer = new();
    private static readonly FloatComparer _floatSequenceComparer = new();

    private static readonly Dictionary<string, string> uidToHash = new();

    /// <summary>
    /// The parsed version of the current level
    /// </summary>
    public static ZeepLevel CurrentZeepLevel { get; private set; }

    /// <summary>
    /// The hash of the current level
    /// </summary>
    public static string CurrentHash { get; private set; }

    /// <summary>
    /// Gets the current level that is being played
    /// </summary>
    public static LevelScriptableObject CurrentLevel => GetLevelFromLoader();

    internal static void Initialize()
    {
        SetupGame_FinishLoading.FinishLoading += FinishLoading;
    }

    private static void FinishLoading(SetupGame instance)
    {
        CurrentZeepLevel = ZeepLevelParser.Parse(instance.lines);
        CurrentHash = Hash(CurrentZeepLevel);
    }

    private static LevelScriptableObject GetLevelFromLoader()
    {
        if (PlayerManager.Instance == null)
            return null;
        if (PlayerManager.Instance.loader == null)
            return null;
        if (PlayerManager.Instance.loader.GlobalLevel == null)
            return null;
        if (PlayerManager.Instance.loader.GlobalLevel.LevelData == null ||
            PlayerManager.Instance.loader.GlobalLevel.LevelData.Length == 0)
        {
            return null;
        }

        return PlayerManager.Instance.loader.GlobalLevel;
    }

    /// <summary>
    /// Gets the hash of the current level
    /// <remarks>This is a shorthand for <see cref="GetLevelHash(LevelScriptableObject)"/> combined with <see cref="CurrentLevel"/></remarks>
    /// </summary>
    [Obsolete("Use CurrentHash instead")]
    public static string GetCurrentLevelHash()
    {
        return GetLevelHash(CurrentLevel);
    }

    /// <summary>
    /// Creates a hash that is unique to this level
    /// </summary>
    /// <param name="levelScriptableObject">The level to hash</param>
    public static string GetLevelHash(LevelScriptableObject levelScriptableObject)
    {
        ZeepLevel zeepLevel = ZeepLevelParser.Parse(levelScriptableObject.LevelData);
        return zeepLevel == null ? null : Hash(zeepLevel);
    }

    private static string Hash(ZeepLevel zeepLevel)
    {
        if (zeepLevel == null)
        {
            logger.LogWarning("Trying to hash a null level");
            return null;
        }

        StringBuilder inputBuilder = new();
        inputBuilder.AppendCLRF(zeepLevel.Skybox.ToString());
        inputBuilder.AppendCLRF(zeepLevel.Ground.ToString());

        List<ZeepBlock> orderedBlocks = zeepLevel.Blocks
            .Where(x => x.Id != PresentBlockID)
            .OrderBy(x => x.Id)
            .ThenBy(x => x.Position, _vector3Comparer)
            .ThenBy(x => x.Euler, _vector3Comparer)
            .ThenBy(x => x.Scale, _vector3Comparer)
            .ThenBy(x => x.Paints, _intSequenceComparer)
            .ThenBy(x => x.Options, _floatSequenceComparer)
            .ToList();

        foreach (ZeepBlock block in orderedBlocks)
        {
            inputBuilder.AppendCLRF(block.ToString());
        }

        byte[] hash = SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(inputBuilder.ToString()));
        StringBuilder hashBuilder = new(hash.Length * 2);

        foreach (byte b in hash)
        {
            hashBuilder.Append(b.ToString("X2"));
        }

        string levelHash = hashBuilder.ToString();
        return string.Equals(levelHash, OldLevel05Hash, StringComparison.OrdinalIgnoreCase)
            ? NewLevel05Hash
            : levelHash;
    }
}
