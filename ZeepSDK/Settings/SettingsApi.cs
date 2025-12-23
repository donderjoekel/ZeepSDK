using ZeepkistClient;
using ZeepSDK.UI;

namespace ZeepSDK.Settings;

/// <summary>
/// Provides APIs for managing the mod settings window.
/// </summary>
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
        // UIApi.AddToolbarItemChild("File", "Settings", OpenModSettings, int.MinValue);
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

    internal static void DispatchWindowOpened()
    {
        ModSettingsWindowOpened?.Invoke();
    }

    internal static void DispatchWindowClosed()
    {
        ModSettingsWindowClosed?.Invoke();
    }
}