using System;
using HarmonyLib;

namespace ZeepSDK.UI.Patches;

[HarmonyPatch(typeof(OnlineChatUI), nameof(OnlineChatUI.Awake))]
internal class OnlineChatUI_Awake
{
    public static event Action<OnlineChatUI> Awake;

    private static void Postfix(OnlineChatUI __instance)
    {
        Awake?.Invoke(__instance);
    }
}
