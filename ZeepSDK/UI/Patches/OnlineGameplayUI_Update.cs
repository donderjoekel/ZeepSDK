using System;
using HarmonyLib;
using TMPro;
using UnityEngine;

[HarmonyPatch(typeof(OnlineGameplayUI), "Update")]
public class OnlineGameplayUI_Update
{
    public static event Action<OnlineGameplayUI> Update;

    private static void Postfix(OnlineGameplayUI __instance)
    {
        Update?.Invoke(__instance);
    }
}
