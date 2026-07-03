using System.Collections.Generic;
using BepInEx.Configuration;

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
    public static IEnumerable<IZeepSettingsDrawer> Build(
        IReadOnlyDictionary<string, IReadOnlyList<ConfigEntryBase>> entriesBySection)
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
                yield return new ZeepSettingsEntryDrawer(entry);
                yield return new ZeepSettingsSeparatorDrawer();
            }

            if (anyVisible)
                yield return new ZeepSettingsSectionSpacingDrawer();
        }
    }
}
