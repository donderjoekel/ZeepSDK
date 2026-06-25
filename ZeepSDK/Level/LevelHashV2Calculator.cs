using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ZeepSDK.Utilities;

namespace ZeepSDK.Level;

internal static class LevelHashV2Calculator
{
    private const string WinSeparator = "\r\n";
    private const string UnixSeparator = "\n";

    internal static LevelHashV2 Calculate(string levelData, string zeepHash)
    {
        if (string.IsNullOrEmpty(levelData) || string.IsNullOrEmpty(zeepHash))
            return null;

        if (levelData.StartsWith("{"))
            return new LevelHashV2 { Hash = CalculateJsonXxHash(levelData), ZeepHash = zeepHash };

        return CalculateCsv(levelData, zeepHash);
    }

    internal static LevelHashV2 CalculateCsv(IEnumerable<string> csvLevelData, string zeepHash)
    {
        if (csvLevelData == null || string.IsNullOrEmpty(zeepHash))
            return null;

        CsvZeepLevel csvZeepLevel = CsvZeepLevelParser.Parse(csvLevelData.ToArray());
        return new LevelHashV2 { Hash = csvZeepLevel?.CalculateXxHash(), ZeepHash = zeepHash };
    }

    internal static LevelHashV2 CalculateCsv(string csvLevelData, string zeepHash)
    {
        if (string.IsNullOrEmpty(csvLevelData) || string.IsNullOrEmpty(zeepHash))
            return null;

        CsvZeepLevel csvZeepLevel = CsvZeepLevelParser.Parse(
            csvLevelData.Contains(WinSeparator)
                ? csvLevelData.Split(WinSeparator)
                : csvLevelData.Split(UnixSeparator));
        return new LevelHashV2 { Hash = csvZeepLevel?.CalculateXxHash(), ZeepHash = zeepHash };
    }

    internal static LevelHashV2 CalculateJson(string content)
    {
        JObject root = ParseCanonicalJson(content);
        return new LevelHashV2
        {
            Hash = XxHash(CanonicalJsonBlocks(root)),
            ZeepHash = root["level"]?["zeepHash"]?.Value<string>()
        };
    }

    internal static string CalculateJsonXxHash(string content)
    {
        return CalculateJson(content)?.Hash;
    }

    private static string CanonicalJsonBlocks(JObject root)
    {
        JArray blocks = (JArray)root["blox"];
        IEnumerable<string> orderedBlocks = blocks
            .Select((block, index) => new { Canonical = CanonicalJson(block), Index = index })
            .OrderBy(block => block.Canonical, StringComparer.Ordinal)
            .ThenBy(block => block.Index)
            .Select(block => block.Canonical);
        return $"[{string.Join(",", orderedBlocks)}]";
    }

    private static string XxHash(string content)
    {
        return ToUpperHex(XxHash128Reflection.Hash(Encoding.UTF8.GetBytes(content)));
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

