using System;
using HarmonyLib;
using JetBrains.Annotations;

namespace ZeepSDK.Racing.Patches;

[HarmonyPatch(typeof(GameMaster), nameof(GameMaster.RestartLevel))]
internal class GameMaster_RestartLevel
{
    public static event Action RestartLevel;

    [UsedImplicitly]
    private static void Prefix()
    {
        RestartLevel?.Invoke();
    }
}
