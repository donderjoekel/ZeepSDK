using System;
using HarmonyLib;
using JetBrains.Annotations;

namespace ZeepSDK.Leaderboard.Patches;

[HarmonyPatch(typeof(OnlineTabLeaderboardUI), nameof(OnlineTabLeaderboardUI.Update))]
internal class OnlineTabLeaderboardUI_Update
{
    public static event Action<OnlineTabLeaderboardUI> Update;

    [UsedImplicitly]
    private static bool Prefix(OnlineTabLeaderboardUI __instance)
    {
        Update?.Invoke(__instance);
        return false;
    }
}
