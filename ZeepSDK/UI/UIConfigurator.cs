using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx.Configuration;
using BepInEx.Logging;
using MonoMod.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using ZeepSDK.Messaging;
using ZeepSDK.UI.Patches;
using ZeepSDK.Utilities;
using ColorUtility = ZeepSDK.Utilities.ColorUtility;

namespace ZeepSDK.UI;

internal class UIConfigurator : MonoBehaviour
{
    private const float AnimatedBorderFadeDuration = 1.0f;

    private const string MoveHandle =
        "iVBORw0KGgoAAAANSUhEUgAAACAAAAAgCAIAAAD8GO2jAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAADfSURBVEhL3ZVBEoMgDEVtj9JTwVU4LByFxoZRCRR+EBbtW8kI7w8k6CPGuK1kDwghpNFsnHPP9Ijx+pAGGIqAQ63KQAOEFM+AAqo6MKMf0BAhGbouYqP3nodd1F00wF8GgO1RUl0oA4btTLk8C7hpZ4TkDJhiZzIV3QO8r1WQ1hizvIvOm3zd17c98Zz2W4bnZDd54kFdVdkRTckQElmDmxnl8kqRhzOqC5d30e8H9P9o7U9Iu2DQH62hQNoBOqKqCLETaA2EDrQTiiIfUtxO6LqI1Co7sXeRtTaNprNtb4jdcZdwHID6AAAAAElFTkSuQmCC";

    private const string ScaleHandle =
        "iVBORw0KGgoAAAANSUhEUgAAACAAAAAgCAIAAAD8GO2jAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAADASURBVEhL3ZZBEsIgEATRp/ArvsJj4Sk4FpRGxAV2Nh7ShxRcumG55FZKcWfyDOSc286aGOO9LU+DvYH3vq0+SSnh+48bsIF6UgGDG8gNg8CvZ6iwAdkOqEBnH85KHxjavxvKgHB2rI9bTWBlMi+2A1t2sBfYtYONgMIOVgM6O1gKqO1gHmDsYBIg7UAK8HbwDkB3NJrYQX+D6rWyg8GIDO1g8sikHUgB3g6kQDcrHZMR8Y1JgOca/6YhhLYzx7kHwO9hWRWBT8gAAAAASUVORK5CYII=";

    private const string ResetHandle =
        "iVBORw0KGgoAAAANSUhEUgAAACAAAAAgCAIAAAD8GO2jAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAADZSURBVEhL7ZbhDYQgDEbxRmErVmFYGAWrNoZAKf2I5nLJvV8UaZ8FTdhKKe5NDkHOmaOniTF+ePgasMB7zyMbmACtTgCCheqEVbBWnTAJmuoUXnCsAh9yjUVjEqSUeHRCYT2jO6wdNA6i1igOYIt6BzF1YGew4IAPWXf0CILph6HQ57aC5eqjJuAtQvkLpnxJsPAtjVJagfLLWOjTh1sENaEsFgT3Wxgd9zKxe7kDu0OvTgy3qHaImnp+VJ2Y3OzE0g1K9fnNjpKVfP3pxe/fTY8OQggcPY5zO9a7a/F3f5AmAAAAAElFTkSuQmCC";

    private static Texture2D WhiteTexture;

    private static ManualLogSource logger = LoggerFactory.GetLogger(typeof(UIConfigurator));

    private readonly List<RectTransform> transforms = new();
    private readonly Dictionary<string, TransformSaveData> transformSaveData = new();

    private ConfigEntry<KeyCode> configEditModeKey;
    private ConfigEntry<KeyCode> configNextCycleKey;
    private ConfigEntry<KeyCode> configPreviousCycleKey;
    private ConfigEntry<bool> configResetAllButton;
    private ConfigEntry<string> configBorderColor;

    private bool previousCursorVisible;
    private CursorLockMode previousCursorLockMode;
    private bool previousGameObjectActive;
    private Vector2 previousMousePosition;

