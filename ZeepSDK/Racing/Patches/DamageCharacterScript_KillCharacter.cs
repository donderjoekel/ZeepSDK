﻿using System;
using HarmonyLib;
using JetBrains.Annotations;

namespace ZeepSDK.Racing.Patches;

[HarmonyPatch(typeof(DamageCharacterScript), nameof(DamageCharacterScript.KillCharacter))]
internal class DamageCharacterScript_KillCharacter
{
    public static event Action<CrashReason> CharacterKilled;

    [UsedImplicitly]
    private static void Prefix(DamageCharacterScript __instance, DamageCharacterScript.DeathReasonEnum deathReason)
    {
        if (__instance.IsDead())
            return;

        CrashReason reason = deathReason switch
        {
            DamageCharacterScript.DeathReasonEnum.Gameplay_Crashed => CrashReason.Crashed,
            DamageCharacterScript.DeathReasonEnum.Gameplay_Death_Eye => CrashReason.Eye,
            DamageCharacterScript.DeathReasonEnum.Gameplay_Death_Ghost => CrashReason.Ghost,
            DamageCharacterScript.DeathReasonEnum.Gameplay_Death_Sticky => CrashReason.Sticky,
            DamageCharacterScript.DeathReasonEnum.Gameplay_Death_FoundFootage => CrashReason.FoundFootage,
            _ => CrashReason.Unknown
        };

        CharacterKilled?.Invoke(reason);
    }
}
