﻿using System;
using HarmonyLib;

namespace ZeepSDK.Racing.Patches;

[HarmonyPatch(typeof(GameMaster), nameof(GameMaster.StartLevelFirstTime))]
internal class GameMaster_StartLevelFirstTime
{
    public static event Action StartLevelFirstTime;

    private static void Postfix()
    {
        StartLevelFirstTime?.Invoke();
    }
}
