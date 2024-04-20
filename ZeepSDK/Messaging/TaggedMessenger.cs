using System;
using UnityEngine;

namespace ZeepSDK.Messaging;

internal class TaggedMessenger : ITaggedMessenger
{
    public string Tag
    {
        get;
    }

    public TaggedMessenger(string tag)
    {
        Tag = tag;
    }

    public void Log(string message, float duration = MessengerApi.DEFAULT_DURATION)
    {
        MessengerApi.Log(FormatMessage(message), duration);
    }

    public void LogSuccess(string message, float duration = MessengerApi.DEFAULT_DURATION)
    {
        MessengerApi.LogSuccess(FormatMessage(message), duration);
    }

    public void LogWarning(string message, float duration = MessengerApi.DEFAULT_DURATION)
    {
        MessengerApi.LogWarning(FormatMessage(message), duration);
    }

    public void LogError(string message, float duration = MessengerApi.DEFAULT_DURATION)
    {
        MessengerApi.LogError(FormatMessage(message), duration);
    }

    public void LogCustomColors(
        string message,
        Color backgroundColor,
        Color textColor,
        float duration = MessengerApi.DEFAULT_DURATION
    )
    {
        MessengerApi.LogCustomColors(FormatMessage(message), backgroundColor, textColor, duration);
    }

    private string FormatMessage(string message)
    {
        return $"[{Tag}] {message}";
    }
}
