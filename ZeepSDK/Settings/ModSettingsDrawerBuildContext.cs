using System.Collections.Generic;
using BepInEx;
using BepInEx.Configuration;
using ZeepSDK.Settings.Drawers;

namespace ZeepSDK.Settings;

/// <summary>
/// Context passed to mod settings drawer providers when building a drawer list.
/// </summary>
public sealed class ModSettingsDrawerBuildContext
{
    /// <summary>
    /// The plugin whose settings are being drawn.
    /// </summary>
    public PluginInfo Plugin { get; }

    /// <summary>
    /// The plugin's config entries grouped by section name.
    /// </summary>
    public IReadOnlyDictionary<string, IReadOnlyList<ConfigEntryBase>> EntriesBySection { get; }

    internal ModSettingsDrawerBuildContext(
        PluginInfo plugin,
        IReadOnlyDictionary<string, IReadOnlyList<ConfigEntryBase>> entriesBySection)
    {
        Plugin = plugin;
        EntriesBySection = entriesBySection;
    }

    /// <summary>
    /// Creates the default drawer list for this plugin's config entries.
    /// </summary>
    /// <returns>A sequence of drawers that render the plugin's config entries using the default layout.</returns>
    public IEnumerable<IZeepSettingsDrawer> CreateDefaultDrawers()
        => ZeepSettingsDefaultDrawersBuilder.Build(
            EntriesBySection,
            ZeepSettingsEntryLabelRegistry.GetLabels(Plugin.Metadata.GUID),
            ZeepSettingsEntryDrawerRegistry.GetDrawers(Plugin.Metadata.GUID));
}
