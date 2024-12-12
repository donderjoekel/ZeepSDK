using JetBrains.Annotations;
using UnityEngine;
using ZeepSDK.Scripting.Attributes;
using ZeepSDK.Utilities;

namespace ZeepSDK.Messaging;

/// <summary>
/// Show the user a message using the game's messenger
/// </summary>
[PublicAPI]
public static class MessengerApi
{
    private static Sprite _cachedErrorSprite;
    private static Sprite _cachedSuccessSprite;
    private static Sprite _cachedNeutralSprite;

    private static Sprite ErrorSprite
    {
        get
        {
            if (_cachedErrorSprite != null)
                return _cachedErrorSprite;

            _cachedErrorSprite = SpriteUtility.FromBase64(MessengerImages.ERROR);
            return _cachedErrorSprite;
        }
    }

    private static Sprite SuccessSprite
    {
        get
        {
            if (_cachedSuccessSprite != null)
                return _cachedSuccessSprite;

            _cachedSuccessSprite = SpriteUtility.FromBase64(MessengerImages.SUCCESS);
            return _cachedSuccessSprite;
        }
    }

    private static Sprite NeutralSprite
    {
        get
        {
            if (_cachedNeutralSprite != null)
                return _cachedNeutralSprite;

            _cachedNeutralSprite = SpriteUtility.FromBase64(MessengerImages.NEUTRAL);
            return _cachedNeutralSprite;
        }
    }

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
    [GenerateFunction]
    public static void Log(string message, float duration = DEFAULT_DURATION)
    {
        PlayerManager.Instance.messenger.Log(message, duration);
    }

    /// <summary>
    /// Log a success level message to the user
    /// </summary>
    /// <param name="message">The message to display</param>
    /// <param name="duration">The duration to display the message</param>
    [GenerateFunction]
    public static void LogSuccess(string message, float duration = DEFAULT_DURATION)
    {
        PlayerManager.Instance.messenger.LogCustomColor(
            message,
            duration,
            Color.white,
            new Color32(40, 167, 69, 255));
        PlayerManager.Instance.messenger.frog.sprite = SuccessSprite;
    }

    /// <summary>
    /// Log a warning level message to the user
    /// </summary>
    /// <param name="message">The message to display</param>
    /// <param name="duration">The duration to display the message</param>
    [GenerateFunction]
    public static void LogWarning(string message, float duration = DEFAULT_DURATION)
    {
        PlayerManager.Instance.messenger.LogCustomColor(
            message,
            duration,
            new Color32(52, 58, 64, 255),
            new Color32(255, 193, 7, 255));
        PlayerManager.Instance.messenger.frog.sprite = NeutralSprite;
    }

    /// <summary>
    /// Log an error level message to the user
    /// </summary>
    /// <param name="message">The message to display</param>
    /// <param name="duration">The duration to display the message</param>
    [GenerateFunction]
    public static void LogError(string message, float duration = DEFAULT_DURATION)
    {
        PlayerManager.Instance.messenger.LogError(message, duration);
        PlayerManager.Instance.messenger.frog.sprite = ErrorSprite;
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
