using System;
using HarmonyLib;

namespace ZeepSDK.LevelEditor.Patches;

[HarmonyPatch(typeof(LEV_LevelEditorCentral), nameof(LEV_LevelEditorCentral.OnDestroy))]
internal class LEV_LevelEditorCentral_OnDestroy
{
    public static event Action PostfixEvent;

    private static void Postfix()
    {
        PostfixEvent?.Invoke();
    }
}
