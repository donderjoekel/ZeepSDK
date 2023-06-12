using System;
using HarmonyLib;
using JetBrains.Annotations;

namespace ZeepSDK.Leaderboard.Patches;

[HarmonyPatch(typeof(OnlineTabLeaderboardUI), nameof(OnlineTabLeaderboardUI.OnClose))]
internal class OnlineTabLeaderboardUI_OnClose
{
    public static event Action<OnlineTabLeaderboardUI> OnClose;

    [UsedImplicitly]
    private static bool Prefix(OnlineTabLeaderboardUI __instance)
    {
        OnClose?.Invoke(__instance);
        return false;
    }
}
