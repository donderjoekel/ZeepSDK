using System.Collections.Generic;
using BepInEx.Logging;
using UnityEngine;
using ZeepSDK.Extensions;
using ZeepSDK.UI.Patches;
using ZeepSDK.Utilities;

namespace ZeepSDK.UI;

/// <summary>
/// An API related to the UI of the game
/// </summary>
public static class UIApi
{
    private static readonly ManualLogSource logger = LoggerFactory.GetLogger(typeof(UIApi));
    private static UIConfigurator uiConfigurator;
    
    internal static void Initialize(GameObject gameObject)
    {
        uiConfigurator = gameObject.AddComponent<UIConfigurator>();
        
        OnlineChatUI_Awake.Awake += OnOnlineChatUIAwake;
        OnlineGameplayUI_Awake.Awake += OnOnlineGameplayUIAwake;
        PlayerScreensUI_Awake.Awake += OnPlayerScreensUIAwake;
        SpectatorCameraUI_Awake.Awake += OnSpectatorCameraUIAwake;
    }
    
    private static void OnOnlineChatUIAwake(OnlineChatUI instance)
    {
        AddToConfigurator(instance.transform.GetComponentsInDirectDescendants<RectTransform>());
    }
    
    private static void OnOnlineGameplayUIAwake(OnlineGameplayUI instance)
    {
        Transform gameplayUi = instance.transform.GetChild(0);
        if (gameplayUi == null)
            return;
        
        AddToConfigurator(gameplayUi.GetComponentsInDirectDescendants<RectTransform>());
    }
    
    private static void OnPlayerScreensUIAwake(PlayerScreensUI instance)
    {
        Transform playerPanel = instance.transform.GetChild(0);
        if (playerPanel == null)
            return;
        
        IEnumerable<RectTransform> rectTransforms = playerPanel.GetComponentsInDirectDescendants<RectTransform>();
        foreach (RectTransform rectTransform in rectTransforms)
        {
            string name = rectTransform.name.ToLowerInvariant();
            switch (name)
            {
                case "checkpointspanel":
                case "image":
                case "debug":
                case "center shower":
                case "wr (for saty)":
                    break;
                default:
                    AddToConfigurator(rectTransform);
                    break;
            }
        }
    }
    
    private static void OnSpectatorCameraUIAwake(SpectatorCameraUI instance)
    {
        Transform guiHolder = instance.transform.Find("GUI Holder");
        Transform flyingCameraGUI = guiHolder.transform.Find("Flying Camera GUI");
        Transform smallLeaderboardHolder = guiHolder.transform.Find("Small Leaderboard Holder (false)");
        Transform DSLRRect = flyingCameraGUI.transform.Find("DSLR Rect");
        
        if (smallLeaderboardHolder != null)
        {
            Transform smallLeaderboard = smallLeaderboardHolder.GetChild(0);
            
            if (smallLeaderboard != null)
            {
                if (smallLeaderboard.TryGetComponent(out RectTransform rectTransform))
                {
                    AddToConfigurator(rectTransform);
                }
            }
        }
        
        if (DSLRRect != null)
        {
            AddToConfigurator(DSLRRect.GetComponentsInDirectDescendants<RectTransform>());
        }
    }
    
    /// <summary>
    /// Allows a rect transform to be configured with the UI Configurator
    /// </summary>
    /// <param name="rectTransform"></param>
    public static void AddToConfigurator(RectTransform rectTransform)
    {
        if (rectTransform == null)
            return;
        
        uiConfigurator.Add(rectTransform);
    }
    
    /// <summary>
    /// Allows a collection of rect transforms to be configured with the UI Configurator
    /// </summary>
    /// <param name="rectTransforms"></param>
    public static void AddToConfigurator(IEnumerable<RectTransform> rectTransforms)
    {
        foreach (RectTransform rectTransform in rectTransforms)
        {
            AddToConfigurator(rectTransform);
        }
    }
    
    /// <summary>
    /// Removes a rect transform from the UI Configurator
    /// </summary>
    /// <param name="rectTransform"></param>
    public static void RemoveFromConfigurator(RectTransform rectTransform)
    {
        uiConfigurator.Remove(rectTransform);
    }
}
