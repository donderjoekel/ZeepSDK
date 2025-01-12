using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace ZeepSDK.UI;

/// <summary>
/// A custom dropdown menu that can be used for ZeepGUI
/// </summary>
[PublicAPI]
public class ZeepDropdownMenu
{
    private static ZeepDropdownMenu _currentDropdownMenu;
    private static int _windowId = nameof(ZeepDropdownMenu).GetHashCode();

    internal static bool ClickedInDropdown => _currentDropdownMenu != null &&
                                              _currentDropdownMenu._rect.Contains(ZeepGUI.MousePosition);

    internal static bool HasDropdown => _currentDropdownMenu != null;

    private class ZeepDropdownMenuItem
    {
        public GUIContent Content;
        public Action Action;
    }

    private readonly int _x;
    private readonly int _y;
    private readonly List<ZeepDropdownMenuItem> _items = new();

    private bool _initialized;
    private Rect _rect;

    public ZeepDropdownMenu()
        : this(ZeepGUI.MousePosition.x, ZeepGUI.MousePosition.y)
    {
    }

    public ZeepDropdownMenu(float x, float y)
        : this(Mathf.RoundToInt(x), Mathf.RoundToInt(y))
    {
    }

    public ZeepDropdownMenu(int x, int y)
    {
        _x = x;
        _y = y;
    }

    public void AddItem(GUIContent content, Action action)
    {
        _items.Add(new ZeepDropdownMenuItem
        {
            Content = content,
            Action = action
        });
    }

    public void Show()
    {
        _currentDropdownMenu = this;
    }

    internal static void Hide()
    {
        _currentDropdownMenu = null;
    }

    private void CreateRect()
    {
        float maxWidth = 0;
        float height = 0;

        foreach (ZeepDropdownMenuItem item in _items)
        {
            Vector2 contentSize = GUI.skin.button.CalcSize(item.Content);
            Vector2 screenSize = GUI.skin.button.CalcScreenSize(contentSize);
            maxWidth = Mathf.Max(maxWidth, screenSize.x);
            height += screenSize.y;
        }

        height += 4;

        _rect = new Rect(_x, _y, maxWidth, height);
    }

    internal static void OnGUI()
    {
        if (_currentDropdownMenu == null) return;

        if (!_currentDropdownMenu._initialized)
        {
            _currentDropdownMenu.CreateRect();
            _currentDropdownMenu._initialized = true;
        }

        GUI.ModalWindow(_windowId,
            new Rect(0, 0, Screen.width, Screen.height),
            DrawWindow,
            string.Empty,
            GUIStyle.none);
        GUI.BringWindowToFront(_windowId);
    }

    private static void DrawWindow(int id)
    {
        using (new GUILayout.AreaScope(_currentDropdownMenu._rect, GUIContent.none, GUI.skin.box))
        {
            foreach (ZeepDropdownMenuItem imGuiDropdownMenuItem in _currentDropdownMenu._items)
            {
                if (GUILayout.Button(imGuiDropdownMenuItem.Content, "toolbar button"))
                {
                    _currentDropdownMenu = null;
                    imGuiDropdownMenuItem.Action?.Invoke();
                }
            }

            if (Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.Escape)
            {
                _currentDropdownMenu = null;
                Event.current.Use();
            }
        }

        if (Event.current.type == EventType.MouseUp && !ClickedInDropdown)
        {
            _currentDropdownMenu = null;
            Event.current.Use();
        }
    }
}