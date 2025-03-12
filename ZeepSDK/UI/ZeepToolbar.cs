using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using ZeepSDK.Controls;
using ZeepSDK.UI.Patches;
using ZeepSDK.Utilities;
using ZeepSDK.Utilities.Override;
using Cursor = UnityEngine.Cursor;
using Screen = UnityEngine.Device.Screen;

namespace ZeepSDK.UI;

internal class ZeepToolbar : ZeepGUIBehaviour
{
    private class ToolbarButtonRoot
    {
        private readonly Dictionary<int, List<ToolbarButtonChild>> _children;

        public string Title { get; private set; }
        public Action OnClick { get; private set; }

        public IEnumerable<ToolbarButtonChild> Children
        {
            get
            {
                foreach ((int _, List<ToolbarButtonChild> children) in _children.OrderBy(x=>x.Key))
                {
                    foreach (ToolbarButtonChild child in children)
                    {
                        yield return child;
                    }
                }
            }
        }

        public ToolbarButtonRoot(string title, Action onClick)
        {
            Title = title;
            OnClick = onClick;
            _children = new Dictionary<int, List<ToolbarButtonChild>>();
        }

        public void AddChild(string title, Action onClick, int priority)
        {
            if (OnClick != null)
                return;

            if (!_children.TryGetValue(priority, out List<ToolbarButtonChild> children))
                _children.Add(priority, children = new List<ToolbarButtonChild>());

            children.Add(new ToolbarButtonChild(title, onClick));
            children.Sort((x, y) => string.Compare(x.Title, y.Title, StringComparison.Ordinal));
        }
    }

    private class ToolbarButtonChild
    {
        public string Title { get; private set; }
        public Action OnClick { get; private set; }

        public ToolbarButtonChild(string title, Action onClick)
        {
            Title = title;
            OnClick = onClick;
        }
    }

    private readonly Dictionary<string, ToolbarButtonRoot> _toolbarRoots = new();
    private readonly DisposableBag _disposableBag;
    private readonly OverrideStack<bool> _cursorVisible = new(() => Cursor.visible, x =>
    {
        Cursor.visible = x;
        Cursor.lockState = x ? CursorLockMode.None : CursorLockMode.Locked;
    }, true);

    private VisualElement _toolbar;
    private bool _mouseOver;
    private bool _inMenu;
    private bool _visibleByKey;

    private bool MouseOver
    {
        set
        {
            if (value == _mouseOver)
                return;

            _mouseOver = value;
            UpdateStyle();
        }
    }

    private bool InMenu
    {
        set
        {
            if (value == _inMenu)
                return;

            _inMenu = value;
            UpdateStyle();
        }
    }

    public ZeepToolbar()
    {
        _disposableBag = ControlsApi.DisableAllInput(InputLockCondition);
        _disposableBag.Add(_cursorVisible.Override(true, () => _visibleByKey));
        CursorManager_SetCursorEnabled.Invoked += SetCursorEnabledInvoked;
    }

    protected override bool BlocksInput()
    {
        return false;
    }

    private void SetCursorEnabledInvoked()
    {
        _cursorVisible.UpdateBaseValue(Cursor.visible);
    }

    private bool InputLockCondition()
    {
        return ((_mouseOver || _inMenu) && Cursor.visible) || _visibleByKey;
    }
    
    public void AddToolbarButtonRoot(string title, Action onClick = null)
    {
        if (_toolbarRoots.ContainsKey(title))
            return; // Handle

        ToolbarButtonRoot root = new(title, onClick);
        _toolbarRoots.Add(title, root);
    }

    public void AddToolbarButtonChild(string rootTitle, string title, Action onClick, int priority = 0)
    {
        if (!_toolbarRoots.TryGetValue(rootTitle, out ToolbarButtonRoot root))
            return; // Handle

        root.AddChild(title, onClick, priority);
    }

    protected override void BuildUi()
    {
        using (new ZeepGUI.ToolbarScope(ZeepGUI))
        {
            _toolbar = ZeepGUI.GetLastElement();

            foreach ((string _, ToolbarButtonRoot root) in _toolbarRoots)
            {
                if (root.OnClick != null)
                {
                    ZeepGUI.Button(root.Title, () => root.OnClick());
                }
                else
                {
                    ZeepGUI.DropdownButton(root.Title, menu =>
                    {
                        InMenu = true;

                        foreach (ToolbarButtonChild child in root.Children)
                        {
                            menu.AddItem(child.Title, false, () =>
                            {
                                InMenu = false;
                                child.OnClick();
                            });
                        }
                    });

                    VisualElement dropdown = ZeepGUI.GetLastElement();
                    dropdown.elementPanel.hierarchyChanged += OnHierarchyChanged;

                    void OnHierarchyChanged(VisualElement element, HierarchyChangeType type)
                    {
                        if (type != HierarchyChangeType.Add)
                            return;

                        if (!InputLockCondition())
                            return;

                        VisualElement dropdownElement =
                            element?.panel?.visualTree?.Q(className: GenericDropdownMenu.ussClassName);

                        if (dropdownElement == null) return;

                        dropdownElement.RegisterCallback<DetachFromPanelEvent, ZeepToolbar>((_, args) =>
                        {
                            args.InMenu = false;
                            
                        }, this);
                    }
                }
            }
        }
    }

    private void UpdateStyle()
    {
        if (_mouseOver || _inMenu || _visibleByKey)
        {
            _toolbar.style.top = 0;
        }
        else
        {
            _toolbar.style.top = -_toolbar.resolvedStyle.height;
        }
    }

    private void Update()
    {
        if (_toolbar == null) return;

        if (Input.GetKeyUp(Plugin.Instance.ToggleMenuBarKey.Value))
        {
            _visibleByKey = !_visibleByKey;
            UpdateStyle();
        }

        float mousePositionY = Screen.height - Input.mousePosition.y;
        MouseOver = mousePositionY <= _toolbar.resolvedStyle.height;
    }
}
