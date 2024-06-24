using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using BepInEx.Logging;
using JetBrains.Annotations;
using ZeepSDK.Utilities;

namespace ZeepSDK.Level;

/// <summary>
/// An API for interacting with levels
/// </summary>
[PublicAPI]
public static class LevelApi
{
    private static readonly ManualLogSource logger = LoggerFactory.GetLogger(typeof(LevelApi));
    private static readonly Vector3Comparer _vector3Comparer = new();
    private static readonly IntComparer _intSequenceComparer = new();
    private static readonly FloatComparer _floatSequenceComparer = new();

    private static readonly Dictionary<string, string> uidToHash = new();

    /// <summary>
    /// Gets the current level that is being played
    /// </summary>
    public static LevelScriptableObject CurrentLevel
    {
        get
        {
            if (PlayerManager.Instance == null)
                return null;
            if (PlayerManager.Instance.loader == null)
                return null;
            return PlayerManager.Instance.loader.GlobalLevel;
        }
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
        StringBuilder inputBuilder = new();
        inputBuilder.AppendLine(zeepLevel.Skybox.ToString());
        inputBuilder.AppendLine(zeepLevel.Ground.ToString());

        List<ZeepBlock> orderedBlocks = zeepLevel.Blocks.OrderBy(x => x.Id)
            .ThenBy(x => x.Position, _vector3Comparer)
            .ThenBy(x => x.Euler, _vector3Comparer)
            .ThenBy(x => x.Scale, _vector3Comparer)
            .ThenBy(x => x.Paints, _intSequenceComparer)
            .ThenBy(x => x.Options, _floatSequenceComparer)
            .ToList();

        foreach (ZeepBlock block in orderedBlocks)
        {
            inputBuilder.AppendLine(block.ToString());
        }

        byte[] hash = SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(inputBuilder.ToString()));
        StringBuilder hashBuilder = new(hash.Length * 2);

        foreach (byte b in hash)
        {
            hashBuilder.Append(b.ToString("X2"));
        }

        return hashBuilder.ToString();
    }
}
