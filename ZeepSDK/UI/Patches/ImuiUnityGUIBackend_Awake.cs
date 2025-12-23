using System;
using HarmonyLib;
using Imui.IO.UGUI;
using JetBrains.Annotations;

namespace ZeepSDK.UI.Patches;

[HarmonyPatch(typeof(ImuiUnityGUIBackend), nameof(ImuiUnityGUIBackend.Awake))]
internal class ImuiUnityGUIBackend_Awake
{
    public static event Action<ImuiUnityGUIBackend> Awake;
    
    [UsedImplicitly]
    private static void Postfix(ImuiUnityGUIBackend __instance)
    {
        Awake?.Invoke(__instance);
    }
}