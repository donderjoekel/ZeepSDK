using System;
using HarmonyLib;
using JetBrains.Annotations;

namespace ZeepSDK.LevelEditor.Patches;

[HarmonyPatch(typeof(LEV_SaveLoad), nameof(LEV_SaveLoad.ExternalSaveFile))]
internal class LEV_SaveLoad_ExternalSaveFile
{
    public static event Action PostfixEvent;

    [UsedImplicitly]
    private static void HandlePostfix(bool isTestMap)
    {
        if (!isTestMap)
        {
            PostfixEvent?.Invoke();
        }
    }
}
