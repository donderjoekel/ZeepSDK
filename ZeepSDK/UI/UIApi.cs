using System;
using System.Collections.Generic;
using BepInEx.Logging;
using JetBrains.Annotations;
using UnityEngine;
using ZeepSDK.Extensions;
using ZeepSDK.UI.Patches;
using ZeepSDK.Utilities;

namespace ZeepSDK.UI;

/// <summary>
/// An API related to the UI of the game
/// </summary>
[PublicAPI]
public static class UIApi
{
    private static readonly ManualLogSource Logger = LoggerFactory.GetLogger(typeof(UIApi));
    private static UIConfigurator _uiConfigurator;
    private static ZeepGUI _zeepGUI;
    private static ZeepToolbar _zeepToolbar;
    private static ZeepTooltip _zeepTooltip;

    public const string FileTitle = "File";

    internal static void Initialize(GameObject gameObject)
    {
        _uiConfigurator = gameObject.AddComponent<UIConfigurator>();
        _zeepGUI = gameObject.AddComponent<ZeepGUI>();
        _zeepToolbar = new ZeepToolbar();
        _zeepGUI.AddZeepGUIDrawer(_zeepToolbar);
        _zeepTooltip = new ZeepTooltip();
        _zeepGUI.AddZeepGUIDrawer(_zeepTooltip);

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
        Transform dslrRect = flyingCameraGUI.transform.Find("DSLR Rect");

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

        if (dslrRect != null)
        {
            AddToConfigurator(dslrRect.GetComponentsInDirectDescendants<RectTransform>());
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

        _uiConfigurator.Add(rectTransform);
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
        _uiConfigurator.Remove(rectTransform);
    }

    /// <summary>
    /// Adds a ZeepGUI drawer that will be called during the GUI rendering phase
    /// </summary>
    /// <param name="drawer">The drawer to add</param>
    /// <exception cref="ArgumentNullException">Thrown when drawer is null</exception>
    public static void AddZeepGUIDrawer(IZeepGUIDrawer drawer)
    {
        if (drawer == null)
            throw new ArgumentNullException(nameof(drawer));
        _zeepGUI.AddZeepGUIDrawer(drawer);
    }

    /// <summary>
    /// Removes a ZeepGUI drawer from the rendering phase
    /// </summary>
    /// <param name="drawer">The drawer to remove</param>
    /// <exception cref="ArgumentNullException">Thrown when drawer is null</exception>
    public static void RemoveZeepGUIDrawer(IZeepGUIDrawer drawer)
    {
        if (drawer == null)
            throw new ArgumentNullException(nameof(drawer));
        _zeepGUI.RemoveZeepGUIDrawer(drawer);
    }

    /// <summary>
    /// Adds a toolbar drawer that will be rendered in the Zeep toolbar
    /// </summary>
    /// <param name="drawer">The toolbar drawer to add</param>
    /// <exception cref="ArgumentNullException">Thrown when drawer is null</exception>
    public static void AddToolbarDrawer(IZeepToolbarDrawer drawer)
    {
        if (drawer == null)
            throw new ArgumentNullException(nameof(drawer));
        _zeepToolbar.AddToolbarDrawer(drawer);
    }

    /// <summary>
    /// Removes a toolbar drawer from the Zeep toolbar
    /// </summary>
    /// <param name="drawer">The toolbar drawer to remove</param>
    /// <exception cref="ArgumentNullException">Thrown when drawer is null</exception>
    public static void RemoveToolbarDrawer(IZeepToolbarDrawer drawer)
    {
        if (drawer == null)
            throw new ArgumentNullException(nameof(drawer));
        _zeepToolbar.RemoveToolbarDrawer(drawer);
    }
    
    /// <summary>
    /// Adds a tooltip to the game object with the specified text
    /// The tooltip will show up when the mouse is over the game object
    /// </summary>
    public static void AddTooltip(GameObject gameObject, string text)
    {
        var tooltipper = gameObject.AddComponent<Tooltipper>();
        tooltipper.Initialize(text);
        _zeepTooltip.AddTooltip(tooltipper);
    }

    /// <summary>
    /// Removes a tooltip from the specified game object
    /// </summary>
    /// <param name="gameObject">The game object to remove the tooltip from</param>
    public static void RemoveTooltip(GameObject gameObject)
    {
        var tooltipper = gameObject.GetComponent<Tooltipper>();
        if (tooltipper != null)
        {
            _zeepTooltip.RemoveTooltip(tooltipper);
        }
    }
}
