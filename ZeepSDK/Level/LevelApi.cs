using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Hashing;
using System.Linq;
using System.Text;
using BepInEx.Logging;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
    /// The trusted V2 hash of the current level
    /// </summary>
    [GenerateProperty]
    public static LevelHashV2 CurrentHashV2 { get; private set; }

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
        CurrentHashV2 = GetLevelHashV2(instance.GlobalLevel);
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
    /// Creates a trusted content hash and legacy zeepHash for this level
    /// </summary>
    /// <param name="levelScriptableObject">The level to hash</param>
    [GenerateFunction]
    public static LevelHashV2 GetLevelHashV2(LevelScriptableObject levelScriptableObject)
    {
        try
        {
            string zeepHash = GetLevelHash(levelScriptableObject);
            if (string.IsNullOrEmpty(zeepHash))
                return null;

            const string winSeparator = "\r\n";
            const string unixSeparator = "\n";

            levelScriptableObject.ForceCheckIfOldOrNewData();

            if (levelScriptableObject.useLevelV15Data)
            {
                string v15LevelData = levelScriptableObject.GetV15LevelData();
                if (v15LevelData.StartsWith("{"))
                    return new LevelHashV2 { Hash = XxHash(CanonicalJsonBlocks(v15LevelData)), ZeepHash = zeepHash };

                CsvZeepLevel csvZeepLevel = CsvZeepLevelParser.Parse(
                    v15LevelData.Contains(winSeparator)
                        ? v15LevelData.Split(winSeparator)
                        : v15LevelData.Split(unixSeparator));
                return new LevelHashV2 { Hash = csvZeepLevel?.CalculateXxHash(), ZeepHash = zeepHash };
            }

            CsvZeepLevel oldCsvZeepLevel = CsvZeepLevelParser.Parse(levelScriptableObject.GetOldLevelData());
            return new LevelHashV2 { Hash = oldCsvZeepLevel?.CalculateXxHash(), ZeepHash = zeepHash };
        }
        catch (Exception e)
        {
            _logger.LogError("Failed to calculate level hash V2");
            _logger.LogError(e);
            return null;
        }
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

    private static string XxHash(string content)
    {
        return ToUpperHex(XxHash128Reflection.Hash(Encoding.UTF8.GetBytes(content)));
    }

    private static string CanonicalJsonBlocks(string content)
    {
        JObject root = ParseCanonicalJson(content);
        JArray blocks = (JArray)root["blox"];
        IEnumerable<string> orderedBlocks = blocks
            .Select((block, index) => new { Canonical = CanonicalJson(block), Index = index })
            .OrderBy(block => block.Canonical, StringComparer.Ordinal)
            .ThenBy(block => block.Index)
            .Select(block => block.Canonical);
        return $"[{string.Join(",", orderedBlocks)}]";
    }

    private static string CanonicalJson(JToken token)
    {
        return token switch
        {
            JObject obj => $"{{{string.Join(",", obj.Properties()
                .OrderBy(property => property.Name, StringComparer.Ordinal)
                .Select(property => $"{JsonConvert.ToString(property.Name)}:{CanonicalJson(property.Value)}"))}}}",
            JArray array => $"[{string.Join(",", array.Select(CanonicalJson))}]",
            JValue value => CanonicalJsonValue(value),
            _ => throw new InvalidOperationException($"Unsupported JSON token: {token.Type}")
        };
    }


    private static string CanonicalJsonValue(JValue value)
    {
        return value.Type switch
        {
            JTokenType.Null => "null",
            JTokenType.Boolean => (bool)value.Value ? "true" : "false",
            JTokenType.String => JsonConvert.ToString((string)value.Value),
            JTokenType.Integer => CanonicalJsonNumber(value),
            JTokenType.Float => CanonicalJsonNumber(value),
            _ => JsonConvert.SerializeObject(value.Value)
        };
    }

    private static JObject ParseCanonicalJson(string content)
    {
        using StringReader stringReader = new(content);
        using JsonTextReader jsonReader = new(stringReader)
        {
            FloatParseHandling = FloatParseHandling.Decimal
        };
        return JObject.Load(jsonReader);
    }

    private static string CanonicalJsonNumber(JValue value)
    {
        decimal number = Convert.ToDecimal(value.Value, CultureInfo.InvariantCulture);
        if (number == decimal.Truncate(number))
            return number.ToString("0", CultureInfo.InvariantCulture);

        decimal absolute = Math.Abs(number);
        if (absolute != 0 && absolute < 0.000001m)
        {
            string fixedText = number.ToString("0.#############################", CultureInfo.InvariantCulture);
            bool negative = fixedText.StartsWith("-");
            string digits = fixedText.TrimStart('-').Replace("0.", string.Empty);
            int leadingZeroes = digits.TakeWhile(character => character == '0').Count();
            string significant = digits[leadingZeroes..].TrimEnd('0');
            string mantissa = significant.Length == 1
                ? significant
                : $"{significant[0]}.{significant[1..]}";
            return $"{(negative ? "-" : string.Empty)}{mantissa}e-{leadingZeroes + 1}";
        }

        return number.ToString("0.#############################", CultureInfo.InvariantCulture);
    }
    private static string ToUpperHex(byte[] hash)
    {
        StringBuilder hashBuilder = new(hash.Length * 2);
        foreach (byte b in hash)
            hashBuilder.Append(b.ToString("X2"));
        return hashBuilder.ToString();
    }
}

