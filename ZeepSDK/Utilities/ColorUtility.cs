using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ZeepSDK.Utilities;

/// <summary>
/// A class containing utilities that are related to <see cref="Color"/>
/// </summary>
public static class ColorUtility
{
    private static readonly HashSet<ColorDefinition> colorDefinitions =
    [
        new ColorDefinition("Black", Color.black),
        new ColorDefinition("Blue", Color.blue),
        new ColorDefinition("Cyan", Color.cyan),
        new ColorDefinition("Gray", Color.gray),
        new ColorDefinition("Green", Color.green),
        new ColorDefinition("Magenta", Color.magenta),
        new ColorDefinition("Red", Color.red),
        new ColorDefinition("White", Color.white),
        new ColorDefinition("Yellow", Color.yellow)
    ];

    /// <summary>
    /// All supported color definitions
    /// </summary>
    public static IEnumerable<ColorDefinition> ColorDefinitions => colorDefinitions;

    /// <summary>
    /// Tries to get a color from a name
    /// </summary>
    /// <param name="name">The name of the color</param>
    /// <returns>Either the color found by the name, or white</returns>
    public static Color FromName(string name)
    {
        return colorDefinitions.FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase))
            ?.Color ?? Color.white;
    }
}
