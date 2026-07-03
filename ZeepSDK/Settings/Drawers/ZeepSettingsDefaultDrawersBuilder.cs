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
    /// </summary>
    /// <param name="entriesBySection">Config entries grouped by section name.</param>
    /// <param name="customLabels">Optional custom labels keyed by config definition.</param>
    /// <param name="customDrawers">Optional custom draw callbacks keyed by config definition.</param>
    public static IEnumerable<IZeepSettingsDrawer> Build(
        IReadOnlyDictionary<string, IReadOnlyList<ConfigEntryBase>> entriesBySection,
        IReadOnlyDictionary<ConfigDefinition, string> customLabels = null,
        IReadOnlyDictionary<ConfigDefinition, ModSettingsConfigEntryDrawDelegate> customDrawers = null)
    {
        foreach ((string section, IReadOnlyList<ConfigEntryBase> sectionEntries) in entriesBySection)
        {
            yield return new ZeepSettingsHeaderDrawer(section);

            var anyVisible = false;
            foreach (var entry in sectionEntries)
            {
                if (ZeepSettingsEntryMetadata.IsHidden(entry))
                    continue;

                anyVisible = true;

                string label = null;
                customLabels?.TryGetValue(entry.Definition, out label);

                if (customDrawers != null && customDrawers.TryGetValue(entry.Definition, out var drawer))
                    yield return new ZeepSettingsCustomConfigEntryDrawer(entry, label, drawer);
                else
                    yield return new ZeepSettingsEntryDrawer(entry, label);

                yield return new ZeepSettingsSeparatorDrawer();
            }

            if (anyVisible)
                yield return new ZeepSettingsSectionSpacingDrawer();
        }
    }
}
