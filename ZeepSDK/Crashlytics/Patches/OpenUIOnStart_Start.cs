using System;
using HarmonyLib;
using JetBrains.Annotations;

namespace ZeepSDK.BugReporting.Patches;

[HarmonyPatch(typeof(OpenUIOnStart), nameof(OpenUIOnStart.Start))]
internal class OpenUIOnStart_Start
{
    public static event Action<OpenUIOnStart> Postfixed;
    
    [UsedImplicitly]
    private static void Postfix(OpenUIOnStart __instance)
    {
        Postfixed?.Invoke(__instance);
    }
}
