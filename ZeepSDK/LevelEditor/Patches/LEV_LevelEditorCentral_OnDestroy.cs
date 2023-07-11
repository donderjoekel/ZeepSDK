using System;
using HarmonyLib;
using JetBrains.Annotations;

namespace ZeepSDK.LevelEditor.Patches;

[HarmonyPatch(typeof(LEV_LevelEditorCentral), nameof(LEV_LevelEditorCentral.OnDestroy))]
internal class LEV_LevelEditorCentral_OnDestroy
{
    public static event Action PostfixEvent;

    [UsedImplicitly]
    private static void Postfix()
    {
        PostfixEvent?.Invoke();
    }
}
