using System;
using System.Collections.Generic;
using BepInEx.Configuration;
using ZeepSDK.Settings.Drawers;

namespace ZeepSDK.Settings;

/// <summary>
/// Stores global config entry type drawer factories keyed by setting type.
/// </summary>
internal static class ZeepSettingsConfigEntryTypeDrawerRegistry
{
    private static readonly Dictionary<Type, ModSettingsConfigEntryTypeDrawerFactory> Factories = new();

    public static bool IsRegistered(Type settingType) => Factories.ContainsKey(settingType);

    public static void Register(Type settingType, ModSettingsConfigEntryTypeDrawerFactory factory)
        => Factories[settingType] = factory;

    public static void Clear(Type settingType) => Factories.Remove(settingType);

    public static bool TryCreateDrawer(ConfigEntryBase entry, string label, out IZeepSettingsDrawer drawer)
    {
        if (Factories.TryGetValue(entry.SettingType, out var factory))
        {
            drawer = factory(entry, label);
            return drawer != null;
        }

        drawer = null;
        return false;
    }
}
