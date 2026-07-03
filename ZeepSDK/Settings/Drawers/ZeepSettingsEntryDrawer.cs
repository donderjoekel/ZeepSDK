using BepInEx.Configuration;
using Imui.Core;

namespace ZeepSDK.Settings.Drawers;

/// <summary>
/// Draws a BepInEx config entry in the mod settings panel.
/// </summary>
public class ZeepSettingsEntryDrawer : IZeepSettingsDrawer
{
    /// <summary>
    /// The config entry to draw.
    /// </summary>
    public ConfigEntryBase ConfigEntry { get; }

    /// <summary>
    /// The label shown for this entry. Defaults to the config key when not specified.
    /// </summary>
    public string Label { get; }

    /// <summary>
    /// Creates a new config entry drawer.
    /// </summary>
    /// <param name="configEntry">The config entry to draw.</param>
    /// <param name="label">Optional display label. Uses the config key when omitted.</param>
    public ZeepSettingsEntryDrawer(ConfigEntryBase configEntry, string label = null)
    {
        ConfigEntry = configEntry;
        Label = label ?? configEntry.Definition.Key;
    }

    /// <inheritdoc />
    public void Draw(ImGui gui, ZeepSettingsDrawContext context)
    {
        using (gui.Indent())
        {
            ZeepSettingsEntryRenderer.Draw(gui, ConfigEntry, context, Label);
        }
    }
}
