using System;
using HarmonyLib;
using JetBrains.Annotations;
using Rewired;

namespace ZeepSDK.Controls.Patches;

[HarmonyPatch(typeof(InputPlayerScriptableObject),nameof(InputPlayerScriptableObject.EnableGameplayInput))]
internal class Input_EnableGameplayInput
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
