using System;
using HarmonyLib;

namespace ZeepSDK.Multiplayer.Patches;

[HarmonyPatch(typeof(PhotonZeepkist), nameof(PhotonZeepkist.OnCreatedRoom))]
internal class PhotonZeepkist_OnCreatedRoom
{
    public static event Action CreatedRoom;

    private static void Postfix()
    {
        CreatedRoom?.Invoke();
    }
}
