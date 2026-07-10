using System;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Configuration;
using ZeepSDK.Settings.Drawers;
using ZeepSDK.Utilities;

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

    private string PluginGuid => Plugin.Metadata.GUID;

    private IReadOnlyDictionary<ConfigDefinition, string> CustomLabels
        => ZeepSettingsEntryLabelRegistry.GetLabels(PluginGuid);

    private IReadOnlyDictionary<ConfigDefinition, ModSettingsConfigEntryDrawDelegate> CustomDrawers
        => ZeepSettingsEntryDrawerRegistry.GetDrawers(PluginGuid);

    /// <summary>
    /// Creates the default drawer list for this plugin's config entries.
    /// </summary>
    /// <returns>A sequence of drawers that render the plugin's config entries using the default layout.</returns>
    public IEnumerable<IZeepSettingsDrawer> CreateDefaultDrawers()
        => ZeepSettingsDefaultDrawersBuilder.Build(
            EntriesBySection,
            CustomLabels,
            CustomDrawers,
            PluginGuid);

    /// <summary>
    /// Creates the default drawers for a single BepInEx config section.
    /// </summary>
    /// <param name="section">The BepInEx section name.</param>
    /// <returns>Drawers for the section header, entries, separators, and trailing spacing.</returns>
    public IEnumerable<IZeepSettingsDrawer> CreateSectionDrawers(string section)
    {
        if (!EntriesBySection.TryGetValue(section, out var entries))
            return [];

        return ZeepSettingsDefaultDrawersBuilder.BuildSectionDrawers(section, entries, CustomLabels, CustomDrawers);
    }

    /// <summary>
    /// Creates flat section drawers for sections not assigned to a configured tab layout.
    /// </summary>
    /// <param name="excludeTabbedSections">
    /// When <see langword="true"/>, sections assigned via <see cref="SettingsApi.ConfigureModSettingsTabs"/> are omitted.
    /// </param>
    /// <returns>Flat section drawers for the remaining sections.</returns>
    public IEnumerable<IZeepSettingsDrawer> CreateFlatSectionDrawers(bool excludeTabbedSections)
    {
        HashSet<string> assignedSections = null;
        if (excludeTabbedSections &&
            ZeepSettingsTabsRegistry.TryGetTabs(PluginGuid, out var tabConfig) &&
            tabConfig.Count > 0)
        {
            assignedSections = ZeepSettingsTabsRegistry.GetAssignedSections(tabConfig);
        }

        foreach ((string section, IReadOnlyList<ConfigEntryBase> sectionEntries) in EntriesBySection)
        {
            if (assignedSections != null && assignedSections.Contains(section))
                continue;

            foreach (var drawer in ZeepSettingsDefaultDrawersBuilder.BuildSectionDrawers(
                         section, sectionEntries, CustomLabels, CustomDrawers))
                yield return drawer;
        }
    }

    /// <summary>
    /// Creates a tabbed settings drawer from a tab configuration callback.
    /// </summary>
    /// <param name="configure">Builds the tab layout.</param>
    /// <returns>A drawer that renders the configured tabs.</returns>
    public ZeepSettingsTabbedSectionsDrawer CreateTabbedSectionsDrawer(Action<ModSettingsTabsBuilder> configure)
    {
        if (configure == null)
            throw new ArgumentNullException(nameof(configure));

        var builder = new ModSettingsTabsBuilder(EntriesBySection, PluginGuid);
        configure(builder);
        var tabConfig = builder.Build();

        return ZeepSettingsDefaultDrawersBuilder.CreateTabbedDrawer(
            tabConfig,
            EntriesBySection,
            CustomLabels,
            CustomDrawers);
    }

    internal static Dictionary<string, IReadOnlyList<ConfigEntryBase>> BuildEntriesBySection(PluginInfo plugin)
    {
        var customLabels = ZeepSettingsEntryLabelRegistry.GetLabels(plugin.Metadata.GUID);
        var sections = new Dictionary<string, List<ConfigEntryBase>>();

        foreach ((ConfigDefinition definition, ConfigEntryBase entry) in plugin.Instance.Config)
        {
            if (!sections.TryGetValue(definition.Section, out var entries))
                sections.Add(definition.Section, entries = []);

            entries.Add(entry);
        }

        foreach (var entries in sections.Values)
        {
            entries.Sort((a, b) =>
            {
                int cmp = NaturalStringComparer.Instance.Compare(
                    GetEntrySortKey(a, customLabels),
                    GetEntrySortKey(b, customLabels));
                return cmp != 0
                    ? cmp
                    : string.Compare(a.Definition.Key, b.Definition.Key, StringComparison.Ordinal);
            });
        }

        var result = new Dictionary<string, IReadOnlyList<ConfigEntryBase>>(sections.Count);
        foreach ((string section, List<ConfigEntryBase> entries) in sections)
            result[section] = entries;

        return result;
    }

    private static string GetEntrySortKey(
        ConfigEntryBase entry,
        IReadOnlyDictionary<ConfigDefinition, string> customLabels)
    {
        if (customLabels != null && customLabels.TryGetValue(entry.Definition, out var label) && label != null)
            return label;

        return entry.Definition.Key;
    }
}
