using System;
using HarmonyLib;
using JetBrains.Annotations;
using Rewired;

namespace ZeepSDK.Controls.Patches;

[HarmonyPatch(typeof(InputPlayerScriptableObject),nameof(InputPlayerScriptableObject.DisableAllInput))]
internal class Input_DisableAllInput
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