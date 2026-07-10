using System.Collections.Generic;
using BepInEx.Configuration;
using JetBrains.Annotations;
using ZeepSDK.Settings.Drawers;

namespace ZeepSDK.Settings;

/// <summary>
/// A single item within a mod settings tab.
/// </summary>
[PublicAPI]
public abstract class ModSettingsTabItem;

/// <summary>
/// Includes all visible config entries from a BepInEx section in a tab.
/// </summary>
[PublicAPI]
public sealed class ModSettingsTabSectionItem : ModSettingsTabItem
{
    /// <summary>
    /// The BepInEx config section name.
    /// </summary>
    public string SectionName { get; }

    internal ModSettingsTabSectionItem(string sectionName) => SectionName = sectionName;
}

/// <summary>
/// Includes a single config entry in a tab.
/// </summary>
[PublicAPI]
public sealed class ModSettingsTabEntryItem : ModSettingsTabItem
{
    /// <summary>
    /// The config entry to draw.
    /// </summary>
    public ConfigEntryBase Entry { get; }

    /// <summary>
    /// Optional display label. Uses the config key when omitted.
    /// </summary>
    public string Label { get; }

    internal ModSettingsTabEntryItem(ConfigEntryBase entry, string label)
    {
        Entry = entry;
        Label = label;
    }
}

/// <summary>
/// Includes a custom settings drawer in a tab.
/// </summary>
[PublicAPI]
public sealed class ModSettingsTabDrawerItem : ModSettingsTabItem
{
    /// <summary>
    /// The drawer to render.
    /// </summary>
    public IZeepSettingsDrawer Drawer { get; }

    internal ModSettingsTabDrawerItem(IZeepSettingsDrawer drawer) => Drawer = drawer;
}

/// <summary>
/// Defines a single tab in the mod settings panel.
/// </summary>
[PublicAPI]
public sealed class ModSettingsTabDefinition
{
    /// <summary>
    /// The label shown on the tab button.
    /// </summary>
    public string Label { get; }

    /// <summary>
    /// The ordered content of this tab.
    /// </summary>
    public IReadOnlyList<ModSettingsTabItem> Items { get; }

    internal ModSettingsTabDefinition(string label, IReadOnlyList<ModSettingsTabItem> items)
    {
        Label = label;
        Items = items;
    }
}
