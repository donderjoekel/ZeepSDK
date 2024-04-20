using System;
using HarmonyLib;
using JetBrains.Annotations;

namespace ZeepSDK.LevelEditor.Patches;

[HarmonyPatch(typeof(LEV_SaveLoad), nameof(LEV_SaveLoad.ExternalLoad))]
internal class LEV_SaveLoad_ExternalLoad
{
    public static event Action PostfixEvent;

    [UsedImplicitly]
    private static void Postfix(bool useV15loading, bool retainUndoList)
    {
        if (!useV15loading && !retainUndoList)
        {
            PostfixEvent?.Invoke();
        }
    }
}
