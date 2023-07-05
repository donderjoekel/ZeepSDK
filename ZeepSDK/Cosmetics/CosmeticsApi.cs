using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using ZeepkistNetworking;

namespace ZeepSDK.Cosmetics;

/// <summary>
/// An api that allows you to get cosmetics
/// </summary>
[PublicAPI]
public static class CosmeticsApi
{
    private static CosmeticWardrobe Wardrobe => PlayerManager.Instance.objectsList.wardrobe;

    /// <summary>
    /// Gets the soapbox with the given id
    /// </summary>
    /// <param name="id">The id of the soapbox</param>
    /// <param name="checkForCheat">Should we check if the soapbox is unlocked?</param>
    /// <returns>A soapbox</returns>
    public static Object_Soapbox GetSoapbox(int id, bool checkForCheat = true)
    {
        return (Object_Soapbox)Wardrobe.GetCosmetic(CosmeticItemType.zeepkist, id, checkForCheat);
    }

    /// <summary>
    /// Gets the hat with the given id
    /// </summary>
    /// <param name="id">The id of the hat</param>
    /// <param name="checkForCheat">Should we check if the hat is unlocked?</param>
    /// <returns>A hat</returns>
    public static HatValues GetHat(int id, bool checkForCheat = true)
    {
        return (HatValues)Wardrobe.GetCosmetic(CosmeticItemType.hat, id, checkForCheat);
    }

    /// <summary>
    /// Gets the color with the given id
    /// </summary>
    /// <param name="id">The id of the color</param>
    /// <param name="checkForCheat">Should we check if the color is unlocked?</param>
    /// <returns>A color</returns>
    public static CosmeticColor GetColor(int id, bool checkForCheat = true)
    {
        return (CosmeticColor)Wardrobe.GetCosmetic(CosmeticItemType.skin, id, checkForCheat);
    }

    /// <summary>
    /// Gets all the soapboxes
    /// </summary>
    public static IReadOnlyList<Object_Soapbox> GetAllZeepkists()
    {
        return Wardrobe.everyZeepkist.Values.Cast<Object_Soapbox>().ToList();
    }

    /// <summary>
    /// Gets all the hats
    /// </summary>
    public static IReadOnlyList<HatValues> GetAllHats()
    {
        return Wardrobe.everyHat.Values.Cast<HatValues>().ToList();
    }

    /// <summary>
    /// Gets all the colors
    /// </summary>
    public static IReadOnlyList<CosmeticColor> GetAllColors()
    {
        return Wardrobe.everyColor.Values.Cast<CosmeticColor>().ToList();
    }

    /// <summary>
    /// Gets all the unlocked soapboxes
    /// </summary>
    public static IReadOnlyList<Object_Soapbox> GetUnlockedZeepkists()
    {
        return Wardrobe.unlockedZeepkist.Values.Cast<Object_Soapbox>().ToList();
    }
    
    /// <summary>
    /// Gets all unlocked hats
    /// </summary>
    public static IReadOnlyList<HatValues> GetUnlockedHats()
    {
        return Wardrobe.unlockedHat.Values.Cast<HatValues>().ToList();
    }
    
    /// <summary>
    /// Gets all unlocked colors
    /// </summary>
    public static IReadOnlyList<CosmeticColor> GetUnlockedColors()
    {
        return Wardrobe.unlockedColor.Values.Cast<CosmeticColor>().ToList();
    }
}
