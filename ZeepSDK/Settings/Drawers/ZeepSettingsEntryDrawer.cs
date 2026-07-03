using BepInEx.Configuration;
using Imui.Core;

namespace ZeepSDK.Settings.Drawers;

/// <summary>
/// Draws a BepInEx config entry in the mod settings panel.
/// </summary>
public readonly struct ZeepSettingsEntryDrawer : IZeepSettingsDrawer
{
    /// <summary>
    /// The config entry to draw.
    /// </summary>
    public readonly ConfigEntryBase ConfigEntry;

    /// <summary>
    /// Creates a new config entry drawer.
    /// </summary>
    /// <param name="configEntry">The config entry to draw.</param>
    public ZeepSettingsEntryDrawer(ConfigEntryBase configEntry)
    {
        ConfigEntry = configEntry;
    }

    /// <inheritdoc />
    public void Draw(ImGui gui, ZeepSettingsDrawContext context)
    {
        using (gui.Indent())
        {
            ZeepSettingsEntryRenderer.Draw(gui, ConfigEntry, context);
        }
    }
}
