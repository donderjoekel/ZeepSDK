using System;
using System.Collections.Generic;
using BepInEx.Configuration;

namespace ZeepSDK.Settings;

/// <summary>
/// Stores tab layout configuration per plugin GUID.
/// </summary>
internal static class ZeepSettingsTabsRegistry
{
    private static readonly Dictionary<string, IReadOnlyList<ModSettingsTabDefinition>> TabsByPlugin = new();

    public static void Configure(
        string pluginGuid,
        Action<ModSettingsTabsBuilder> configure,
        IReadOnlyDictionary<string, IReadOnlyList<ConfigEntryBase>> entriesBySection)
    {
        if (configure == null)
            throw new ArgumentNullException(nameof(configure));

        var builder = new ModSettingsTabsBuilder(entriesBySection, pluginGuid);
        configure(builder);

        var builtTabs = builder.Build();
        if (builtTabs.Count == 0)
        {
            TabsByPlugin.Remove(pluginGuid);
            return;
        }

        TabsByPlugin[pluginGuid] = builtTabs;
    }

    public static void ClearTabs(string pluginGuid) => TabsByPlugin.Remove(pluginGuid);

    public static bool TryGetTabs(string pluginGuid, out IReadOnlyList<ModSettingsTabDefinition> tabs)
        => TabsByPlugin.TryGetValue(pluginGuid, out tabs);

    public static HashSet<string> GetAssignedSections(IReadOnlyList<ModSettingsTabDefinition> tabs)
    {
        var assigned = new HashSet<string>(StringComparer.Ordinal);
        foreach (var tab in tabs)
        {
            foreach (var item in tab.Items)
            {
                if (item is ModSettingsTabSectionItem sectionItem)
                    assigned.Add(sectionItem.SectionName);
            }
        }

        return assigned;
    }
}
