using System;
using HarmonyLib;
using JetBrains.Annotations;

namespace ZeepSDK.UI.Patches;

[HarmonyPatch(typeof(OnlineChatUI), nameof(OnlineChatUI.Awake))]
internal class OnlineChatUI_Awake
{
    public static event Action<OnlineChatUI> Awake;

    [UsedImplicitly]
    private static void Postfix(OnlineChatUI __instance)
    {
        Awake?.Invoke(__instance);
    }
}
