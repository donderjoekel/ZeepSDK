using UnityEngine;

namespace ZeepSDK.Extensions;

public static class EventExtensions
{
    public static bool IsKeyUp(this Event e, KeyCode key)
    {
        return e.type == EventType.KeyUp && e.keyCode == key;
    }
}
