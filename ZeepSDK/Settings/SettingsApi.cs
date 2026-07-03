using System;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using JetBrains.Annotations;
using ZeepkistClient;
using ZeepSDK.Settings.Drawers;
using ZeepSDK.UI;
using ZeepSDK.Utilities;

namespace ZeepSDK.Settings;

/// <summary>
/// Provides APIs for managing the mod settings window.
/// </summary>
[PublicAPI]
public static class SettingsApi
{
    private static readonly ManualLogSource Logger = LoggerFactory.GetLogger(typeof(SettingsApi));

    /// <summary>
    /// Invoked whenever the mod settings window opens.
    /// </summary>
    public static event ModSettingsWindowOpenedDelegate ModSettingsWindowOpened;

    /// <summary>
    /// Invoked whenever the mod settings window closes.
    /// </summary>
    public static event ModSettingsWindowClosedDelegate ModSettingsWindowClosed;

    private static ZeepSettingsDrawer _zeepSettingsDrawer;

    internal static void Initialize()
    {
        ZeepkistNetwork.LobbyGameStateChanged += CloseModSettings;
        _zeepSettingsDrawer = new ZeepSettingsDrawer();
        UIApi.AddZeepGUIDrawer(_zeepSettingsDrawer);
    }

    /// <summary>
    /// Opens the mod settings window if it isn't already open.
    /// </summary>
    public static void OpenModSettings()
    {
        _zeepSettingsDrawer.Open();
        DispatchWindowOpened();
    }

    /// <summary>
    /// Closes the mod settings window if it is already open.
    /// </summary>
    public static void CloseModSettings()
    {
        _zeepSettingsDrawer.Close();
        DispatchWindowClosed();
    }

    /// <summary>
    /// Registers a custom settings drawer provider for the given mod.
    /// When the mod is selected in the settings window, the provider is invoked to build the drawer list.
    /// Use <see cref="ModSettingsDrawerBuildContext.CreateDefaultDrawers"/> to compose with or replace the default layout.
    /// </summary>
    /// <param name="plugin">The mod plugin instance to register the provider for.</param>
    /// <param name="provider">The callback that builds the drawer list for this mod.</param>
    public static void RegisterModSettingsDrawers(BaseUnityPlugin plugin, ModSettingsDrawersDelegate provider)
        => RegisterModSettingsDrawers(plugin.Info.Metadata.GUID, provider);

    /// <summary>
    /// Registers a custom settings drawer provider for the given mod GUID.
    /// </summary>
    /// <param name="pluginGuid">The BepInEx GUID of the mod to register the provider for.</param>
    /// <param name="provider">The callback that builds the drawer list for this mod.</param>
    public static void RegisterModSettingsDrawers(string pluginGuid, ModSettingsDrawersDelegate provider)
        => ZeepSettingsDrawerRegistry.Register(pluginGuid, provider);

    /// <summary>
    /// Sets a custom display label for a config entry when using the default settings layout.
    /// </summary>
    /// <param name="plugin">The mod plugin instance that owns the config entry.</param>
    /// <param name="entry">The config entry to label.</param>
    /// <param name="label">The label to display in the settings panel.</param>
    public static void SetConfigEntryLabel(BaseUnityPlugin plugin, ConfigEntryBase entry, string label)
    {
        if (!TryValidateConfigEntry(plugin, entry))
            return;

        SetConfigEntryLabel(plugin.Info.Metadata.GUID, entry, label);
    }

    /// <summary>
    /// Sets a custom display label for a config entry when using the default settings layout.
    /// </summary>
    /// <param name="pluginGuid">The BepInEx GUID of the mod that owns the config entry.</param>
    /// <param name="entry">The config entry to label.</param>
    /// <param name="label">The label to display in the settings panel.</param>
    public static void SetConfigEntryLabel(string pluginGuid, ConfigEntryBase entry, string label)
        => ZeepSettingsEntryLabelRegistry.SetLabel(pluginGuid, entry.Definition, label);

    /// <summary>
    /// Removes a custom display label for a config entry, restoring the default config key label.
    /// </summary>
    /// <param name="plugin">The mod plugin instance that owns the config entry.</param>
    /// <param name="entry">The config entry to clear the label for.</param>
    public static void ClearConfigEntryLabel(BaseUnityPlugin plugin, ConfigEntryBase entry)
    {
        if (!TryValidateConfigEntry(plugin, entry))
            return;

        ClearConfigEntryLabel(plugin.Info.Metadata.GUID, entry);
    }

    /// <summary>
    /// Removes a custom display label for a config entry, restoring the default config key label.
    /// </summary>
    /// <param name="pluginGuid">The BepInEx GUID of the mod that owns the config entry.</param>
    /// <param name="entry">The config entry to clear the label for.</param>
    public static void ClearConfigEntryLabel(string pluginGuid, ConfigEntryBase entry)
        => ZeepSettingsEntryLabelRegistry.ClearLabel(pluginGuid, entry.Definition);

