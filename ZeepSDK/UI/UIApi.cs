using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx.Logging;
using UnityEngine;
using UnityEngine.UI;
using ZeepSDK.Extensions;
using ZeepSDK.UI.Patches;
using ZeepSDK.Utilities;
using Object = UnityEngine.Object;

namespace ZeepSDK.UI;

/// <summary>
/// An API related to the UI of the game
/// </summary>
public static class UIApi
{
    private static readonly ManualLogSource logger = LoggerFactory.GetLogger(typeof(UIApi));
    private static UIConfigurator uiConfigurator;
    private static Tooltip _tooltip;

    internal static void Initialize(GameObject gameObject)
    {
        gameObject.AddComponent<ZeepGUIDispatcher>();
        
        string directoryName = Path.GetDirectoryName(Plugin.Instance.Info.Location);
        string assetBundlePath = Path.Combine(directoryName, "zeepgui");
        AssetBundle assetBundle = AssetBundle.LoadFromFile(assetBundlePath);
        ZeepGUI.Skins = assetBundle.LoadAllAssets<GUISkin>();
        Plugin.Instance.GuiSkinConfig.SettingChanged += OnSelectedSkinChanged;
        OnSelectedSkinChanged(null, null); // Apply the current skin

        ZeepGUI.AddToolbarItem(ZeepGUI.FileToolbarItem, null, int.MinValue);
        // ZeepGUI.AddToolbarItemChild(ZeepGUI.FileToolbarItem, ZeepGUI.FileSettingsToolbarItem, OpenWindow, int.MinValue);
        ZeepGUI.AddToolbarItemChild(ZeepGUI.FileToolbarItem, ZeepGUI.FileExitToolbarItem, Application.Quit, int.MaxValue);
        
        uiConfigurator = gameObject.AddComponent<UIConfigurator>();
        CreateTooltip();

        OnlineChatUI_Awake.Awake += OnOnlineChatUIAwake;
        OnlineGameplayUI_Awake.Awake += OnOnlineGameplayUIAwake;
        PlayerScreensUI_Awake.Awake += OnPlayerScreensUIAwake;
        SpectatorCameraUI_Awake.Awake += OnSpectatorCameraUIAwake;
    }

    private static void OnSelectedSkinChanged(object o, EventArgs eventArgs)
    {
        ZeepGUI.CurrentSkin = Plugin.Instance.GuiSkinConfig.Value switch
        {
            GuiSkins.Default => ZeepGUI.Skins.First(x => x.name == "Default"),
            GuiSkins.SemiTransparent => ZeepGUI.Skins.First(x => x.name == "Semi Transparent"),
            _ => throw new ArgumentOutOfRangeException()
        };
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

    /// <summary>
    /// Adds a tooltip to the game object with the specified text
    /// The tooltip will show up when the mouse is over the game object
    /// </summary>
    public static void AddTooltip(GameObject gameObject, string text)
    {
        CreateTooltip();
        gameObject.AddComponent<Tooltipper>().Initialize(text);
    }

    private static void CreateTooltip()
    {
        if (_tooltip != null)
            return;

        GameObject canvas = new("Tooltip Canvas", typeof(RectTransform));
        Object.DontDestroyOnLoad(canvas);
        Canvas tooltipCanvas = canvas.AddComponent<Canvas>();
        tooltipCanvas.sortingOrder = short.MaxValue; // Internally sortingOrder is a signed short, not an int
        tooltipCanvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = canvas.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 1;

        GameObject tooltipHolder = new(
            "Tooltip",
            typeof(RectTransform),
            typeof(Image),
            typeof(CanvasGroup),
            typeof(VerticalLayoutGroup),
            typeof(ContentSizeFitter),
            typeof(Tooltip));
        _tooltip = tooltipHolder.GetComponent<Tooltip>();
        _tooltip.transform.SetParent(canvas.transform);
    }

    internal static void ShowTooltip(string text)
    {
        _tooltip.Show(text);
    }

    internal static void HideTooltip()
    {
        _tooltip.Hide();
    }
}
