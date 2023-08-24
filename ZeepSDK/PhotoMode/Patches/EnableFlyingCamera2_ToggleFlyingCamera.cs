using System;
using HarmonyLib;

namespace ZeepSDK.PhotoMode.Patches;

[HarmonyPatch(typeof(EnableFlyingCamera2), nameof(EnableFlyingCamera2.ToggleFlyingCamera), new Type[] { typeof(bool) })]
internal class EnableFlyingCamera2_ToggleFlyingCamera
{
    public static event Action PhotoModeEntered;
    public static event Action PhotoModeExited;
    
    private static void Prefix(EnableFlyingCamera2 __instance, out bool __state)
    {
        __state = __instance.isPhotoMode;
    }

    private static void Postfix(EnableFlyingCamera2 __instance, bool __state)
    {
        if (__state == __instance.isPhotoMode)
            return;

        if (__instance.isPhotoMode)
        {
            PhotoModeEntered?.Invoke();
        }
        else
        {
            PhotoModeExited?.Invoke();
        }
    }
}
