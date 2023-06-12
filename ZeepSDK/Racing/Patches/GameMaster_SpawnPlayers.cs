using System;
using HarmonyLib;
using JetBrains.Annotations;

namespace ZeepSDK.Racing.Patches;

[HarmonyPatch(typeof(GameMaster), nameof(GameMaster.SpawnPlayers))]
internal class GameMaster_SpawnPlayers
{
    public static event Action SpawnPlayers;

    [UsedImplicitly]
    private static void Postfix()
    {
        SpawnPlayers?.Invoke();
    }
}
