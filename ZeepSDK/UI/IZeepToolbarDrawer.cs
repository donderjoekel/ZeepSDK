using Imui.Core;

namespace ZeepSDK.UI;

/// <summary>
/// Interface for drawers that add menu items to the Zeep toolbar
/// </summary>
public interface IZeepToolbarDrawer
{
    /// <summary>
    /// Gets the title of the menu that will appear in the toolbar
    /// </summary>
    string MenuTitle { get; }
    
    /// <summary>
    /// Called to draw the menu items for this toolbar drawer
    /// </summary>
    /// <param name="gui">The ImGui instance to use for drawing</param>
    void DrawMenuItems(ImGui gui);
}