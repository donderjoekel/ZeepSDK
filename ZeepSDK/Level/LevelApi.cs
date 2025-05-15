using System;
using System.Collections.Generic;
using BepInEx.Logging;
using JetBrains.Annotations;
using Newtonsoft.Json;
using ZeepSDK.Level.Patches;
using ZeepSDK.Scripting.Attributes;
using ZeepSDK.Scripting.Functions;
using ZeepSDK.Utilities;

namespace ZeepSDK.Level;

/// <summary>
/// An API for interacting with levels
/// </summary>
[PublicAPI]
public static class LevelApi
{
    private static readonly Dictionary<string, string> uidToHash = new();
    private static ManualLogSource _logger = LoggerFactory.GetLogger(typeof(LevelApi));

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
        try
        {
            const string winSeparator = "\r\n";
            const string unixSeparator = "\n";

            levelScriptableObject.ForceCheckIfOldOrNewData();

            CsvZeepLevel csvZeepLevel;

            if (levelScriptableObject.useLevelV15Data)
            {
                string v15LevelData = levelScriptableObject.GetV15LevelData();

                if (v15LevelData.StartsWith("{"))
                {
                    v15LevelJSON v15LevelJson = JsonConvert.DeserializeObject<v15LevelJSON>(v15LevelData);
                    if (v15LevelJson != null && (levelScriptableObject.UseAvonturenLevel || levelScriptableObject.IsAdventureLevel))
                        return levelScriptableObject.UID;
                    return v15LevelJson.level.zeepHash;
                }

                csvZeepLevel = CsvZeepLevelParser.Parse(
                    v15LevelData.Contains(winSeparator)
                        ? v15LevelData.Split(winSeparator)
                        : v15LevelData.Split(unixSeparator));
            }
            else
            {
                csvZeepLevel = CsvZeepLevelParser.Parse(levelScriptableObject.GetOldLevelData());
            }

            if (csvZeepLevel != null && (levelScriptableObject.UseAvonturenLevel || levelScriptableObject.IsAdventureLevel))
                return levelScriptableObject.UID;
        
            return csvZeepLevel?.CalculateHash();
        }
        catch (Exception e)
        {
            _logger.LogError("Failed to calculate level hash");
            _logger.LogError(e);
            return null;
        }
    }
}
