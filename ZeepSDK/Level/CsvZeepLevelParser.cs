using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using BepInEx.Logging;
using ZeepSDK.Numerics;
using ZeepSDK.Utilities;

namespace ZeepSDK.Level;

internal static class CsvZeepLevelParser
{
    private static readonly ManualLogSource _logger = LoggerFactory.GetLogger(typeof(CsvZeepLevelParser));
    private static readonly CultureInfo _culture = new("en-US");

    public static CsvZeepLevel Parse(string[] lines)
    {
        if (lines.Length == 0)
        {
            _logger.LogWarning("Trying to parse an empty level");
            return null;
        }

        CsvZeepLevel level = new();
        if (!ParseFirstLine(lines.ElementAtOrDefault(0) ?? string.Empty, level))
            return null;

        if (!ParseCameraLine(lines.ElementAtOrDefault(1) ?? string.Empty, level))
            return null;

        if (!ParseValidationLine(lines.ElementAtOrDefault(2) ?? string.Empty, level, out bool validationIsBlock))
            return null;

        int blockStartIndex = validationIsBlock ? 2 : 3;
        if (!ParseBlocks(lines.Length > blockStartIndex ? lines[blockStartIndex..] : Array.Empty<string>(), level))
            return null;

        return level;
    }

    private static bool ParseFirstLine(string line, CsvZeepLevel level)
    {
        try
        {
            string[] splits = line.Split(',');

            if (splits.Length < 3)
            {
                _logger.LogWarning("First line has invalid amount of splits");
                return false;
            }

            level.SceneName = splits[0];
            level.PlayerName = splits[1];
            level.UniqueId = splits[2];

            return true;
        }
        catch (Exception e)
        {
            _logger.LogError("Error while parsing first line");
            _logger.LogError(e);
            return false;
        }
    }

    private static bool ParseCameraLine(string line, CsvZeepLevel level)
    {
        try
        {
            string[] splits = NormalizeRowValues(line.Split(','), 8);

            level.CameraPosition = new Vector3(
                ParseDecimalOrDefault(splits[0]),
                ParseDecimalOrDefault(splits[1]),
                ParseDecimalOrDefault(splits[2]));

            level.CameraEuler = new Vector3(
                ParseDecimalOrDefault(splits[3]),
                ParseDecimalOrDefault(splits[4]),
                ParseDecimalOrDefault(splits[5]));

            level.CameraRotation = new Vector2(
                ParseDecimalOrDefault(splits[6]),
                ParseDecimalOrDefault(splits[7]));

            return true;
        }
        catch (Exception e)
        {
            _logger.LogError("Error while parsing camera line");
            _logger.LogError(e);
            return false;
        }
    }

    private static bool ParseValidationLine(string line, CsvZeepLevel level, out bool validationIsBlock)
    {
        try
        {
            string[] rawSplits = line.Split(',');
            validationIsBlock = rawSplits.Length >= 10;
            string[] splits = validationIsBlock
                ? new[] { "0", "0", "0", "0", "0", "0" }
                : NormalizeRowValues(rawSplits, 6);

            if (IsFiniteFloat(splits[0]))
            {
                level.IsValidated = true;
                level.ValidationTime = ParseFloatOrDefault(splits[0]);
            }
            else
            {
                level.IsValidated = false;
            }

            level.GoldTime = ParseFloatOrDefault(splits[1]);
            level.SilverTime = ParseFloatOrDefault(splits[2]);
            level.BronzeTime = ParseFloatOrDefault(splits[3]);

            level.Skybox = ParseIntOrDefault(splits[4]);
            level.Ground = ParseIntOrDefault(splits[5]);

            return true;
        }
        catch (Exception e)
        {
            validationIsBlock = false;
            _logger.LogError("Error while parsing validation line");
            _logger.LogError(e);
            return false;
        }
    }

