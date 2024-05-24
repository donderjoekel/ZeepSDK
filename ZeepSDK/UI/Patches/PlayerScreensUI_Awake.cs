using System;
using HarmonyLib;

namespace ZeepSDK.UI.Patches;

[HarmonyPatch(typeof(PlayerScreensUI), nameof(PlayerScreensUI.Awake))]
internal class PlayerScreensUI_Awake
{
    public static event Action<PlayerScreensUI> Awake;
    
    private static void Postfix(PlayerScreensUI __instance)
    {
        Awake?.Invoke(__instance);
    }
}
