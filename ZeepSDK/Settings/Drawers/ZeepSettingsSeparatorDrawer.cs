using Imui.Controls;
using Imui.Core;

namespace ZeepSDK.Settings.Drawers;

/// <summary>
/// Draws a horizontal separator between settings entries.
/// </summary>
public readonly struct ZeepSettingsSeparatorDrawer : IZeepSettingsDrawer
{
    /// <inheritdoc />
    public void Draw(ImGui gui, ZeepSettingsDrawContext context)
    {
        using (gui.Indent())
        {
            gui.AddSpacing(gui.Style.Layout.Spacing * 2);
            gui.Separator();
            gui.AddSpacing(gui.Style.Layout.Spacing * 2);
        }
    }
}