    private static bool ParseBlocks(string[] lines, CsvZeepLevel level)
    {
        List<CsvZeepBlock> blocks = new();

        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            try
            {
                string[] splits = NormalizeBlockValues(line.Split(','));

                CsvZeepBlock block = new();

                block.Id = ParseIntOrDefault(splits[0]);

                block.Position = new Vector3(
                    ParseDecimalOrDefault(splits[1]),
                    ParseDecimalOrDefault(splits[2]),
                    ParseDecimalOrDefault(splits[3]));
                block.PositionSort = new CsvSortVector(
                    ParseDoubleOrDefault(splits[1]),
                    ParseDoubleOrDefault(splits[2]),
                    ParseDoubleOrDefault(splits[3]));
                block.RawPosition = FormatVectorParts(splits, 1);

                block.Euler = new Vector3(
                    ParseDecimalOrDefault(splits[4]),
                    ParseDecimalOrDefault(splits[5]),
                    ParseDecimalOrDefault(splits[6]));
                block.EulerSort = new CsvSortVector(
                    ParseDoubleOrDefault(splits[4]),
                    ParseDoubleOrDefault(splits[5]),
                    ParseDoubleOrDefault(splits[6]));
                block.RawEuler = FormatVectorParts(splits, 4);

                block.Scale = new Vector3(
                    ParseDecimalOrDefault(splits[7]),
                    ParseDecimalOrDefault(splits[8]),
                    ParseDecimalOrDefault(splits[9]));
                block.ScaleSort = new CsvSortVector(
                    ParseDoubleOrDefault(splits[7]),
                    ParseDoubleOrDefault(splits[8]),
                    ParseDoubleOrDefault(splits[9]));
                block.RawScale = FormatVectorParts(splits, 7);

                // Hackfix for the note block
                if (block.Id == 2279)
                {
                    splits[10..27].ToList()
                        .ForEach(x => block.Paints.Add((int)ParseFloatOrDefault(x)));
                }
                else
                {
                    splits[10..27].ToList().ForEach(x => block.Paints.Add(ParseIntOrDefault(x)));
                }

                splits[27..].ToList().ForEach(x => block.Options.Add(ParseOptionFloatOrDefault(x)));

                blocks.Add(block);
            }
            catch (Exception e)
            {
                _logger.LogError("Error while parsing block line");
                _logger.LogError(e);
                return false;
            }
        }

        level.Blocks = blocks;
        return true;
    }

    private static string[] NormalizeBlockValues(string[] values)
    {
        if (values.Length >= 38)
            return values;

        return NormalizeRowValues(values, 38);
    }

    private static string[] NormalizeRowValues(string[] values, int length)
    {
        if (values.Length >= length)
            return values[..length];

        string[] normalized = new string[length];
        Array.Copy(values, normalized, values.Length);
        for (int index = values.Length; index < normalized.Length; index++)
            normalized[index] = "0";
        return normalized;
    }

    private static decimal ParseDecimalOrDefault(string value)
    {
        return decimal.TryParse(value, NumberStyles.Any, _culture, out decimal parsed) ? parsed : 0m;
    }

    private static float ParseFloatOrDefault(string value)
    {
        if (!float.TryParse(value, NumberStyles.Any, _culture, out float parsed))
            return 0f;

        return float.IsNaN(parsed) || float.IsInfinity(parsed) ? 0f : parsed;
    }

    private static float ParseOptionFloatOrDefault(string value)
    {
        if (!double.TryParse(value, NumberStyles.Any, _culture, out double parsed))
            return 0f;

        return double.IsNaN(parsed) || double.IsInfinity(parsed) ? 0f : (float)parsed;
    }

    private static double ParseDoubleOrDefault(string value)
    {
        if (!double.TryParse(value, NumberStyles.Any, _culture, out double parsed))
            return 0d;

        return double.IsNaN(parsed) || double.IsInfinity(parsed) ? 0d : parsed;
    }

    private static bool IsFiniteFloat(string value)
    {
        return float.TryParse(value, NumberStyles.Any, _culture, out float parsed)
               && !float.IsNaN(parsed)
               && !float.IsInfinity(parsed);
    }

    private static int ParseIntOrDefault(string value)
    {
        return (int)ParseDecimalOrDefault(value);
    }

    private static string[] FormatVectorParts(string[] values, int offset)
    {
        return new[]
        {
            FormatDecimal(values[offset]),
            FormatDecimal(values[offset + 1]),
            FormatDecimal(values[offset + 2])
        };
    }

    private static string FormatDecimal(string value)
    {
        Match match = Regex.Match(
            value.Trim(),
            @"^([+-]?)(\d+)(?:\.(\d*))?(?:[eE]([+-]?\d+))?$",
            RegexOptions.CultureInvariant);
        if (!match.Success)
            return "0";

        string sign = match.Groups[1].Value == "-" ? "-" : string.Empty;
        string integer = match.Groups[2].Value;
        string fraction = match.Groups[3].Success ? match.Groups[3].Value : string.Empty;
        int exponent = match.Groups[4].Success ? int.Parse(match.Groups[4].Value, _culture) : 0;
        string digits = integer + fraction;
        int decimalIndex = integer.Length + exponent;

        string expanded;
        if (decimalIndex <= 0)
        {
            expanded = "0." + new string('0', -decimalIndex) + digits;
        }
        else if (decimalIndex >= digits.Length)
        {
            expanded = digits + new string('0', decimalIndex - digits.Length);
        }
        else
        {
            expanded = digits[..decimalIndex] + "." + digits[decimalIndex..];
        }

        string[] parts = expanded.Split('.');
        string expandedInteger = parts[0].TrimStart('0');
        if (expandedInteger.Length == 0)
            expandedInteger = "0";

        string normalized = parts.Length == 1 ? expandedInteger : expandedInteger + "." + parts[1];
        return Regex.IsMatch(normalized, @"^0(?:\.0*)?$", RegexOptions.CultureInvariant)
            ? normalized
            : sign + normalized;
    }
}
