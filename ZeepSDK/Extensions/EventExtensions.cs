using UnityEngine;

namespace ZeepSDK.Extensions;

/// <summary>
/// Provides extension methods for Unity's <see cref="Event"/> class.
/// </summary>
public static class EventExtensions
{
    /// <summary>
    /// Determines whether the event represents a key up event for the specified key.
    /// </summary>
    /// <param name="e">The Unity event to check.</param>
    /// <param name="key">The key code to check for.</param>
    /// <returns><c>true</c> if the event type is <see cref="EventType.KeyUp"/> and the key code matches; otherwise, <c>false</c>.</returns>
    public static bool IsKeyUp(this Event e, KeyCode key)
    {
        return e.type == EventType.KeyUp && e.keyCode == key;
    }
}
