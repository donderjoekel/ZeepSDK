using Imui.Core;

namespace ZeepSDK.Settings.Drawers;

/// <summary>
/// Draws a single element in the mod settings panel.
/// </summary>
public interface IZeepSettingsDrawer
{
    /// <summary>
    /// Draws this settings element using the given ImGui instance.
    /// </summary>
    /// <param name="gui">The ImGui instance to draw with.</param>
    /// <param name="context">Shared services available while drawing settings.</param>
    void Draw(ImGui gui, ZeepSettingsDrawContext context);
}
