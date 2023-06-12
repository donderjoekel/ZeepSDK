using System;
using HarmonyLib;
using JetBrains.Annotations;

namespace ZeepSDK.LevelEditor.Patches;

[HarmonyPatch(typeof(LEV_Inspector), nameof(LEV_Inspector.Awake))]
internal class LEV_Inspector_Awake
{
    public static event Action<LEV_Inspector> Awake;

    [UsedImplicitly]
    private static void Postfix(LEV_Inspector __instance)
    {
        Awake?.Invoke(__instance);
    }
}
