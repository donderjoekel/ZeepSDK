using UnityEngine;

namespace ZeepSDK.Messaging;

/// <summary>
/// A messenger that shows a tag when logging a message
/// </summary>
public interface ITaggedMessenger
{
    /// <summary>
    /// The tag that is shown
    /// </summary>
    public string Tag
    {
        get;
    }

    /// <summary>
    /// <see cref="MessengerApi.Log"/>
    /// </summary>
    void Log(string message, float duration = MessengerApi.DEFAULT_DURATION);

    /// <summary>
    /// <see cref="MessengerApi.LogSuccess"/>
    /// </summary>
    void LogSuccess(string message, float duration = MessengerApi.DEFAULT_DURATION);

    /// <summary>
    /// <see cref="MessengerApi.LogWarning"/>
    /// </summary>
    void LogWarning(string message, float duration = MessengerApi.DEFAULT_DURATION);

    /// <summary>
    /// <see cref="MessengerApi.LogError"/>
    /// </summary>
    void LogError(string message, float duration = MessengerApi.DEFAULT_DURATION);

    /// <summary>
    /// <see cref="MessengerApi.LogCustomColors"/>
    /// </summary>
    void LogCustomColors(
        string message,
        Color backgroundColor,
        Color textColor,
        float duration = MessengerApi.DEFAULT_DURATION
    );
}
