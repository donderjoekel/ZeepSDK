using System;
using HarmonyLib;
using JetBrains.Annotations;

namespace ZeepSDK.Racing.Patches;

[HarmonyPatch(typeof(PauseMenuUI), nameof(PauseMenuUI.OnQuit))]
internal class PauseMenuUI_OnQuit
{
    public static event Action OnQuit;

    [UsedImplicitly]
    private static void Prefix()
    {
        OnQuit?.Invoke();
    }
}
