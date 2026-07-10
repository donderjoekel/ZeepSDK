using System;
using System.Collections.Generic;
using BepInEx.Configuration;
using BepInEx.Logging;
using JetBrains.Annotations;
using ZeepSDK.Settings.Drawers;
using ZeepSDK.Utilities;

namespace ZeepSDK.Settings;

/// <summary>
/// Builds tab definitions for the mod settings panel.
/// </summary>
[PublicAPI]
public sealed class ModSettingsTabsBuilder
{
    private static readonly ManualLogSource Logger = LoggerFactory.GetLogger(typeof(ModSettingsTabsBuilder));

    private readonly List<ModSettingsTabDefinition> _tabs = [];
    private readonly HashSet<string> _assignedSections = new(StringComparer.Ordinal);
    private readonly IReadOnlyDictionary<string, IReadOnlyList<ConfigEntryBase>> _entriesBySection;
    private readonly string _pluginGuid;
    private ModSettingsTabBuilder _currentTab;

    internal ModSettingsTabsBuilder(
        IReadOnlyDictionary<string, IReadOnlyList<ConfigEntryBase>> entriesBySection,
        string pluginGuid = null)
    {
        _entriesBySection = entriesBySection ?? throw new ArgumentNullException(nameof(entriesBySection));
        _pluginGuid = pluginGuid;
    }

    /// <summary>
    /// Adds a tab with the given label and optional initial sections.
    /// </summary>
    /// <param name="label">The label shown on the tab button.</param>
    /// <param name="sections">BepInEx section names to include in this tab.</param>
    /// <returns>A builder for adding more content to this tab.</returns>
    public ModSettingsTabBuilder Tab(string label, params string[] sections)
    {
        FinalizeCurrentTab();
        _currentTab = new ModSettingsTabBuilder(label, this);
        if (sections is { Length: > 0 })
            _currentTab.Sections(sections);

        return _currentTab;
    }

    internal void FinalizeCurrentTab()
    {
        if (_currentTab == null)
            return;

        if (_currentTab.ItemCount > 0)
            _tabs.Add(_currentTab.ToDefinition());

        _currentTab = null;
    }

    internal bool TryAddSection(string sectionName)
    {
        if (!_entriesBySection.ContainsKey(sectionName))
        {
            if (_pluginGuid != null)
            {
                Logger.LogWarning(
                    $"Mod settings tab configuration for '{_pluginGuid}' references unknown section '{sectionName}'. Section was skipped.");
            }

            return false;
        }

        if (!_assignedSections.Add(sectionName))
        {
            if (_pluginGuid != null)
            {
                Logger.LogWarning(
                    $"Mod settings tab configuration for '{_pluginGuid}' assigns section '{sectionName}' to multiple tabs. Section was skipped.");
            }

            return false;
        }

        return true;
    }

    internal IReadOnlyList<ModSettingsTabDefinition> Build()
    {
        FinalizeCurrentTab();
        return _tabs;
    }
}

/// <summary>
/// Builds the content of a single mod settings tab.
/// </summary>
[PublicAPI]
public sealed class ModSettingsTabBuilder
{
    private readonly string _label;
    private readonly ModSettingsTabsBuilder _parent;
    private readonly List<ModSettingsTabItem> _items = [];

    internal ModSettingsTabBuilder(string label, ModSettingsTabsBuilder parent)
    {
        _label = label ?? throw new ArgumentNullException(nameof(label));
        _parent = parent ?? throw new ArgumentNullException(nameof(parent));
    }

    internal int ItemCount => _items.Count;

    /// <summary>
    /// Adds all visible config entries from a BepInEx section to this tab.
    /// </summary>
    /// <param name="sectionName">The BepInEx config section name.</param>
    /// <returns>This builder for chaining.</returns>
    public ModSettingsTabBuilder Section(string sectionName)
    {
        if (string.IsNullOrEmpty(sectionName))
            throw new ArgumentException("Section name cannot be null or empty.", nameof(sectionName));

        if (!_parent.TryAddSection(sectionName))
            return this;

        _items.Add(new ModSettingsTabSectionItem(sectionName));
        return this;
    }

    /// <summary>
    /// Adds all visible config entries from multiple BepInEx sections to this tab.
    /// </summary>
    /// <param name="sectionNames">The BepInEx config section names.</param>
    /// <returns>This builder for chaining.</returns>
    public ModSettingsTabBuilder Sections(params string[] sectionNames)
    {
        if (sectionNames == null)
            throw new ArgumentNullException(nameof(sectionNames));

        foreach (var sectionName in sectionNames)
            Section(sectionName);

        return this;
    }

    /// <summary>
    /// Adds a single config entry to this tab.
    /// </summary>
    /// <param name="entry">The config entry to draw.</param>
    /// <param name="label">Optional display label. Uses the config key when omitted.</param>
    /// <returns>This builder for chaining.</returns>
    public ModSettingsTabBuilder Entry(ConfigEntryBase entry, string label = null)
    {
        if (entry == null)
            throw new ArgumentNullException(nameof(entry));

        _items.Add(new ModSettingsTabEntryItem(entry, label));
        return this;
    }

    /// <summary>
    /// Adds a custom settings drawer to this tab.
    /// </summary>
    /// <param name="drawer">The drawer to render.</param>
    /// <returns>This builder for chaining.</returns>
    public ModSettingsTabBuilder Drawer(IZeepSettingsDrawer drawer)
    {
        if (drawer == null)
            throw new ArgumentNullException(nameof(drawer));

        _items.Add(new ModSettingsTabDrawerItem(drawer));
        return this;
    }

    internal ModSettingsTabDefinition ToDefinition() => new(_label, _items);
}
