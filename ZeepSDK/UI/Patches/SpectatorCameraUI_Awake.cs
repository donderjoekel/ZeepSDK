using System;
using HarmonyLib;

namespace ZeepSDK.UI.Patches;

[HarmonyPatch(typeof(SpectatorCameraUI), nameof(SpectatorCameraUI.Awake))]
internal class SpectatorCameraUI_Awake
{
    public static event Action<SpectatorCameraUI> Awake;
    
    private static void Postfix(SpectatorCameraUI __instance)
    {
        Awake?.Invoke(__instance);
    }
}