    /// <summary>
    /// Sets a custom draw callback for a config entry when using the default settings layout.
    /// </summary>
    /// <param name="plugin">The mod plugin instance that owns the config entry.</param>
    /// <param name="entry">The config entry to draw.</param>
    /// <param name="drawer">The callback invoked to draw this config entry.</param>
    public static void SetConfigEntryDrawer(BaseUnityPlugin plugin, ConfigEntryBase entry, ModSettingsConfigEntryDrawDelegate drawer)
    {
        if (!TryValidateConfigEntry(plugin, entry))
            return;

        SetConfigEntryDrawer(plugin.Info.Metadata.GUID, entry, drawer);
    }

    /// <summary>
    /// Sets a custom draw callback for a config entry when using the default settings layout.
    /// </summary>
    /// <param name="pluginGuid">The BepInEx GUID of the mod that owns the config entry.</param>
    /// <param name="entry">The config entry to draw.</param>
    /// <param name="drawer">The callback invoked to draw this config entry.</param>
    public static void SetConfigEntryDrawer(string pluginGuid, ConfigEntryBase entry, ModSettingsConfigEntryDrawDelegate drawer)
        => ZeepSettingsEntryDrawerRegistry.SetDrawer(pluginGuid, entry.Definition, drawer);

    /// <summary>
    /// Removes a custom draw callback for a config entry, restoring the default row renderer.
    /// </summary>
    /// <param name="plugin">The mod plugin instance that owns the config entry.</param>
    /// <param name="entry">The config entry to clear the drawer for.</param>
    public static void ClearConfigEntryDrawer(BaseUnityPlugin plugin, ConfigEntryBase entry)
    {
        if (!TryValidateConfigEntry(plugin, entry))
            return;

        ClearConfigEntryDrawer(plugin.Info.Metadata.GUID, entry);
    }

    /// <summary>
    /// Removes a custom draw callback for a config entry, restoring the default row renderer.
    /// </summary>
    /// <param name="pluginGuid">The BepInEx GUID of the mod that owns the config entry.</param>
    /// <param name="entry">The config entry to clear the drawer for.</param>
    public static void ClearConfigEntryDrawer(string pluginGuid, ConfigEntryBase entry)
        => ZeepSettingsEntryDrawerRegistry.ClearDrawer(pluginGuid, entry.Definition);

    /// <summary>
    /// Registers a global drawer factory for config entries with the given setting type.
    /// Any plugin's config entry with a matching <see cref="ConfigEntryBase.SettingType"/> uses this drawer in the default layout.
    /// </summary>
    /// <typeparam name="T">The config entry value type.</typeparam>
    /// <param name="factory">Creates a drawer for each config entry of this type.</param>
    public static void RegisterConfigEntryTypeDrawer<T>(Func<ConfigEntry<T>, string, IZeepSettingsDrawer> factory)
    {
        if (factory == null)
            throw new ArgumentNullException(nameof(factory));

        RegisterConfigEntryTypeDrawer(typeof(T), (entry, label) => factory((ConfigEntry<T>)entry, label));
    }

    /// <summary>
    /// Registers a global drawer factory for config entries with the given setting type.
    /// </summary>
    /// <param name="settingType">The config entry value type.</param>
    /// <param name="factory">Creates a drawer for each config entry of this type.</param>
    public static void RegisterConfigEntryTypeDrawer(Type settingType, ModSettingsConfigEntryTypeDrawerFactory factory)
    {
        if (settingType == null)
            throw new ArgumentNullException(nameof(settingType));

        if (factory == null)
            throw new ArgumentNullException(nameof(factory));

        if (ZeepSettingsConfigEntryTypeDrawerRegistry.IsRegistered(settingType))
        {
            Logger.LogWarning(
                $"Replacing an existing config entry type drawer for '{settingType.FullName}'.");
        }

        ZeepSettingsConfigEntryTypeDrawerRegistry.Register(settingType, factory);
    }

    /// <summary>
    /// Removes the global drawer factory for the given setting type.
    /// </summary>
    /// <typeparam name="T">The config entry value type.</typeparam>
    public static void ClearConfigEntryTypeDrawer<T>() => ClearConfigEntryTypeDrawer(typeof(T));

    /// <summary>
    /// Removes the global drawer factory for the given setting type.
    /// </summary>
    /// <param name="settingType">The config entry value type.</param>
    public static void ClearConfigEntryTypeDrawer(Type settingType)
    {
        if (settingType == null)
            throw new ArgumentNullException(nameof(settingType));

        ZeepSettingsConfigEntryTypeDrawerRegistry.Clear(settingType);
    }

    internal static void DispatchWindowOpened()
    {
        ModSettingsWindowOpened?.Invoke();
    }

    internal static void DispatchWindowClosed()
    {
        ModSettingsWindowClosed?.Invoke();
    }

    private static bool TryValidateConfigEntry(BaseUnityPlugin plugin, ConfigEntryBase entry)
    {
        foreach ((_, ConfigEntryBase configEntry) in plugin.Config)
        {
            if (configEntry == entry)
                return true;
        }

        Logger.LogWarning(
            $"Config entry '{entry.Definition.Section}.{entry.Definition.Key}' does not belong to plugin '{plugin.Info.Metadata.GUID}'. Registration was skipped.");
        return false;
    }
}
