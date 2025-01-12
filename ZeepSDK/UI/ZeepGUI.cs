using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using ZeepSDK.Extensions;
using ZeepSDK.Utilities.Override;

namespace ZeepSDK.UI;

[PublicAPI]
public static class ZeepGUI
{
    public const string FileToolbarItem = "File";
    public const string FileSettingsToolbarItem = "Settings";
    public const string FileExitToolbarItem = "Exit Zeepkist";

    private static readonly Dictionary<int, Rect> _menuButtonRects = new();

    private static readonly ZeepToolbar _toolbar = new();
    private static readonly List<IZeepGUI> _receivers = [];
    private static readonly List<IZeepWindow> _windows = [];
    private static bool _hasSetBaseValue;

    internal static Vector2 MousePosition { get; private set; }
    internal static GUISkin CurrentSkin;
    internal static GUISkin[] Skins;

    public static OverrideStack<GUISkin> Skin = new(() => GUI.skin, value => GUI.skin = value, null);

    public delegate void WindowFunction(int id, bool closeClicked);

    public static void AddZeepGUI(IZeepGUI receiver)
    {
        _receivers.Add(receiver);
    }

    public static void RemoveZeepGUI(IZeepGUI receiver)
    {
        _receivers.Remove(receiver);
    }

    public static void AddToolbarItem(string content, Action clicked, int priority = 0)
    {
        AddToolbarItem(new GUIContent(content), clicked, priority);
    }

    public static void AddToolbarItem(GUIContent content, Action clicked, int priority = 0)
    {
        if (!string.Equals(content.text, FileToolbarItem, StringComparison.OrdinalIgnoreCase) &&
            priority == int.MinValue)
        {
            throw new Exception("A priority of int.MinValue is reserved for the File toolbar item");
        }

        _toolbar.AddItem(content, clicked, priority);
    }

    public static void AddToolbarItemChild(string parent, string content, Action clicked, int priority = 0)
    {
        AddToolbarItemChild(parent, new GUIContent(content), clicked, priority);
    }

