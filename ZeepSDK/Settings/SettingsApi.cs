using BepInEx;
using JetBrains.Annotations;
using ZeepkistClient;
using ZeepSDK.UI;

namespace ZeepSDK.Settings;

/// <summary>
/// Provides APIs for managing the mod settings window.
/// </summary>
[PublicAPI]
public static class SettingsApi
{
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

    internal static void DispatchWindowOpened()
    {
        ModSettingsWindowOpened?.Invoke();
    }

    internal static void DispatchWindowClosed()
    {
        ModSettingsWindowClosed?.Invoke();
    }
}
