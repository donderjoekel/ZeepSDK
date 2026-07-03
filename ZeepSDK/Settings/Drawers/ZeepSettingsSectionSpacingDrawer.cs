using Imui.Controls;
using Imui.Core;

namespace ZeepSDK.Settings.Drawers;

/// <summary>
/// Draws trailing spacing after a config section.
/// </summary>
internal class ZeepSettingsSectionSpacingDrawer : IZeepSettingsDrawer
{
    /// <inheritdoc />
    public void Draw(ImGui gui, ZeepSettingsDrawContext context)
    {
        using (gui.Indent())
            gui.AddSpacing();
    }
}
