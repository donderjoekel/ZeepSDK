using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Hashing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;
using ZeepSDK.Level;

namespace ZeepSDK.Tests;

/**
 * LegacyHashCompatibilityTests is an unbreakable contract between ZeepSDK and
 * the ZeepCentraal API for legacy SHA1/zeepHash and XXH128 hash generation.
 *
 * NEVER modify recorded hashes in `vectors.csv`.
 *
 * If a test fails to generate the expected hash, it is a breaking change and
 * must be corrected before merging. Breaking this contract will result in
 * ZeepSDK/GTR being unable to associate record submissions to the correct level
 * in ZeepCentraal.
 */

public class LegacyHashCompatibilityTests
{
    private static readonly string FixtureDirectory =
        Path.Combine(AppContext.BaseDirectory, "testdata", "legacy-hash");

    public static IEnumerable<object[]> Vectors()
    {
        string manifestPath = Path.Combine(FixtureDirectory, "vectors.csv");
        return File.ReadAllLines(manifestPath)
            .Skip(1)
            .Select(line =>
            {
                string[] values = line.Split(',');
                return new object[] { values[0], values[1], values[2], values[3], values[4] };
            });
    }

    [Theory]
    [MemberData(nameof(Vectors))]
    public void HashesMatchGoldenVector(
        string fileName,
        string format,
        string expectedZeepHash,
        string expectedSha256,
        string expectedXxh128)
    {
        string path = Path.Combine(FixtureDirectory, fileName);
        byte[] bytes = File.ReadAllBytes(path);
        string content = File.ReadAllText(path);

        string actualSha256;
        using (SHA256 sha256 = SHA256.Create())
        {
            actualSha256 = string.Concat(sha256.ComputeHash(bytes).Select(value => value.ToString("X2")));
        }

        Assert.Equal(expectedSha256, actualSha256);

        if (format == "csv")
        {
            CsvZeepLevel level = CsvZeepLevelParser.Parse(File.ReadAllLines(path));
            Assert.NotNull(level);
            Assert.Equal(expectedZeepHash, level.CalculateHash());
            Assert.Equal(expectedXxh128, level.CalculateXxHash());
            return;
        }

        JObject json = JObject.Parse(content);
        Assert.Equal(expectedZeepHash, json["level"]?["zeepHash"]?.Value<string>());
        Assert.Equal(expectedXxh128, XxHash(CanonicalJsonBlocks(json)));
    }

    private static string XxHash(string content)
    {
        return ToUpperHex(XxHash128.Hash(Encoding.UTF8.GetBytes(content)));
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
        StringBuilder builder = new(hash.Length * 2);
        foreach (byte value in hash)
            builder.Append(value.ToString("X2"));
        return builder.ToString();
    }
}