    private bool isEditing;
    private bool isRectEditing;
    private bool isMoving;
    private bool isScaling;

    private Color borderColor = Color.red;
    private Color animatedBorderColor;

    private bool hasSetPlaceholderText;

    private GameObject moveHandle;
    private GameObject scaleHandle;
    private GameObject resetHandle;

    private RectTransform moveHandleRect;
    private RectTransform scaleHandleRect;
    private RectTransform resetHandleRect;
    private Vector3[] corners = new Vector3[4];

    private int currentRectIndex;
    private RectTransform currentRect;

    private void Awake()
    {
        WhiteTexture = Texture2D.whiteTexture;
        
        DontDestroyOnLoad(gameObject);
        RegisterConfig();
        LoadSaveData();

        SceneManager_LoadScene.BeforeLoadScene += () => DisableEditMode();
    }

    private void RegisterConfig()
    {
        configEditModeKey = Plugin.Instance.Config.Bind(
            "UI",
            "Toggle Edit Mode Key",
            KeyCode.Keypad8,
            "The key to toggle UI edit mode");
        configNextCycleKey = Plugin.Instance.Config.Bind(
            "UI",
            "Cycle Next Key",
            KeyCode.Keypad6,
            "The key to cycle to the next UI element");
        configPreviousCycleKey = Plugin.Instance.Config.Bind(
            "UI",
            "Cycle Previous Key",
            KeyCode.Keypad4,
            "The key to cycle to the previous UI element");
        configResetAllButton = Plugin.Instance.Config.Bind(
            "UI",
            "Reset All",
            true,
            "[Button] Reset All UI");
        configBorderColor = Plugin.Instance.Config.Bind(
            "UI",
            "Border Color",
            "Red",
            new ConfigDescription(
                "Selected UI element color",
                new AcceptableValueList<string>(ColorUtility.ColorDefinitions.Select(x => x.Name).ToArray())));

        configResetAllButton.SettingChanged += (sender, args) => ResetAll();
        configBorderColor.SettingChanged += (sender, args) => SetColor(configBorderColor.Value);
        SetColor(configBorderColor.Value);
    }

    private void SetColor(string colorName)
    {
        borderColor = ColorUtility.FromName(colorName);
        animatedBorderColor = borderColor;
    }

    private void LoadSaveData()
    {
        try
        {
            Dictionary<string, TransformSaveData> loadedSaveData =
                Plugin.Storage.LoadFromJson<Dictionary<string, TransformSaveData>>("UIConfigurator");

            if (loadedSaveData != null)
            {
                transformSaveData.AddRange(loadedSaveData);
            }
        }
        catch (FileNotFoundException)
        {
            // Can be ignored
        }
    }

    private void CleanDeadTransforms()
    {
        transforms.RemoveAll(x => x == null);
    }

    public void Add(RectTransform rectTransform)
    {
        if (transforms.Contains(rectTransform))
            return;

        CleanDeadTransforms();
        transforms.Add(rectTransform);
        ApplyOrSaveOriginal(rectTransform);
    }

    public void Remove(RectTransform rectTransform)
    {
        transforms.Remove(rectTransform);
        CleanDeadTransforms();
    }

    private void OnGUI()
    {
        if (!isEditing)
            return;

        DrawOuterBorder();

        if (currentRect == null)
            return;

        DrawCurrentRectBorder();
    }

    private void DrawOuterBorder()
    {
        Rect screenborderRect = new Rect(0, 0, Screen.width, Screen.height);
        DrawScreenRectBorder(screenborderRect, 8, animatedBorderColor);
    }

    private void DrawCurrentRectBorder()
    {
        Vector3[] corners = new Vector3[4];
        currentRect.GetWorldCorners(corners);

        Canvas canvas = currentRect.GetComponentInParent<Canvas>();
        if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            for (int i = 0; i < 4; i++)
            {
                corners[i] = RectTransformUtility.WorldToScreenPoint(null, corners[i]);
            }
        }
        else
        {
            Camera camera = canvas.worldCamera != null ? canvas.worldCamera : Camera.main;
            for (int i = 0; i < 4; i++)
            {
                corners[i] = RectTransformUtility.WorldToScreenPoint(camera, corners[i]);
            }
        }

