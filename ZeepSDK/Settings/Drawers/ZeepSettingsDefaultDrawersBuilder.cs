using System.Collections.Generic;
using BepInEx.Configuration;
using ZeepSDK.Settings;

namespace ZeepSDK.Settings.Drawers;

/// <summary>
/// Builds the default sequence of settings drawers for a plugin's config entries.
/// </summary>
internal static class ZeepSettingsDefaultDrawersBuilder
{
    /// <summary>
    /// Builds the default drawer list: section header, entry, separator, and section spacing.
    /// When tab configuration exists for the plugin, tabbed sections are rendered first and
    /// unassigned sections are rendered flat below.
    /// </summary>
    public static IEnumerable<IZeepSettingsDrawer> Build(
        IReadOnlyDictionary<string, IReadOnlyList<ConfigEntryBase>> entriesBySection,
        IReadOnlyDictionary<ConfigDefinition, string> customLabels = null,
        IReadOnlyDictionary<ConfigDefinition, ModSettingsConfigEntryDrawDelegate> customDrawers = null,
        string pluginGuid = null)
    {
        if (pluginGuid != null &&
            ZeepSettingsTabsRegistry.TryGetTabs(pluginGuid, out var tabConfig) &&
            tabConfig.Count > 0)
        {
            yield return CreateTabbedDrawer(tabConfig, entriesBySection, customLabels, customDrawers);

            var assignedSections = ZeepSettingsTabsRegistry.GetAssignedSections(tabConfig);
            foreach ((string section, IReadOnlyList<ConfigEntryBase> sectionEntries) in entriesBySection)
            {
                if (assignedSections.Contains(section))
                    continue;

                foreach (var drawer in BuildSectionDrawers(section, sectionEntries, customLabels, customDrawers))
                    yield return drawer;
            }

            yield break;
        }

        foreach ((string section, IReadOnlyList<ConfigEntryBase> sectionEntries) in entriesBySection)
        {
            foreach (var drawer in BuildSectionDrawers(section, sectionEntries, customLabels, customDrawers))
                yield return drawer;
        }
    }

    public static IEnumerable<IZeepSettingsDrawer> BuildSectionDrawers(
        string section,
        IReadOnlyList<ConfigEntryBase> sectionEntries,
        IReadOnlyDictionary<ConfigDefinition, string> customLabels = null,
        IReadOnlyDictionary<ConfigDefinition, ModSettingsConfigEntryDrawDelegate> customDrawers = null)
    {
        yield return new ZeepSettingsHeaderDrawer(section);

        var anyVisible = false;
        foreach (var entry in sectionEntries)
        {
            if (ZeepSettingsEntryMetadata.IsHidden(entry))
                continue;

            anyVisible = true;

            foreach (var drawer in BuildEntryDrawers(entry, customLabels, customDrawers))
                yield return drawer;

            yield return new ZeepSettingsSeparatorDrawer();
        }

        if (anyVisible)
            yield return new ZeepSettingsSectionSpacingDrawer();
    }

    public static IEnumerable<IZeepSettingsDrawer> BuildEntryDrawers(
        ConfigEntryBase entry,
        IReadOnlyDictionary<ConfigDefinition, string> customLabels = null,
        IReadOnlyDictionary<ConfigDefinition, ModSettingsConfigEntryDrawDelegate> customDrawers = null,
        string explicitLabel = null)
    {
        if (ZeepSettingsEntryMetadata.IsHidden(entry))
            yield break;

        string label = explicitLabel;
        if (label == null)
            customLabels?.TryGetValue(entry.Definition, out label);

        if (customDrawers != null && customDrawers.TryGetValue(entry.Definition, out var drawer))
            yield return new ZeepSettingsCustomConfigEntryDrawer(entry, label, drawer);
        else if (ZeepSettingsConfigEntryTypeDrawerRegistry.TryCreateDrawer(entry, label, out var typeDrawer))
            yield return typeDrawer;
        else
            yield return new ZeepSettingsEntryDrawer(entry, label);
    }

    public static ZeepSettingsTabbedSectionsDrawer CreateTabbedDrawer(
        IReadOnlyList<ModSettingsTabDefinition> tabConfig,
        IReadOnlyDictionary<string, IReadOnlyList<ConfigEntryBase>> entriesBySection,
        IReadOnlyDictionary<ConfigDefinition, string> customLabels = null,
        IReadOnlyDictionary<ConfigDefinition, ModSettingsConfigEntryDrawDelegate> customDrawers = null)
    {
        var tabs = new List<ZeepSettingsTabbedSectionsDrawer.TabContent>(tabConfig.Count);

        foreach (var tab in tabConfig)
        {
            var tabDrawers = new List<IZeepSettingsDrawer>();
            foreach (var item in tab.Items)
            {
                switch (item)
                {
                    case ModSettingsTabSectionItem sectionItem:
                        if (entriesBySection.TryGetValue(sectionItem.SectionName, out var sectionEntries))
                        {
                            tabDrawers.AddRange(
                                BuildSectionDrawers(sectionItem.SectionName, sectionEntries, customLabels, customDrawers));
                        }

                        break;

                    case ModSettingsTabEntryItem entryItem:
                        tabDrawers.AddRange(
                            BuildEntryDrawers(entryItem.Entry, customLabels, customDrawers, entryItem.Label));
                        tabDrawers.Add(new ZeepSettingsSeparatorDrawer());
                        break;

                    case ModSettingsTabDrawerItem drawerItem:
                        tabDrawers.Add(drawerItem.Drawer);
                        break;
                }
            }

            if (tabDrawers.Count > 0)
                tabs.Add(new ZeepSettingsTabbedSectionsDrawer.TabContent(tab.Label, tabDrawers));
        }

        return new ZeepSettingsTabbedSectionsDrawer(tabs);
    }
}
