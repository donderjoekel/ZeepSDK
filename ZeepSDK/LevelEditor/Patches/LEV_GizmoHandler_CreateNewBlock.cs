using HarmonyLib;
using JetBrains.Annotations;

namespace ZeepSDK.LevelEditor.Patches;

[HarmonyPatch(typeof(LEV_GizmoHandler), nameof(LEV_GizmoHandler.CreateNewBlock))]
internal class LEV_GizmoHandler_CreateNewBlock
{
    [UsedImplicitly]
    private static bool Prefix(int blockID)
    {
        return !CustomBlockCallbackRegistry.TryInvoke(blockID);
    }
}
