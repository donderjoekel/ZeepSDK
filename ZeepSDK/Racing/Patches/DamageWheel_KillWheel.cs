using System;
using HarmonyLib;

namespace ZeepSDK.Racing.Patches;

[HarmonyPatch(typeof(DamageWheel), nameof(DamageWheel.KillWheel))]
internal class DamageWheel_KillWheel
{
    public static event Action KillWheel;

    private static void Prefix(DamageWheel __instance)
    {
        if (__instance.isdead)
            return;

        KillWheel?.Invoke();
    }
}
