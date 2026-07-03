using Imui.Controls;
using Imui.Core;

namespace ZeepSDK.Settings.Drawers;

/// <summary>
/// Draws a section header in the mod settings panel.
/// </summary>
public class ZeepSettingsHeaderDrawer : IZeepSettingsDrawer
{
    /// <summary>
    /// The header text to display.
    /// </summary>
    public string Header { get; }

    /// <summary>
    /// Creates a new section header drawer.
    /// </summary>
    /// <param name="header">The header text to display.</param>
    public ZeepSettingsHeaderDrawer(string header)
    {
        Header = header;
    }

    /// <inheritdoc />
    public void Draw(ImGui gui, ZeepSettingsDrawContext context)
    {
        using (gui.Indent())
        {
            gui.Text(Header, new ImTextSettings(36));
        }
    }
}
