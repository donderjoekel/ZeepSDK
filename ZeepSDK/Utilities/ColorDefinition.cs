using UnityEngine;

namespace ZeepSDK.Utilities;

/// <summary>
/// A definition of a color with a name and a value
/// </summary>
public class ColorDefinition
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="color"></param>
    public ColorDefinition(string name, Color color)
    {
        Name = name;
        Color = color;
    }

    /// <summary>
    /// The display name of the color
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// The value representing the color
    /// </summary>
    public Color Color { get; }
}
