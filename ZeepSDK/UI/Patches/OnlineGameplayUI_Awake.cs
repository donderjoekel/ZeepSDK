using System;
using HarmonyLib;

namespace ZeepSDK.UI.Patches;

[HarmonyPatch(typeof(OnlineGameplayUI), nameof(OnlineGameplayUI.Awake))]
internal class OnlineGameplayUI_Awake
{
    public static event Action<OnlineGameplayUI> Awake;

    private static void Postfix(OnlineGameplayUI __instance)
    {
        Awake?.Invoke(__instance);
    }
}
