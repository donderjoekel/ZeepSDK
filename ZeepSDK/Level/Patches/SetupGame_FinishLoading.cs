using System;
using HarmonyLib;
using JetBrains.Annotations;

namespace ZeepSDK.Level.Patches;

[HarmonyPatch(typeof(SetupGame), nameof(SetupGame.FinishLoading))]
internal class SetupGame_FinishLoading
{
    public static event Action<SetupGame> FinishLoading;

    [UsedImplicitly]
    private static void Prefix(SetupGame __instance)
    {
        FinishLoading?.Invoke(__instance);
    }
}