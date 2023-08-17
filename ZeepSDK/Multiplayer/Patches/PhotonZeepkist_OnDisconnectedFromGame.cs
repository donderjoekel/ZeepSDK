using System;
using HarmonyLib;

namespace ZeepSDK.Multiplayer.Patches;

[HarmonyPatch(typeof(PhotonZeepkist), nameof(PhotonZeepkist_OnDisconnectedFromGame))]
internal class PhotonZeepkist_OnDisconnectedFromGame
{
    public static event Action DisconnectedFromGame;

    private static void Postfix()
    {
        DisconnectedFromGame?.Invoke();
    }
}
