using System;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using UnityEngine;

namespace ZeepSDK.UI;

[PublicAPI]
public static class ZeepGUIUtility
{
    private static MethodInfo _unclipMethod;
    
    static ZeepGUIUtility()
    {
        Type guiClipType = typeof(GUIUtility).Assembly
            .GetTypes()
            .First(x => string.Equals(x.Name, "GUIClip", StringComparison.OrdinalIgnoreCase));

        MethodInfo[] methods = guiClipType.GetMethods(BindingFlags.Static | BindingFlags.Public);

        _unclipMethod = methods
            .Where(x => string.Equals(x.Name, "Unclip", StringComparison.OrdinalIgnoreCase))
            .First(x => x.GetParameters().FirstOrDefault()?.ParameterType == typeof(Vector2));
    }

    public static Rect ConvertToAbsolutePosition(Rect rect)
    {
        return new Rect(rect)
        {
            position = (Vector2)_unclipMethod.Invoke(null, [rect.position])
        };
    }
}
