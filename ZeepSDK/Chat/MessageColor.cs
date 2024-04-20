using System.Globalization;

namespace ZeepSDK.Chat;

/// <summary>
/// The available colors for messages
/// </summary>
public enum MessageColor
{
    /// <summary>
    /// 
    /// </summary>
    Red,

    /// <summary>
    /// 
    /// </summary>
    Orange,

    /// <summary>
    /// 
    /// </summary>
    Yellow,

    /// <summary>
    /// 
    /// </summary>
    Blue,

    /// <summary>
    /// 
    /// </summary>
    Green,

    /// <summary>
    /// 
    /// </summary>
    Pink,

    /// <summary>
    /// 
    /// </summary>
    Purple,

    /// <summary>
    /// 
    /// </summary>
    Black,

    /// <summary>
    /// 
    /// </summary>
    White
}

/// <summary>
/// Extensions for the MessageColor enum
/// </summary>
public static class MessageColorExtensions
{
    /// <summary>
    /// Returns a valid string representation of the color
    /// </summary>
    /// <param name="color">The color to convert</param>
    public static string ToValidString(this MessageColor color)
    {
#pragma warning disable CA1308 // Ignoring here because we specifically need lowercase
        return color.ToString().ToLowerInvariant();
#pragma warning restore CA1308
    }
}
