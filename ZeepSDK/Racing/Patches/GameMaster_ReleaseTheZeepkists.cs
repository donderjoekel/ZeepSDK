using System;
using HarmonyLib;
using JetBrains.Annotations;

namespace ZeepSDK.Racing.Patches;

[HarmonyPatch(typeof(GameMaster), nameof(GameMaster.ReleaseTheZeepkists))]
internal class GameMaster_ReleaseTheZeepkists
{
    public static event Action Released;

    [UsedImplicitly]
    private static void Postfix()
    {
        Released?.Invoke();
    }
}
