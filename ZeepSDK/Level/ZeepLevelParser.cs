using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using BepInEx.Logging;
using ZeepSDK.Utilities;

namespace ZeepSDK.Level;

internal static class ZeepLevelParser
{
    private static readonly ManualLogSource _logger = LoggerFactory.GetLogger(typeof(ZeepLevelParser));

    public static ZeepLevel Parse(string[] lines)
    {
        if (lines.Length == 0)
        {
            _logger.LogWarning("Trying to parse an empty level");
            return null;
        }

        ZeepLevel level = new();
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

    private static bool ParseFirstLine(string line, ZeepLevel level)
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

    private static bool ParseCameraLine(string line, ZeepLevel level)
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
                float.Parse(splits[0]),
                float.Parse(splits[1]),
                float.Parse(splits[2]));

            level.CameraEuler = new Vector3(
                float.Parse(splits[3]),
                float.Parse(splits[4]),
                float.Parse(splits[5]));

            level.CameraRotation = new Vector2(
                float.Parse(splits[6]),
                float.Parse(splits[7]));

            return true;
        }
        catch (Exception e)
        {
            _logger.LogError("Error while parsing camera line");
            _logger.LogError(e);
            return false;
        }
    }

    private static bool ParseValidationLine(string line, ZeepLevel level)
    {
        try
        {
            string[] splits = line.Split(',');

            if (splits.Length != 6)
            {
                _logger.LogWarning("Validation line has invalid amount of splits");
                return false;
            }

            if (float.TryParse(splits[0], out float validationTime))
            {
                level.IsValidated = true;
                level.ValidationTime = validationTime;
            }
            else
            {
                level.IsValidated = false;
            }

            level.GoldTime = float.Parse(splits[1]);
            level.SilverTime = float.Parse(splits[2]);
            level.BronzeTime = float.Parse(splits[3]);

            level.Skybox = int.Parse(splits[4]);
            level.Ground = int.Parse(splits[5]);

            return true;
        }
        catch (Exception e)
        {
            _logger.LogError("Error while parsing validation line");
            _logger.LogError(e);
            return false;
        }
    }

    private static bool ParseBlocks(string[] lines, ZeepLevel level)
    {
        List<ZeepBlock> blocks = new();

        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            try
            {
                string[] splits = line.Split(',');
                if (splits.Length != 38)
                {
                    _logger.LogWarning($"Block line has invalid amount of splits: {splits.Length}; '{line}'");
                    return false;
                }

                ZeepBlock block = new();

                block.Id = int.Parse(splits[0]);

                block.Position = new Vector3(
                    float.Parse(splits[1]),
                    float.Parse(splits[2]),
                    float.Parse(splits[3]));

                block.Euler = new Vector3(
                    float.Parse(splits[4]),
                    float.Parse(splits[5]),
                    float.Parse(splits[6]));

                block.Scale = new Vector3(
                    float.Parse(splits[7]),
                    float.Parse(splits[8]),
                    float.Parse(splits[9]));

                splits[10..27].ToList().ForEach(x => block.Paints.Add(int.Parse(x)));

                splits[27..38].ToList().ForEach(x => block.Options.Add(float.Parse(x)));

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
