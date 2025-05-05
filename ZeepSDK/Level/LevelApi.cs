using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;
using ZeepSDK.Level.Patches;
using ZeepSDK.Scripting.Attributes;

namespace ZeepSDK.Level;

/// <summary>
/// An API for interacting with levels
/// </summary>
[PublicAPI]
public static class LevelApi
{
    private static readonly Dictionary<string, string> uidToHash = new();

    /// <summary>
    /// The hash of the current level
    /// </summary>
    [GenerateProperty]
    public static string CurrentHash { get; private set; }

    /// <summary>
    /// Gets the current level that is being played
    /// </summary>
    [GenerateProperty]
    public static LevelScriptableObject CurrentLevel => GetLevelFromLoader();

    internal static void Initialize()
    {
        SetupGame_FinishLoading.FinishLoading += FinishLoading;
    }

    private static void FinishLoading(SetupGame instance)
    {
        CurrentHash = GetLevelHash(instance.GlobalLevel);
    }

    private static LevelScriptableObject GetLevelFromLoader()
    {
        if (PlayerManager.Instance == null)
            return null;
        if (PlayerManager.Instance.loader == null)
            return null;
        if (PlayerManager.Instance.loader.GlobalLevel == null)
            return null;

        return PlayerManager.Instance.loader.GlobalLevel;
    }

    /// <summary>
    /// Creates a hash that is unique to this level
    /// </summary>
    /// <param name="levelScriptableObject">The level to hash</param>
    [GenerateFunction]
    public static string GetLevelHash(LevelScriptableObject levelScriptableObject)
    {
        const string winSeparator = "\r\n";
        const string unixSeparator = "\n";

        levelScriptableObject.ForceCheckIfOldOrNewData();

        if (levelScriptableObject.useLevelV15Data)
        {
            string v15LevelData = levelScriptableObject.GetV15LevelData();

            if (v15LevelData.StartsWith("{"))
            {
                v15LevelJSON v15LevelJson = JsonConvert.DeserializeObject<v15LevelJSON>(v15LevelData);
                if (v15LevelJson != null && levelScriptableObject.UseAvonturenLevel)
                    return levelScriptableObject.UID;
                return v15LevelJson.level.zeepHash;
            }

            CsvZeepLevel csvZeepLevel = CsvZeepLevelParser.Parse(
                v15LevelData.Contains(winSeparator)
                    ? v15LevelData.Split(winSeparator)
                    : v15LevelData.Split(unixSeparator));
                
            if (csvZeepLevel != null && levelScriptableObject.UseAvonturenLevel)
                return levelScriptableObject.UID;
            return csvZeepLevel.CalculateHash();
        }
        else
        {
            CsvZeepLevel csvZeepLevel = CsvZeepLevelParser.Parse(levelScriptableObject.GetOldLevelData());
            if (csvZeepLevel != null && levelScriptableObject.UseAvonturenLevel)
                return levelScriptableObject.UID;
            return csvZeepLevel.CalculateHash();
        }
    }
}
