using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using ZeepSDK.Controls;
using ZeepSDK.UI.Patches;
using ZeepSDK.Utilities;
using ZeepSDK.Utilities.Override;

namespace ZeepSDK.UI;

internal class ZeepToolbar : IDisposable
{
    private class ZeepToolbarItem
    {
        public GUIContent Content;
        public Action Action;
        public readonly List<ZeepToolbarItem> Children = new();
        public int Priority;
    }

    private readonly Dictionary<string, ZeepToolbarItem> _toolbarItems = new();
    private readonly DisposableBag _disposableBag;
    private Rect _toolbarRect = new Rect(0, -32, 0, 0);
    private float _toolbarVelocity;
    private bool _hasMenu;
    private float _hideDelay;
    private bool _visibleByKey;

    private readonly OverrideStack<bool> _cursorVisible = new(() => Cursor.visible, x =>
    {
        Cursor.visible = x;
        Cursor.lockState = x ? CursorLockMode.None : CursorLockMode.Locked;
    }, true);

    public ZeepToolbar()
    {
        _disposableBag = ControlsApi.DisableAllInput(InputLockCondition);
        _disposableBag.Add(_cursorVisible.Override(true, () => _visibleByKey));
        CursorManager_SetCursorEnabled.Invoked += SetCursorEnabledInvoked;
    }

    public void Dispose()
    {
        _disposableBag.Dispose();
        CursorManager_SetCursorEnabled.Invoked -= SetCursorEnabledInvoked;
    }

    private void SetCursorEnabledInvoked()
    {
        _cursorVisible.UpdateBaseValue(Cursor.visible);
    }

    private bool InputLockCondition()
    {
        return (ZeepGUI.MousePosition.y < 32 && Cursor.visible) || _visibleByKey;
    }

    public void AddItem(GUIContent content, Action action, int priority)
    {
        if (string.IsNullOrWhiteSpace(content.text)) throw new Exception("Content cannot be empty");
        AddItemToDictionary(content.text, content, action, priority);
    }

    public void AddChild(string parent, GUIContent content, Action action, int priority)
    {
        if (string.IsNullOrWhiteSpace(parent)) throw new Exception("Parent cannot be empty");
        if (string.IsNullOrWhiteSpace(content.text)) throw new Exception("Content cannot be empty");
        if (!_toolbarItems.TryGetValue(parent, out ZeepToolbarItem parentItem))
            throw new Exception("Parent does not exist");
        parentItem.Children.Add(new ZeepToolbarItem
        {
            Content = content,
            Action = action,
            Priority = priority
        });
        parentItem.Children.Sort((lhs, rhs) => lhs.Priority.CompareTo(rhs.Priority));
    }

    private ZeepToolbarItem AddItemToDictionary(string key, GUIContent content, Action action, int priority)
    {
        if (_toolbarItems.ContainsKey(key))
            throw new Exception("Key already exists");
        ZeepToolbarItem item = new()
        {
            Content = content,
            Action = action,
            Priority = priority
        };
        _toolbarItems.Add(key, item);
        return item;
    }

    public void OnGUI()
    {
        _toolbarRect.width = Screen.width;
        _toolbarRect.height = Screen.height;

        Rect mouseRect = new(_toolbarRect)
        {
            y = 0,
            height = 32
        };
        Vector2 mousePosition = Event.current.mousePosition;
        bool hasMouse = mouseRect.Contains(mousePosition);
        _toolbarRect.y = Mathf.SmoothDamp(
            _toolbarRect.y,
            hasMouse || _hasMenu || _hideDelay > 0 ? 0 : -32,
            ref _toolbarVelocity,
            0.25f, 
            Mathf.Infinity,
            Time.unscaledDeltaTime);

        if (!hasMouse && !_hasMenu)
            _hideDelay -= Time.unscaledDeltaTime;
        else
            _hideDelay = 1f;

        if (_visibleByKey)
        {
            _toolbarRect.y = 0;
        }

        GUILayout.BeginArea(_toolbarRect);
        GUILayout.BeginHorizontal("toolbar", GUILayout.ExpandWidth(true));

        IOrderedEnumerable<ZeepToolbarItem> items = _toolbarItems
            .Select(x => x.Value)
            .OrderBy(x => x.Priority);

        foreach (ZeepToolbarItem item in items)
        {
            DrawItem(item);
        }

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }

    public void PostOnGUI()
    {
        if (_hasMenu && !ZeepDropdownMenu.HasDropdown)
        {
            _hasMenu = false;
        }
    }

    private void DrawItem(ZeepToolbarItem item)
    {
        if (!GUILayout.Button(item.Content)) 
            return;

        if (item.Children.Count > 0)
        {
            CreateAndShowDropdown(item);
        }
        else
        {
            item.Action.Invoke();
        }
    }

    private void CreateAndShowDropdown(ZeepToolbarItem item)
    {
        ZeepDropdownMenu menu = new();
        foreach (ZeepToolbarItem child in item.Children)
        {
            menu.AddItem(child.Content, () =>
            {
                _hasMenu = false;
                child.Action?.Invoke();
            });
        }

        _hasMenu = true;
        menu.Show();
    }

    public void ToggleVisibleByKey()
    {
        _visibleByKey = !_visibleByKey;
    }
}