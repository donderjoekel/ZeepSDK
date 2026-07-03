using System.Collections.Generic;
using BepInEx.Configuration;

namespace ZeepSDK.Settings;

/// <summary>
/// Stores custom draw callbacks for config entries, keyed by plugin GUID and config definition.
/// </summary>
internal static class ZeepSettingsEntryDrawerRegistry
{
    private static readonly Dictionary<string, Dictionary<ConfigDefinition, ModSettingsConfigEntryDrawDelegate>> DrawersByPlugin = new();

    public static void SetDrawer(string pluginGuid, ConfigDefinition definition, ModSettingsConfigEntryDrawDelegate drawer)
    {
        if (!DrawersByPlugin.TryGetValue(pluginGuid, out var drawers))
            DrawersByPlugin[pluginGuid] = drawers = new Dictionary<ConfigDefinition, ModSettingsConfigEntryDrawDelegate>();

        drawers[definition] = drawer;
    }

    public static void ClearDrawer(string pluginGuid, ConfigDefinition definition)
    {
        if (!DrawersByPlugin.TryGetValue(pluginGuid, out var drawers))
            return;

        drawers.Remove(definition);

        if (drawers.Count == 0)
            DrawersByPlugin.Remove(pluginGuid);
    }

    public static IReadOnlyDictionary<ConfigDefinition, ModSettingsConfigEntryDrawDelegate> GetDrawers(string pluginGuid)
        => DrawersByPlugin.GetValueOrDefault(pluginGuid);
}
