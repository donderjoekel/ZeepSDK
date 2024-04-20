using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Logging;
using JetBrains.Annotations;
using ZeepkistNetworking;
using ZeepSDK.Utilities;

namespace ZeepSDK.Cosmetics;

/// <summary>
/// An api that allows you to get cosmetics
/// </summary>
[PublicAPI]
public static class CosmeticsApi
{
    private static readonly ManualLogSource _logger = LoggerFactory.GetLogger(typeof(CosmeticsApi));

    private static CosmeticWardrobe Wardrobe => PlayerManager.Instance.objectsList.wardrobe;

    /// <summary>
    /// Gets the soapbox with the given id
    /// </summary>
    /// <param name="id">The id of the soapbox</param>
    /// <param name="checkForCheat">Should we check if the soapbox is unlocked?</param>
    /// <returns>A soapbox</returns>
    public static Object_Soapbox GetSoapbox(int id, bool checkForCheat = true)
    {
        try
        {
            return (Object_Soapbox)Wardrobe.GetCosmetic(CosmeticItemType.zeepkist, id, checkForCheat);
        }
        catch (Exception e)
        {
            _logger.LogError($"Unhandled exception in {nameof(GetSoapbox)}: " + e);
            return null;
        }
    }

    /// <summary>
    /// Gets the hat with the given id
    /// </summary>
    /// <param name="id">The id of the hat</param>
    /// <param name="checkForCheat">Should we check if the hat is unlocked?</param>
    /// <returns>A hat</returns>
    public static HatValues GetHat(int id, bool checkForCheat = true)
    {
        try
        {
            return (HatValues)Wardrobe.GetCosmetic(CosmeticItemType.hat, id, checkForCheat);
        }
        catch (Exception e)
        {
            _logger.LogError($"Unhandled exception in {nameof(GetHat)}: " + e);
            return null;
        }
    }

    /// <summary>
    /// Gets the color with the given id
    /// </summary>
    /// <param name="id">The id of the color</param>
    /// <param name="checkForCheat">Should we check if the color is unlocked?</param>
    /// <returns>A color</returns>
    public static CosmeticColor GetColor(int id, bool checkForCheat = true)
    {
        try
        {
            return (CosmeticColor)Wardrobe.GetCosmetic(CosmeticItemType.skin, id, checkForCheat);
        }
        catch (Exception e)
        {
            _logger.LogError($"Unhandled exception in {nameof(GetColor)}: " + e);
            return null;
        }
    }

    /// <summary>
    /// Gets all the soapboxes
    /// </summary>
    public static IReadOnlyList<Object_Soapbox> GetAllZeepkists()
    {
        try
        {
            return Wardrobe.everyZeepkist.Values.Cast<Object_Soapbox>().ToList();
        }
        catch (Exception e)
        {
            _logger.LogError($"Unhandled exception in {nameof(GetAllZeepkists)}: " + e);
            return Array.Empty<Object_Soapbox>();
        }
    }

    /// <summary>
    /// Gets all the hats
    /// </summary>
    public static IReadOnlyList<HatValues> GetAllHats()
    {
        try
        {
            return Wardrobe.everyHat.Values.Cast<HatValues>().ToList();
        }
        catch (Exception e)
        {
            _logger.LogError($"Unhandled exception in {nameof(GetAllHats)}: " + e);
            return Array.Empty<HatValues>();
        }
    }

    /// <summary>
    /// Gets all the colors
    /// </summary>
    public static IReadOnlyList<CosmeticColor> GetAllColors()
    {
        try
        {
            return Wardrobe.everyColor.Values.Cast<CosmeticColor>().ToList();
        }
        catch (Exception e)
        {
            _logger.LogError($"Unhandled exception in {nameof(GetAllColors)}: " + e);
            return Array.Empty<CosmeticColor>();
        }
    }

    /// <summary>
    /// Gets all the unlocked soapboxes
    /// </summary>
    public static IReadOnlyList<Object_Soapbox> GetUnlockedZeepkists()
    {
        try
        {
            return Wardrobe.unlockedZeepkist.Values.Cast<Object_Soapbox>().ToList();
        }
        catch (Exception e)
        {
            _logger.LogError($"Unhandled exception in {nameof(GetUnlockedZeepkists)}: " + e);
            return Array.Empty<Object_Soapbox>();
        }
    }

    /// <summary>
    /// Gets all unlocked hats
    /// </summary>
    public static IReadOnlyList<HatValues> GetUnlockedHats()
    {
        try
        {
            return Wardrobe.unlockedHat.Values.Cast<HatValues>().ToList();
        }
        catch (Exception e)
        {
            _logger.LogError($"Unhandled exception in {nameof(GetUnlockedHats)}: " + e);
            return Array.Empty<HatValues>();
        }
    }

    /// <summary>
    /// Gets all unlocked colors
    /// </summary>
    public static IReadOnlyList<CosmeticColor> GetUnlockedColors()
    {
        try
        {
            return Wardrobe.unlockedColor.Values.Cast<CosmeticColor>().ToList();
        }
        catch (Exception e)
        {
            _logger.LogError($"Unhandled exception in {nameof(GetUnlockedColors)}: " + e);
            return Array.Empty<CosmeticColor>();
        }
    }
}
