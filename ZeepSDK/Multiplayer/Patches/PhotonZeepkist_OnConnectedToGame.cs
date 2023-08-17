using System;
using HarmonyLib;

namespace ZeepSDK.Multiplayer.Patches;

[HarmonyPatch(typeof(PhotonZeepkist), nameof(PhotonZeepkist.OnConnectedToGame))]
internal class PhotonZeepkist_OnConnectedToGame
{
    public static event Action ConnectedToGame;

    private static void Prefix()
    {
        ConnectedToGame?.Invoke();
    }
}
