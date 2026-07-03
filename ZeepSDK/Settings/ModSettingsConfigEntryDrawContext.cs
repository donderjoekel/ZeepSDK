using BepInEx.Configuration;
using Imui.Core;
using ZeepSDK.Settings.Drawers;

namespace ZeepSDK.Settings;

/// <summary>
/// Context passed to custom config entry draw callbacks.
/// </summary>
public sealed class ModSettingsConfigEntryDrawContext
{
    /// <summary>
    /// The config entry being drawn.
    /// </summary>
    public ConfigEntryBase Entry { get; }

    /// <summary>
    /// The resolved display label, including any label override registered via <see cref="SettingsApi.SetConfigEntryLabel"/>.
    /// </summary>
    public string Label { get; }

    internal ModSettingsConfigEntryDrawContext(ConfigEntryBase entry, string label)
    {
        Entry = entry;
        Label = label;
    }

    /// <summary>
    /// Draws the default settings row for this config entry.
    /// </summary>
    /// <param name="gui">The ImGui instance to draw with.</param>
    /// <param name="context">Shared services available while drawing settings.</param>
    public void DrawDefault(ImGui gui, ZeepSettingsDrawContext context)
        => ZeepSettingsEntryRenderer.Draw(gui, Entry, context, Label);
}
