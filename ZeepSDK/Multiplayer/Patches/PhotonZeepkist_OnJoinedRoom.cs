using System;
using HarmonyLib;

namespace ZeepSDK.Multiplayer.Patches;

[HarmonyPatch(typeof(PhotonZeepkist), nameof(PhotonZeepkist.OnJoinedRoom))]
internal class PhotonZeepkist_OnJoinedRoom
{
    public static event Action JoinedRoom;

    private static void Postfix()
    {
        JoinedRoom?.Invoke();
    }
}
