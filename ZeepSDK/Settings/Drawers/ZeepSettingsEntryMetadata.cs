using System;
using BepInEx.Configuration;

namespace ZeepSDK.Settings.Drawers;

/// <summary>
/// Helpers for interpreting BepInEx config entry description metadata.
/// </summary>
internal static class ZeepSettingsEntryMetadata
{
    /// <summary>
    /// Returns whether the config entry should be hidden from the settings UI.
    /// Entries with a description starting with <c>[hide]</c> or <c>[hidden]</c> are hidden.
    /// </summary>
    public static bool IsHidden(ConfigEntryBase entry)
    {
        return entry.Description.Description.StartsWith("[hide]", StringComparison.OrdinalIgnoreCase) ||
               entry.Description.Description.StartsWith("[hidden]", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Returns whether the config entry should be rendered as an action button.
    /// Entries with a description starting with <c>[button]</c> and a bool type are treated as buttons.
    /// </summary>
    public static bool IsButton(ConfigEntryBase entry)
    {
        return entry.Description.Description.StartsWith("[button]", StringComparison.OrdinalIgnoreCase) &&
               entry.SettingType == typeof(bool);
    }
}
