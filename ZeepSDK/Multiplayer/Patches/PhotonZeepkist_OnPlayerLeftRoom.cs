using System;
using HarmonyLib;
using ZeepkistClient;

namespace ZeepSDK.Multiplayer.Patches;

[HarmonyPatch(typeof(PhotonZeepkist), nameof(PhotonZeepkist.OnPlayerLeftRoom))]
internal class PhotonZeepkist_OnPlayerLeftRoom
{
    public static event Action<ZeepkistNetworkPlayer> PlayerLeftRoom;

    private static void Postfix(ZeepkistNetworkPlayer player)
    {
        PlayerLeftRoom?.Invoke(player);
    }
}
