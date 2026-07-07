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
    private const int PresentBlockId = 2264;

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
            .Where(block => block["i"]?.Value<int>() != PresentBlockId)
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
        using StringReader stringReader = new(NormalizeNonFiniteJsonNumbers(content));
        using JsonTextReader jsonReader = new(stringReader)
        {
            FloatParseHandling = FloatParseHandling.Decimal
        };
        return JObject.Load(jsonReader);
    }

    internal static string NormalizeNonFiniteJsonNumbers(string content)
    {
        StringBuilder builder = new(content.Length);
        bool inString = false;
        bool escaped = false;

        for (int index = 0; index < content.Length; index++)
        {
            char character = content[index];
            if (inString)
            {
                builder.Append(character);
                if (escaped)
                    escaped = false;
                else if (character == '\\')
                    escaped = true;
                else if (character == '"')
                    inString = false;
                continue;
            }

            if (character == '"')
            {
                inString = true;
                builder.Append(character);
                continue;
            }

            string replacement = ReplaceBareToken(content, index, "-Infinity")
                                 ?? ReplaceBareToken(content, index, "Infinity")
                                 ?? ReplaceBareToken(content, index, "NaN");
            if (replacement != null)
            {
                int tokenLength = content.AsSpan(index).StartsWith("-Infinity")
                    ? "-Infinity".Length
                    : content.AsSpan(index).StartsWith("Infinity")
                        ? "Infinity".Length
                        : "NaN".Length;
                builder.Append(replacement);
                index += tokenLength - 1;
                continue;
            }

            builder.Append(character);
        }

        return builder.ToString();
    }

    private static string ReplaceBareToken(string content, int index, string token)
    {
        if (!content.AsSpan(index).StartsWith(token))
            return null;

        if (IsJsonIdentifierCharacter(index > 0 ? content[index - 1] : null)
            || IsJsonIdentifierCharacter(index + token.Length < content.Length
                ? content[index + token.Length]
                : null))
            return null;

        return "0";
    }

    private static bool IsJsonIdentifierCharacter(char? character)
    {
        return character is >= 'A' and <= 'Z'
               or >= 'a' and <= 'z'
               or >= '0' and <= '9'
               or '_'
               or '$';
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
