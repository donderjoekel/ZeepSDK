using System;
using HarmonyLib;
using JetBrains.Annotations;
using Rewired;

namespace ZeepSDK.Controls.Patches;

[HarmonyPatch(typeof(InputPlayerScriptableObject),nameof(InputPlayerScriptableObject.DisableLevelEditorInput))]
internal class Input_DisableLevelEditorInput
{
    public static Action Invoked;
    
    [UsedImplicitly]
    public static void Postfix()
    {
        if (ReInput.isReady)
        {
            Invoked?.Invoke();
        }
    }
}
