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
            FloatParseHandling = FloatParseHandling.Double
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
        double number = Convert.ToDouble(value.Value, CultureInfo.InvariantCulture);
        if (number == 0)
            return "0";

        string shortest = ToShortestRoundTrip(number);
        double absolute = Math.Abs(number);
        if (absolute >= 0.000001d && absolute < 1e21d)
            return ToFixedNotation(shortest);

        return NormalizeExponent(shortest);
    }

    private static string ToShortestRoundTrip(double number)
    {
        for (int precision = 15; precision <= 17; precision++)
        {
            string candidate = number.ToString($"G{precision}", CultureInfo.InvariantCulture);
            if (double.TryParse(
                    candidate,
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture,
                    out double parsed)
                && parsed.Equals(number))
                return candidate;
        }

        return number.ToString("R", CultureInfo.InvariantCulture);
    }

    private static string ToFixedNotation(string text)
    {
        int exponentIndex = text.IndexOfAny(new[] { 'E', 'e' });
        if (exponentIndex < 0)
            return text;

        bool negative = text[0] == '-';
        string mantissa = text[..exponentIndex].TrimStart('-');
        int exponent = int.Parse(text[(exponentIndex + 1)..], CultureInfo.InvariantCulture);
        int dotIndex = mantissa.IndexOf('.');
        int fractionalDigits = dotIndex < 0 ? 0 : mantissa.Length - dotIndex - 1;
        string digits = mantissa.Replace(".", string.Empty);
        int point = digits.Length - fractionalDigits + exponent;
        string result = point <= 0
            ? $"0.{new string('0', -point)}{digits}"
            : point >= digits.Length
                ? $"{digits}{new string('0', point - digits.Length)}"
                : $"{digits[..point]}.{digits[point..]}";
        if (result.Contains("."))
            result = result.TrimEnd('0').TrimEnd('.');
        return negative ? $"-{result}" : result;
    }

    private static string NormalizeExponent(string text)
    {
        int exponentIndex = text.IndexOfAny(new[] { 'E', 'e' });
        if (exponentIndex < 0)
            return text;

        string mantissa = text[..exponentIndex].ToLowerInvariant();
        int exponent = int.Parse(text[(exponentIndex + 1)..], CultureInfo.InvariantCulture);
        return $"{mantissa}e{(exponent >= 0 ? "+" : string.Empty)}{exponent.ToString(CultureInfo.InvariantCulture)}";
    }

    private static string ToUpperHex(byte[] hash)
    {
        StringBuilder hashBuilder = new(hash.Length * 2);
        foreach (byte b in hash)
            hashBuilder.Append(b.ToString("X2"));
        return hashBuilder.ToString();
    }
}
