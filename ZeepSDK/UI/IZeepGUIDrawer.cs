using Imui.Core;

namespace ZeepSDK.UI;

/// <summary>
/// Interface for drawers that render custom GUI elements in the Zeep GUI system
/// </summary>
public interface IZeepGUIDrawer
{
    /// <summary>
    /// Called during the GUI rendering phase to draw custom UI elements
    /// </summary>
    /// <param name="gui">The ImGui instance to use for drawing</param>
    void OnZeepGUI(ImGui gui);
}