        Rect screenRect = GetScreenRect(corners[0], corners[2]);
        DrawScreenRectBorder(screenRect, 2, borderColor);
    }

    private void Update()
    {
        if (Input.GetKeyDown(configEditModeKey.Value))
        {
            ToggleEditMode();
        }

        if (!isEditing)
            return;

        if (isRectEditing)
        {
            if (Input.GetKeyDown(configNextCycleKey.Value))
            {
                CycleToNext();
            }

            if (Input.GetKeyDown(configPreviousCycleKey.Value))
            {
                CycleToPrevious();
            }

            HandleRectEditing();
        }

        if (Input.GetMouseButtonUp(0))
        {
            isMoving = false;
            isScaling = false;
        }

        PulseAnimatedBorder();
    }

    private void CycleToNext()
    {
        if (!transforms.Any())
            return;

        ExitRectEditMode();
        CleanDeadTransforms();

        RectTransform rectTransform =
            ListUtility.FindFirst(transforms, currentRectIndex + 1, x => x.gameObject.activeInHierarchy);

        if (rectTransform != null)
        {
            EnterRectEditMode(rectTransform);
        }
        else
        {
            MessengerApi.Log("No configurable UI available");
        }
    }

    private void CycleToPrevious()
    {
        if (!transforms.Any())
            return;

        ExitRectEditMode();
        CleanDeadTransforms();

        RectTransform rectTransform =
            ListUtility.FindFirstReverse(transforms, currentRectIndex - 1, x => x.gameObject.activeInHierarchy);

        if (rectTransform != null)
        {
            EnterRectEditMode(rectTransform);
        }
        else
        {
            MessengerApi.Log("No configurable UI available");
        }
    }

    private void HandleRectEditing()
    {
        if (currentRect == null)
            return;

        Vector2 currentMousePosition = Input.mousePosition;
        Vector2 delta = (currentMousePosition - previousMousePosition) / new Vector2(Screen.width, Screen.height);

        if (isMoving)
        {
            currentRect.anchorMin += delta;
            currentRect.anchorMax += delta;
            SetAnchors(currentRect);
        }
        else if (isScaling)
        {
            currentRect.anchorMax += delta;
            SetAnchorMax(currentRect);
        }

        currentRect.GetWorldCorners(corners);
        moveHandleRect.anchoredPosition = ConvertPosition(corners[0]);
        scaleHandleRect.anchoredPosition = ConvertPosition(corners[2]);
        resetHandleRect.anchoredPosition = ConvertPosition(corners[1]);

        previousMousePosition = currentMousePosition;
    }

    private Vector2 ConvertPosition(Vector2 position)
    {
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(null, position);
        return RectTransformUtility.ScreenPointToLocalPointInRectangle(
            currentRect.GetComponentInParent<Canvas>().GetComponent<RectTransform>(),
            screenPoint,
            null,
            out Vector2 localPoint)
            ? localPoint
            : Vector2.zero;
    }

    /// <summary>
    /// Converts a mouse position relative to the screen (0,0 to 1,1) to a position relative to a given panel.
    /// </summary>
    /// <param name="mousePosition">Mouse position in screen coordinates (0,0 to 1,1).</param>
    /// <param name="panelStartAnchor">Panel start anchor in screen coordinates (0,0 to 1,1).</param>
    /// <param name="panelEndAnchor">Panel end anchor in screen coordinates (0,0 to 1,1).</param>
    /// <returns>Position relative to the panel.</returns>
    public static Vector2 ConvertMousePositionToPanelPosition(
        Vector2 mousePosition,
        Vector2 panelStartAnchor,
        Vector2 panelEndAnchor
    )
    {
        // Calculate the size of the panel in screen coordinates
        Vector2 panelSize = panelEndAnchor - panelStartAnchor;

        // Calculate the position relative to the panel
        Vector2 relativePosition = (mousePosition - panelStartAnchor) / panelSize;

        return relativePosition;
    }

    private void SetAnchors(RectTransform rectTransform)
    {
        string path = GetPath(rectTransform);
        if (transformSaveData.TryGetValue(path, out TransformSaveData data))
        {
            data.currentAnchorMin = rectTransform.anchorMin;
            data.currentAnchorMax = rectTransform.anchorMax;
        }
    }

    private void SetAnchorMax(RectTransform rectTransform)
    {
        string path = GetPath(rectTransform);
        if (transformSaveData.TryGetValue(path, out TransformSaveData data))
        {
            data.currentAnchorMax = rectTransform.anchorMax;
        }
    }

    private void ToggleEditMode()
    {
        if (isEditing)
        {
            DisableEditMode();
        }
        else
        {
            EnableEditMode();
        }
    }

    private void EnableEditMode()
    {
        if (isEditing)
            return;

        CleanDeadTransforms();
        isEditing = true;

        previousCursorVisible = Cursor.visible;
        previousCursorLockMode = Cursor.lockState;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        RectTransform rectTransform = GetFirstActiveRectTransform();
        if (rectTransform != null)
        {
            EnterRectEditMode(rectTransform);
        }
        else
        {
            MessengerApi.Log("No configurable UI available");
            DisableEditMode(false);
        }
    }

    private void DisableEditMode(bool saveAndLog = true)
    {
        if (!isEditing)
            return;

        ExitRectEditMode();

        isEditing = false;

        Cursor.visible = previousCursorVisible;
        Cursor.lockState = previousCursorLockMode;

        if (saveAndLog)
        {
            MessengerApi.Log("Saving UIConfig!");
            SaveToDisk();
        }

        DeactivateHandles();

        if (currentRect != null)
        {
            currentRect.gameObject.SetActive(previousGameObjectActive);
        }

        isRectEditing = false;
        currentRect = null;
        currentRectIndex = -1;
    }

    private void EnterRectEditMode(RectTransform rectTransform)
    {
        currentRect = rectTransform;
        currentRectIndex = transforms.IndexOf(currentRect);

        previousGameObjectActive = currentRect.gameObject.activeSelf;
        SetPlaceholderTextIfNecessary();
        currentRect.gameObject.SetActive(true);

        isRectEditing = true;
        ActivateHandles(currentRect);
        MessengerApi.Log(currentRect.gameObject.name);
    }

    private void ExitRectEditMode()
    {
        if (!isRectEditing)
            return;

        if (hasSetPlaceholderText)
        {
            if (currentRect != null)
            {
                currentRect.GetComponentInChildren<TextMeshProUGUI>().text = "";
            }

            hasSetPlaceholderText = false;
        }

        if (currentRect != null)
        {
            currentRect.gameObject.SetActive(previousGameObjectActive);
        }

        isRectEditing = false;
        DeactivateHandles();
        currentRect = null;
    }

    private void SetPlaceholderTextIfNecessary()
    {
        TextMeshProUGUI text = currentRect.GetComponentInChildren<TextMeshProUGUI>();
        if (text == null)
            return;

        if (!string.IsNullOrEmpty(text.text))
            return;

        text.text = GetPlaceholderText(currentRect);
        hasSetPlaceholderText = true;
    }

    private void ActivateHandles(RectTransform rect)
    {
        if (moveHandle == null)
        {
            moveHandle = CreateHandle("MoveHandle", StartMove);
            moveHandleRect = moveHandle.GetComponent<RectTransform>();
        }

        if (scaleHandle == null)
        {
            scaleHandle = CreateHandle("ScaleHandle", StartScale);
            scaleHandleRect = scaleHandle.GetComponent<RectTransform>();
        }

        if (resetHandle == null)
        {
            resetHandle = CreateHandle("ResetHandle", ResetToOriginal);
            resetHandleRect = resetHandle.GetComponent<RectTransform>();
        }

        PositionHandle(moveHandle, rect, new Vector2(0, 0));
        PositionHandle(scaleHandle, rect, new Vector2(1, 1));
        PositionHandle(resetHandle, rect, new Vector2(0, 1));
    }

    private void DeactivateHandles()
    {
        if (moveHandle != null)
            moveHandle.SetActive(false);
        if (scaleHandle != null)
            scaleHandle.SetActive(false);
        if (resetHandle != null)
            resetHandle.SetActive(false);
    }

    private GameObject CreateHandle(string name, UnityAction<RectTransform> callback)
    {
        GameObject handle = new GameObject(name);
        RectTransform handleRect = handle.AddComponent<RectTransform>();
        handleRect.sizeDelta = new Vector2(32, 32);

        Image image = handle.AddComponent<Image>();
        image.color = Color.white;

        Sprite s = SpriteUtility.FromBase64(GetBase64ForName(name));
        if (s != null)
        {
            image.sprite = s;
        }

        EventTrigger trigger = handle.AddComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerDown;
        entry.callback.AddListener((data) => callback(currentRect));
        trigger.triggers.Add(entry);

        handle.SetActive(false);
        return handle;
    }

    private static string GetBase64ForName(string name)
    {
        return name switch
        {
            "MoveHandle" => MoveHandle,
            "ScaleHandle" => ScaleHandle,
            "ResetHandle" => ResetHandle,
            _ => null
        };
    }

    private void StartMove(RectTransform rect)
    {
        currentRect = rect;
        isMoving = true;
        isScaling = false;
    }

    private void StartScale(RectTransform rect)
    {
        currentRect = rect;
        isScaling = true;
        isMoving = false;
    }

    private void ApplyOrSaveOriginal(RectTransform rectTransform)
    {
        string path = GetPath(rectTransform);
        if (transformSaveData.TryGetValue(path, out TransformSaveData data))
        {
            Apply(rectTransform, data);
        }
        else
        {
            SaveOriginal(rectTransform);
        }
    }

    private void Apply(RectTransform rect, TransformSaveData data)
    {
        rect.anchorMin = data.currentAnchorMin;
        rect.anchorMax = data.currentAnchorMax;
    }

    private void SaveOriginal(RectTransform rect)
    {
        string path = GetPath(rect);
        TransformSaveData data = new TransformSaveData
        {
            originalAnchorMin = rect.anchorMin,
            originalAnchorMax = rect.anchorMax,
            currentAnchorMin = rect.anchorMin,
            currentAnchorMax = rect.anchorMax
        };
        transformSaveData[path] = data;
    }

    private void ResetAll()
    {
        foreach (RectTransform rectTransform in transforms)
        {
            ResetToOriginal(rectTransform);
        }

        SaveToDisk();
        MessengerApi.Log("All UI elements have been reset!");
    }

    private void ResetToOriginal(RectTransform rect)
    {
        string path = GetPath(rect);
        if (transformSaveData.TryGetValue(path, out TransformSaveData data))
        {
            rect.anchorMin = data.originalAnchorMin;
            rect.anchorMax = data.originalAnchorMax;
            data.currentAnchorMin = rect.anchorMin;
            data.currentAnchorMax = rect.anchorMax;
        }
    }

    private void SaveToDisk()
    {
        Plugin.Storage.SaveToJson("UIConfigurator", transformSaveData);
    }

    private void PositionHandle(GameObject handle, RectTransform parent, Vector2 anchor)
    {
        handle.transform.SetParent(PlayerManager.Instance.gameObject.transform.Find("Canvas"), false);
        RectTransform handleRect = handle.GetComponent<RectTransform>();
        handleRect.anchorMin = handleRect.anchorMax = handleRect.pivot = new Vector2(0.5f, 0.5f);
        handleRect.anchoredPosition = Vector2.zero;
        handle.SetActive(true);
    }

    private RectTransform GetFirstActiveRectTransform()
    {
        return transforms.FirstOrDefault(x => x.gameObject.activeInHierarchy);
    }

    private void PulseAnimatedBorder()
    {
        float alpha = Mathf.PingPong(Time.time / AnimatedBorderFadeDuration, 1.0f);
        animatedBorderColor = new Color(animatedBorderColor.r, animatedBorderColor.g, animatedBorderColor.b, alpha);
    }

    private string GetPlaceholderText(RectTransform rect)
    {
        string rectName = rect.name.ToLower();
        switch (rectName)
        {
            //Player screen
            case "timeout title":
                return "Too Slow!\nPress Right Shift to quick reset!";
            case "timeout countdown":
                return "X.X";
            case "results position":
                return "Results Position Placeholder";
            case "results time":
                return "XX:XX.XXX";
            case "results checkpoints":
                return "X/X Checkpoints";
            case "results press to continue":
                return "Press Right Shift to continue!";
            case "velocity":
                return "888";
            case "checkpoints":
                return "X/X";
            case "time":
                return "XX:XX.XXX";

            //Photomode
            case "target":
                return "Target: Player 1";
            case "mode":
                return "Mode: Free";
            case "fov":
                return "60°";
            case "level":
                return "Level X by Bouwerman";
            case "time left holder":
                return "XX:XX";
            case "tooltips":
                return "F1: Show Controls\nF2: Toggle Camera GUI\nF3: Toggle Small Leaderboard";
            case "velocitypanel":
                return "XXX - XX:XX.XXX";
            case "target steam id":
                return "Steam ID: XXXXXXXXXXXXXXXXXX";
            case "draw players indicator":
                return "Show Player: state (V)";

            //Online
            case "voteskip text":
            case "voteskip text alternate position":
                return "voteskip (X/X)";
            case "servermessage":
            case "servermessage alternate position":
                return "Placeholder text for server message.";
        }

        return "";
    }

    private static string GetPath(Transform transform)
    {
        if (transform == null) return null;
        string path = GetPath(transform.parent);
        return string.IsNullOrEmpty(path) ? transform.name : $"{path}/{transform.name}";
    }

    private static void DrawScreenRect(Rect rect, Color color)
    {
        Color previousColor = GUI.color;
        GUI.color = color;
        GUI.DrawTexture(rect, WhiteTexture);
        GUI.color = previousColor;
    }

    private static void DrawScreenRectBorder(Rect rect, float thickness, Color color)
    {
        // Top
        DrawScreenRect(new Rect(rect.xMin, rect.yMin, rect.width, thickness), color);
        // Left
        DrawScreenRect(new Rect(rect.xMin, rect.yMin, thickness, rect.height), color);
        // Right
        DrawScreenRect(new Rect(rect.xMax - thickness, rect.yMin, thickness, rect.height), color);
        // Bottom
        DrawScreenRect(new Rect(rect.xMin, rect.yMax - thickness, rect.width, thickness), color);
    }

    private static Rect GetScreenRect(Vector3 screenPosition1, Vector3 screenPosition2)
    {
        // Move origin from bottom left to top left
        screenPosition1.y = Screen.height - screenPosition1.y;
        screenPosition2.y = Screen.height - screenPosition2.y;
        // Calculate corners
        Vector3 topLeft = Vector3.Min(screenPosition1, screenPosition2);
        Vector3 bottomRight = Vector3.Max(screenPosition1, screenPosition2);
        // Create Rect
        return Rect.MinMaxRect(topLeft.x, topLeft.y, bottomRight.x, bottomRight.y);
    }

    [Serializable]
    private class TransformSaveData
    {
        public Vector2 originalAnchorMin;
        public Vector2 originalAnchorMax;
        public Vector2 currentAnchorMin;
        public Vector2 currentAnchorMax;
    }
}
