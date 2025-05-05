using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
        if (!ParseFirstLine(lines[0], level))
            return null;

        if (!ParseCameraLine(lines[1], level))
            return null;

        if (!ParseValidationLine(lines[2], level))
            return null;

        if (!ParseBlocks(lines[3..], level))
            return null;

        return level;
    }

    private static bool ParseFirstLine(string line, CsvZeepLevel level)
    {
        try
        {
            string[] splits = line.Split(',');

            if (splits.Length != 3)
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
            string[] splits = line.Split(',');

            if (splits.Length != 8)
            {
                _logger.LogWarning("Camera line has invalid amount of splits");
                return false;
            }

            level.CameraPosition = new Vector3(
                decimal.Parse(splits[0], NumberStyles.Any, _culture),
                decimal.Parse(splits[1], NumberStyles.Any, _culture),
                decimal.Parse(splits[2], NumberStyles.Any, _culture));

            level.CameraEuler = new Vector3(
                decimal.Parse(splits[3], NumberStyles.Any, _culture),
                decimal.Parse(splits[4], NumberStyles.Any, _culture),
                decimal.Parse(splits[5], NumberStyles.Any, _culture));

            level.CameraRotation = new Vector2(
                decimal.Parse(splits[6], NumberStyles.Any, _culture),
                decimal.Parse(splits[7], NumberStyles.Any, _culture));

            return true;
        }
        catch (Exception e)
        {
            _logger.LogError("Error while parsing camera line");
            _logger.LogError(e);
            return false;
        }
    }

    private static bool ParseValidationLine(string line, CsvZeepLevel level)
    {
        try
        {
            string[] splits = line.Split(',');

            if (splits.Length != 6)
            {
                _logger.LogWarning("Validation line has invalid amount of splits");
                return false;
            }

            if (float.TryParse(splits[0], NumberStyles.Any, _culture, out float validationTime))
            {
                level.IsValidated = true;
                level.ValidationTime = validationTime;
            }
            else
            {
                level.IsValidated = false;
            }

            level.GoldTime = float.Parse(splits[1], NumberStyles.Any, _culture);
            level.SilverTime = float.Parse(splits[2], NumberStyles.Any, _culture);
            level.BronzeTime = float.Parse(splits[3], NumberStyles.Any, _culture);

            level.Skybox = int.Parse(splits[4], NumberStyles.Any, _culture);
            level.Ground = int.Parse(splits[5], NumberStyles.Any, _culture);

            return true;
        }
        catch (Exception e)
        {
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
                string[] splits = line.Split(',');
                if (splits.Length != 38)
                {
                    _logger.LogError(
                        $"Unable to parse block line, got {splits.Length} splits, expected 38. UID: '{level.UniqueId}'. Line: '{line}'");
                    return false;
                }

                CsvZeepBlock block = new();

                block.Id = int.Parse(splits[0], NumberStyles.Any, _culture);

                block.Position = new Vector3(
                    decimal.Parse(splits[1], NumberStyles.Any, _culture),
                    decimal.Parse(splits[2], NumberStyles.Any, _culture),
                    decimal.Parse(splits[3], NumberStyles.Any, _culture));

                block.Euler = new Vector3(
                    decimal.Parse(splits[4], NumberStyles.Any, _culture),
                    decimal.Parse(splits[5], NumberStyles.Any, _culture),
                    decimal.Parse(splits[6], NumberStyles.Any, _culture));

                block.Scale = new Vector3(
                    decimal.Parse(splits[7], NumberStyles.Any, _culture),
                    decimal.Parse(splits[8], NumberStyles.Any, _culture),
                    decimal.Parse(splits[9], NumberStyles.Any, _culture));

                // Hackfix for the note block
                if (block.Id == 2279)
                {
                    splits[10..27].ToList()
                        .ForEach(x => block.Paints.Add((int)float.Parse(x, NumberStyles.Any, _culture)));
                }
                else
                {
                    splits[10..27].ToList().ForEach(x => block.Paints.Add(int.Parse(x, NumberStyles.Any, _culture)));
                }

                splits[27..38].ToList().ForEach(x => block.Options.Add(float.Parse(x, NumberStyles.Any, _culture)));

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
}
