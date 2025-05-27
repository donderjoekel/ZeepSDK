using BepInEx;
using UnityEngine;
using ZeepkistClient;
using ZeepSDK.UI;

namespace ZeepSDK.Settings;

public static class SettingsApi
{
    /// <summary>
    /// Invoked whenever the mod settings window opens
    /// </summary>
    public static event ModSettingsWindowOpenedDelegate ModSettingsWindowOpened;
    /// <summary>
    /// Invoked whenever the mod settings window closes
    /// </summary>
    public static event ModSettingsWindowClosedDelegate ModSettingsWindowClosed;
    /// <summary>
    /// Invoked whenever the active mod settings tab is switched
    /// </summary>
    public static event ModSettingsTabSwitchedDelegate ModSettingsTabSwitched;
    
    internal static void Initialize()
    {
        UIApi.AddToolbarItemChild("File", "Settings", OpenModSettings, int.MinValue);
        ZeepkistNetwork.LobbyGameStateChanged += CloseModSettings;
    }
    
    /// <summary>
    /// Opens the mod settings if it isn't already open
    /// </summary>
    public static void OpenModSettings()
    {
        ZeepGUIWindow.Open<ZeepSettingsWindow>(true);
        DispatchWindowOpened();
    }

    /// <summary>
    /// Closes the mod settings if it is already open
    /// </summary>
    public static void CloseModSettings()
    {
        ZeepSettingsWindow zeepSettingsWindow = Object.FindObjectOfType<ZeepSettingsWindow>();
        if (zeepSettingsWindow != null)
        {
            zeepSettingsWindow.Close();
            DispatchWindowClosed();
        }

        KeyBindWindow keyBindWindow = Object.FindObjectOfType<KeyBindWindow>();
        if (keyBindWindow != null)
            keyBindWindow.Close();
    }

    internal static void DispatchWindowOpened()
    {
        ModSettingsWindowOpened?.Invoke();
    }

    internal static void DispatchWindowClosed()
    {
        ModSettingsWindowClosed?.Invoke();
    }

    internal static void DispatchTabChanged(PluginInfo from, PluginInfo to)
    {
        ModSettingsTabSwitched?.Invoke(from, to);
    }
}