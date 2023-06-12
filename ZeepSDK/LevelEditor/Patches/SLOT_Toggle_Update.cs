using HarmonyLib;
using JetBrains.Annotations;

namespace ZeepSDK.LevelEditor.Patches;

[HarmonyPatch(typeof(SLOT_Toggle), nameof(SLOT_Toggle.Update))]
internal class SLOT_Toggle_Update
{
    [UsedImplicitly]
    public static bool Prefix()
    {
        return !LevelEditorApi.IsKeyboardInputBlocked;
    }
}
