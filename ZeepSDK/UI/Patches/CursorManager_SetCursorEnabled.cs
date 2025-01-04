using System;
using HarmonyLib;
using JetBrains.Annotations;

namespace ZeepSDK.UI.Patches;

[HarmonyPatch(typeof(CursorManager), nameof(CursorManager.SetCursorEnabled))]
internal class CursorManager_SetCursorEnabled
{
    public static Action Invoked;

    [UsedImplicitly]
    private static void Postfix()
    {
        Invoked?.Invoke();
    }
}