    public static void AddToolbarItemChild(string parent, GUIContent content, Action clicked, int priority = 0)
    {
        if (string.Equals(parent, FileToolbarItem, StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(content.text, FileSettingsToolbarItem, StringComparison.OrdinalIgnoreCase) &&
            priority == int.MinValue)
        {
            throw new Exception("A priority of int.MinValue is reserved for the File/Settings toolbar item");
        }

        if (string.Equals(parent, FileToolbarItem, StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(content.text, FileExitToolbarItem, StringComparison.OrdinalIgnoreCase) &&
            priority == int.MaxValue)
        {
            throw new Exception("A priority of int.MaxValue is reserved for the File/Exit Zeepkist toolbar item");
        }

        _toolbar.AddChild(parent, content, clicked, priority);
    }

    public static int IntField(int value, params GUILayoutOption[] options)
    {
        string currentValue = value.ToString();
        string newValue = GUILayout.TextField(currentValue, options);
        if (currentValue != newValue && int.TryParse(newValue, out int result))
        {
            return result;
        }

        return value;
    }

    public static int IntField(int value, string label,
        GUILayoutOption[] labelOptions = null,
        GUILayoutOption[] fieldOptions = null,
        GUILayoutOption[] areaOptions = null)
    {
        using (new GUILayout.HorizontalScope(areaOptions))
        {
            GUILayout.Label(label, labelOptions);
            GUILayout.Space(8);
            return IntField(value, fieldOptions);
        }
    }

    public static int IntField(string value, params GUILayoutOption[] options)
    {
        if (int.TryParse(value, out int result))
        {
            return IntField(result, options);
        }

        throw new ArgumentException("Value is not a valid integer", nameof(value));
    }

    public static int IntField(string value, string label,
        GUILayoutOption[] labelOptions = null,
        GUILayoutOption[] fieldOptions = null,
        GUILayoutOption[] areaOptions = null)
    {
        if (int.TryParse(value, out int result))
        {
            return IntField(result, label, labelOptions, fieldOptions, areaOptions);
        }

        throw new ArgumentException("Value is not a valid integer", nameof(value));
    }

    public static float FloatField(float value, params GUILayoutOption[] options)
    {
        string currentValue = value.ToString();
        string newValue = GUILayout.TextField(currentValue, options);
        if (currentValue != newValue && float.TryParse(newValue, out float result))
        {
            return result;
        }

        return value;
    }

    public static float FloatField(float value, string label,
        GUILayoutOption[] labelOptions = null,
        GUILayoutOption[] fieldOptions = null,
        GUILayoutOption[] areaOptions = null)
    {
        using (new GUILayout.HorizontalScope(areaOptions))
        {
            GUILayout.Label(label, labelOptions);
            GUILayout.Space(8);
            return FloatField(value, fieldOptions);
        }
    }

    public static float FloatField(string value, params GUILayoutOption[] options)
    {
        if (float.TryParse(value, out float result))
        {
            return FloatField(result, options);
        }

        throw new ArgumentException("Value is not a valid float", nameof(value));
    }

    public static float FloatField(string value, string label,
        GUILayoutOption[] labelOptions = null,
        GUILayoutOption[] fieldOptions = null,
        GUILayoutOption[] areaOptions = null)
    {
        if (float.TryParse(value, out float result))
        {
            return FloatField(result, label, labelOptions, fieldOptions, areaOptions);
        }

        throw new ArgumentException("Value is not a valid float", nameof(value));
    }

    public static double DoubleField(double value, params GUILayoutOption[] options)
    {
        string currentValue = value.ToString();
        string newValue = GUILayout.TextField(currentValue, options);
        if (currentValue != newValue && double.TryParse(newValue, out double result))
        {
            return result;
        }

        return value;
    }

    public static double DoubleField(double value, string label,
        GUILayoutOption[] labelOptions = null,
        GUILayoutOption[] fieldOptions = null,
        GUILayoutOption[] areaOptions = null)
    {
        using (new GUILayout.HorizontalScope(areaOptions))
        {
            GUILayout.Label(label, labelOptions);
            GUILayout.Space(8);
            return DoubleField(value, fieldOptions);
        }
    }

    public static double DoubleField(string value, params GUILayoutOption[] options)
    {
        if (double.TryParse(value, out double result))
        {
            return DoubleField(result, options);
        }

        throw new ArgumentException("Value is not a valid double", nameof(value));
    }

    public static double DoubleField(string value, string label,
        GUILayoutOption[] labelOptions = null,
        GUILayoutOption[] fieldOptions = null,
        GUILayoutOption[] areaOptions = null)
    {
        if (double.TryParse(value, out double result))
        {
            return DoubleField(result, label, labelOptions, fieldOptions, areaOptions);
        }

        throw new ArgumentException("Value is not a valid double", nameof(value));
    }

    public static bool Toggle(bool value, string label,
        GUILayoutOption[] labelOptions = null,
        GUILayoutOption[] fieldOptions = null,
        GUILayoutOption[] areaOptions = null)
    {
        using (new GUILayout.HorizontalScope(areaOptions))
        {
            GUILayout.Label(label, labelOptions);
            GUILayout.Space(8);
            return GUILayout.Toggle(value, GUIContent.none, fieldOptions);
        }
    }

    public static string TextField(string value, string label,
        GUILayoutOption[] labelOptions = null,
        GUILayoutOption[] fieldOptions = null,
        GUILayoutOption[] areaOptions = null)
    {
        using (new GUILayout.HorizontalScope(areaOptions))
        {
            GUILayout.Label(label, labelOptions);
            GUILayout.Space(8);
            return GUILayout.TextField(value, fieldOptions);
        }
    }

    public static bool LabelButton(string label, string value,
        GUILayoutOption[] labelOptions = null,
        GUILayoutOption[] fieldOptions = null,
        GUILayoutOption[] areaOptions = null)
    {
        return LabelButton(new GUIContent(label), new GUIContent(value), labelOptions, fieldOptions, areaOptions);
    }

    public static bool LabelButton(GUIContent label, GUIContent content,
        GUILayoutOption[] labelOptions = null,
        GUILayoutOption[] fieldOptions = null,
        GUILayoutOption[] areaOptions = null)
    {
        using (new GUILayout.HorizontalScope(areaOptions))
        {
            GUILayout.Label(label, labelOptions);
            GUILayout.Space(8);
            return GUILayout.Button(content, fieldOptions);
        }
    }
    
    public static bool LabelDropdownButton(string label, string text, out ZeepDropdownMenu menu,
        GUILayoutOption[] labelOptions = null,
        GUILayoutOption[] fieldOptions = null,
        GUILayoutOption[] areaOptions = null)
    {
        using (new GUILayout.HorizontalScope(areaOptions))
        {
            GUILayout.Label(label, labelOptions);
            GUILayout.Space(8);
            return DropdownButton(text, out menu, fieldOptions);
        }
    }

    public static bool LabelDropdownButton(GUIContent label, GUIContent content, out ZeepDropdownMenu menu,
        GUILayoutOption[] labelOptions = null,
        GUILayoutOption[] fieldOptions = null,
        GUILayoutOption[] areaOptions = null)
    {
        using (new GUILayout.HorizontalScope(areaOptions))
        {
            GUILayout.Label(label, labelOptions);
            GUILayout.Space(8);
            return DropdownButton(content, out menu, fieldOptions);
        }
    }

    public static void BeginMenuBar()
    {
        GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
        GUILayout.BeginHorizontal("toolbar");
    }

    public static void EndMenuBar()
    {
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }

    public static void BeginWindowMenuBar()
    {
        GUILayout.BeginHorizontal("window toolbar");
    }

    public static void EndWindowMenuBar()
    {
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }

    public static bool DropdownButton(GUIContent content, out ZeepDropdownMenu menu, GUILayoutOption[] options = null)
    {
        return MenuButtonInternal(content, "button", out menu, options);
    }

    public static bool DropdownButton(string text, out ZeepDropdownMenu menu, GUILayoutOption[] options = null)
    {
        return DropdownButton(new GUIContent(text), out menu, options);
    }

    public static bool MenuButton(GUIContent content, out ZeepDropdownMenu menu)
    {
        return MenuButtonInternal(content, "toolbar button", out menu);
    }
    
    public static bool MenuButton(string content, out ZeepDropdownMenu menu)
    {
        return MenuButton(new GUIContent(content), out menu);
    }

    public static bool WindowMenuButton(GUIContent content, out ZeepDropdownMenu menu)
    {
        return MenuButtonInternal(content, "window toolbar button", out menu);
    }
    
    public static bool WindowMenuButton(string text, out ZeepDropdownMenu menu)
    {
        return WindowMenuButton(new GUIContent(text), out menu);
    }

    private static bool MenuButtonInternal(GUIContent content, GUIStyle style, out ZeepDropdownMenu menu, GUILayoutOption[] options = null)
    {
        int controlId = GUIUtility.GetControlID(FocusType.Keyboard);
        bool result = GUILayout.Button(content, style, options);
        if (Event.current.type == EventType.Repaint)
        {
            _menuButtonRects[controlId] = ZeepGUIUtility.ConvertToAbsolutePosition(GUILayoutUtility.GetLastRect());
        }

        if (result)
        {
            Rect rect = _menuButtonRects[controlId];
            menu = new ZeepDropdownMenu(rect.x, rect.y + rect.height);
        }
        else
        {
            menu = null;
        }

        return result;
    }

    public static Rect Window(int id, Rect clientRect, string text, GUI.WindowFunction func, Action onClose)
    {
        return GUI.Window(id, clientRect, i => OnWindow(i, clientRect, func, onClose), text);
    }

    private static void OnWindow(int id, Rect clientRect, GUI.WindowFunction onWindow, Action onClose)
    {
        WindowCloseButton(clientRect, onClose);

        const int headerVerticalOffset = 32; // The size of the title bar of the window

        const int contentHorizontalOffset = 1 + 4; // 4 for padding
        const int contentVerticalOffset = headerVerticalOffset + 8; // 8 for padding
        const int contentWidthOffset = contentHorizontalOffset * 2;
        const int contentHeightOffset = contentVerticalOffset + 8;

        GUILayout.BeginArea(
            new Rect(
                contentHorizontalOffset,
                contentVerticalOffset,
                clientRect.width - contentWidthOffset,
                clientRect.height - contentHeightOffset));
        onWindow(id);
        GUILayout.EndArea();

        GUI.DragWindow(new Rect(0, 0, clientRect.width, 32));
    }

    public static Rect MenubarWindow(int id, Rect clientRect, string text,
        Action<int> onMenu,
        GUI.WindowFunction onWindow,
        Action onClose)
    {
        return GUI.Window(id, clientRect, i => OnMenubarWindow(i, clientRect, onMenu, onWindow, onClose), text,
            "toolbar window");
    }

    private static void OnMenubarWindow(int id, Rect clientRect, Action<int> onMenu, GUI.WindowFunction onWindow,
        Action onClose)
    {
        WindowCloseButton(clientRect, onClose);

        const int menuHorizontalOffset = 1;
        const int menuVerticalOffset = 28;
        const int menuWidthOffset = menuHorizontalOffset * 2;
        const int menuHeight = 32;

        GUILayout.BeginArea(
            new Rect(menuHorizontalOffset,
                menuVerticalOffset,
                clientRect.width - menuWidthOffset,
                menuHeight));
        BeginWindowMenuBar();
        onMenu(id);
        EndWindowMenuBar();
        GUILayout.EndArea();

        const int contentHorizontalOffset = 1 + 4; // 4 for padding
        const int contentVerticalOffset = menuVerticalOffset + 20 + 8; // 20 for menu height(how?) 8 for padding
        const int contentWidthOffset = contentHorizontalOffset * 2;
        const int contentHeightOffset = contentVerticalOffset + 8;

        GUILayout.BeginArea(
            new Rect(
                contentHorizontalOffset,
                contentVerticalOffset,
                clientRect.width - contentWidthOffset,
                clientRect.height - contentHeightOffset));
        onWindow(id);
        GUILayout.EndArea();

        GUI.DragWindow(new Rect(0, 0, clientRect.width, 32));
    }

    private static void WindowCloseButton(Rect clientRect, Action onClose)
    {
        const int width = 20;
        const int offset = 4;
        if (GUI.Button(new Rect(clientRect.width - width - offset, offset, width, width), "X"))
        {
            onClose();
            GUIUtility.ExitGUI();
        }
    }

    public static void AddLastingWindow(IZeepWindow zeepWindow)
    {
        if (_windows.Any(x => x.Id == zeepWindow.Id))
            return;
        _windows.Add(zeepWindow);
    }

    public static void RemoveLastingWindow(IZeepWindow zeepWindow)
    {
        _windows.Remove(zeepWindow);
    }

    internal static void OnGUI()
    {
        if (!_hasSetBaseValue)
        {
            Skin.UpdateBaseValue(GUI.skin);
            _hasSetBaseValue = true;
        }

        MousePosition = Event.current.mousePosition;

        using (Skin.Override(CurrentSkin))
        {
            foreach (IZeepGUI receiver in _receivers)
            {
                receiver.OnZeepGUI();
            }

            foreach (IZeepWindow window in _windows)
            {
                window.Rect = window.Modal
                    ? GUI.ModalWindow(window.Id, window.Rect, window.OnZeepWindowGUI, window.Content)
                    : GUI.Window(window.Id, window.Rect, window.OnZeepWindowGUI, window.Content);
            }

            _toolbar.OnGUI();
            ZeepDropdownMenu.OnGUI();
            _toolbar.PostOnGUI();

            if (Event.current.IsKeyUp(Plugin.Instance.ToggleMenuBarKey.Value))
            {
                _toolbar.ToggleVisibleByKey();
            }

            if (Event.current.IsKeyUp(KeyCode.Escape))
            {
                GUIUtility.hotControl = -1;
                GUIUtility.keyboardControl = -1;
            }
        }
    }
}