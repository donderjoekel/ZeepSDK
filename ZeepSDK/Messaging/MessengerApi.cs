using JetBrains.Annotations;
using UnityEngine;

namespace ZeepSDK.Messaging;

/// <summary>
/// Show the user a message using the game's messenger
/// </summary>
[PublicAPI]
public static class MessengerApi
{
    /// <summary>
    /// The default duration for a log message
    /// </summary>
    public const float DEFAULT_DURATION = 2.5f;

    /// <summary>
    /// Creates a new tagged messenger
    /// </summary>
    /// <param name="tag">The tag to use for the messenger</param>
    public static ITaggedMessenger CreateTaggedMessenger(string tag)
    {
        return new TaggedMessenger(tag);
    }

    /// <summary>
    /// Log an information level message to the user
    /// </summary>
    /// <param name="message">The message to display</param>
    /// <param name="duration">The duration to display the message</param>
    public static void Log(string message, float duration = DEFAULT_DURATION)
    {
        PlayerManager.Instance.messenger.Log(message, duration);
    }

    /// <summary>
    /// Log a success level message to the user
    /// </summary>
    /// <param name="message">The message to display</param>
    /// <param name="duration">The duration to display the message</param>
    public static void LogSuccess(string message, float duration = DEFAULT_DURATION)
    {
        PlayerManager.Instance.messenger.LogCustomColor(message,
            duration,
            Color.white,
            new Color32(40, 167, 69, 255));
    }

    /// <summary>
    /// Log a warning level message to the user
    /// </summary>
    /// <param name="message">The message to display</param>
    /// <param name="duration">The duration to display the message</param>
    public static void LogWarning(string message, float duration = DEFAULT_DURATION)
    {
        PlayerManager.Instance.messenger.LogCustomColor(message,
            duration,
            new Color32(52, 58, 64, 255),
            new Color32(255, 193, 7, 255));
    }

    /// <summary>
    /// Log an error level message to the user
    /// </summary>
    /// <param name="message">The message to display</param>
    /// <param name="duration">The duration to display the message</param>
    public static void LogError(string message, float duration = DEFAULT_DURATION)
    {
        PlayerManager.Instance.messenger.LogError(message, duration);
    }

    /// <summary>
    /// Log a custom colored message to the user
    /// </summary>
    /// <param name="message">The message to display</param>
    /// <param name="textColor">The color to use for the text</param>
    /// <param name="backgroundColor">The color to use for the background</param>
    /// <param name="duration">The duration to display the message</param>
    public static void LogCustomColors(
        string message,
        Color textColor,
        Color backgroundColor,
        float duration = DEFAULT_DURATION
    )
    {
        PlayerManager.Instance.messenger.LogCustomColor(message, duration, textColor, backgroundColor);
    }
}
