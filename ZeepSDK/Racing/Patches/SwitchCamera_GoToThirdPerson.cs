using System;
using HarmonyLib;
using JetBrains.Annotations;

namespace ZeepSDK.Racing.Patches;

[HarmonyPatch(typeof(SwitchCamera), nameof(SwitchCamera.GoToThirdPerson))]
internal class SwitchCamera_GoToThirdPerson
{
    public static event Action EnteredThirdPerson;

    [UsedImplicitly]
    private static void Postfix()
    {
        EnteredThirdPerson?.Invoke();
    }
}
