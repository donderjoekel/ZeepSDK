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

    private static readonly ZeepToolbar _toolbar = new();
    private static readonly List<IZeepGUI> _receivers = [];
    private static readonly List<IZeepWindow> _windows = [];
    private static bool _hasSetBaseValue;

    internal static Vector2 MousePosition { get; private set; }
    internal static GUISkin ZeepSkin;

    public static OverrideStack<GUISkin> Skin = new(() => GUI.skin, value => GUI.skin = value, null);

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

        using (Skin.Override(ZeepSkin))
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

            if (Event.current.IsKeyUp(Plugin.Instance.ToggleToolbarKey.Value))
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