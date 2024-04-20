using HarmonyLib;
using JetBrains.Annotations;

namespace ZeepSDK.LevelEditor.Patches;

[HarmonyPatch(typeof(LEV_MoveCamera), nameof(LEV_MoveCamera.IsCursorInGameView))]
internal class LEV_MoveCamera_IsCursorInGameView
{
    [UsedImplicitly]
    public static void Postfix(ref bool __result)
    {
        __result = __result && !LevelEditorApi.IsMouseInputBlocked;
    }
}
