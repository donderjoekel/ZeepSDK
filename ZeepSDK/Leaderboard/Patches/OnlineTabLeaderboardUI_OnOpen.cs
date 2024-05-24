using System;
using HarmonyLib;
using JetBrains.Annotations;

namespace ZeepSDK.Leaderboard.Patches;

[HarmonyPatch(typeof(OnlineTabLeaderboardUI), nameof(OnlineTabLeaderboardUI.OnOpen))]
internal class OnlineTabLeaderboardUI_OnOpen
{
    public static event Action<OnlineTabLeaderboardUI> OnOpen;

    [UsedImplicitly]
    private static bool Prefix(OnlineTabLeaderboardUI __instance)
    {
        OnOpen?.Invoke(__instance);
        return false;
    }
}
