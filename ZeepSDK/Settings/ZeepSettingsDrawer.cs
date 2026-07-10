using System.Collections.Generic;
using System.Linq;
using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using Imui.Controls;
using Imui.Core;
using Imui.IO.Events;
using Imui.Rendering;
using UnityEngine;
using ZeepSDK.Controls;
using ZeepSDK.LevelEditor;
using ZeepSDK.Settings.Drawers;
using ZeepSDK.UI;

namespace ZeepSDK.Settings;

internal class ZeepSettingsDrawer : IZeepGUIDrawer
{
    private class SelectedPluginInfo
    {
        public PluginInfo Plugin { get; }
        public IReadOnlyList<IZeepSettingsDrawer> Drawers { get; }
        public bool UsesTabbedLayout { get; }

        public SelectedPluginInfo(PluginInfo plugin)
        {
            Plugin = plugin;

            if (plugin == null)
            {
                Drawers = [];
                UsesTabbedLayout = false;
                return;
            }

            var entriesBySection = ModSettingsDrawerBuildContext.BuildEntriesBySection(plugin);
            var buildContext = new ModSettingsDrawerBuildContext(plugin, entriesBySection);

            Drawers = ZeepSettingsDrawerRegistry.TryGetProvider(plugin.Metadata.GUID, out var provider)
                ? provider(buildContext).ToList()
                : buildContext.CreateDefaultDrawers().ToList();

            UsesTabbedLayout = Drawers.Any(d => d is ZeepSettingsTabbedSectionsDrawer);
        }
    }

    private readonly List<PluginInfo> _plugins = [];
    private readonly ZeepSettingsDrawContext _drawContext;
    private bool _open;
    private bool _openKeyPopup;
    private bool _mouse;

    private SelectedPluginInfo _selectedPluginInfo;
    private ConfigEntry<KeyCode> _currentKeyCodeEntry;
    private KeyCode _currentKeyCode;
    private float _keyCodeCountdown = 10f;

    private const float TabbedFlatTailFraction = 0.3f;
    private const float MinTabContentHeight = 300f;

    public ZeepSettingsDrawer()
    {
        _drawContext = new ZeepSettingsDrawContext
        {
            OpenKeyCodePopupInternal = OpenKeyPopup
        };

        ControlsApi.DisableAllInputExceptEventSystem(() => (_open && _mouse) || _openKeyPopup);
    }

    public void Open()
    {
        _selectedPluginInfo = new SelectedPluginInfo(null);

        _plugins.Clear();
        _plugins.AddRange(
            Chainloader
                .PluginInfos.Values.Where(x => x.Instance.Config.Any(y => !ZeepSettingsEntryMetadata.IsHidden(y.Value)))
                .OrderBy(x => x.Metadata.Name)
        );

        _open = true;
    }

    public void Close()
    {
        _open = false;
        _openKeyPopup = false;
        UpdateMouseInputBlock();
    }

    public void OnZeepGUI(ImGui gui)
    {
        const int windowHeight = 720;
        if (gui.BeginWindow("Zeep Settings", ref _open, ref _mouse,
                windowOpened: null,
                windowClosed: () => UpdateMouseInputBlock(),
                mouseEntered: () => UpdateMouseInputBlock(mouseOver: true),
                mouseExited: () => UpdateMouseInputBlock(mouseOver: false),
                size: (1280, windowHeight)))
        {
            var maxHeight = gui.GetLayoutHeight();
            using (gui.Horizontal())
            {
                using (gui.Vertical(400, maxHeight))
                {
                    using (gui.List((400, maxHeight)))
                    {
                        DrawPlugins(gui);
                    }
                }

                using (gui.Vertical(gui.GetLayoutWidth(), maxHeight))
                {
                    if (_selectedPluginInfo?.UsesTabbedLayout == true)
                        DrawSelectedPluginWithPadding(gui, maxHeight);
                    else
                    {
                        using (gui.Scrollable())
                            DrawSelectedPluginWithPadding(gui);
                    }
                }
            }

            gui.EndWindow();
        }

        DrawKeyPopup(gui);
    }

    private void DrawPlugins(ImGui gui)
    {
        foreach (var plugin in _plugins)
        {
            if (gui.ListItem(plugin == _selectedPluginInfo.Plugin,
                    plugin.Metadata.Name + $" ({plugin.Metadata.Version})"))
            {
                _selectedPluginInfo = new SelectedPluginInfo(plugin);
            }
        }
    }

    private void DrawSelectedPluginWithPadding(ImGui gui, float maxContentHeight = 0f)
    {
        var padding = gui.Style.Window.ContentPadding;
        var contentWidth = Mathf.Max(0, gui.GetLayoutWidth() - padding.Left - padding.Right);

        using (gui.Horizontal(gui.GetLayoutWidth()))
        {
            gui.AddSpacing(padding.Left);

            using (gui.Vertical(contentWidth))
            {
                if (padding.Top > 0)
                    gui.AddSpacing(padding.Top);

                DrawSelectedPlugin(gui, maxContentHeight);

                if (padding.Bottom > 0)
                    gui.AddSpacing(padding.Bottom);
            }

            gui.AddSpacing(padding.Right);
        }
    }

    private void DrawSelectedPlugin(ImGui gui, float maxContentHeight = 0f)
    {
        if (_selectedPluginInfo?.Plugin == null)
            return;

        gui.Text(_selectedPluginInfo.Plugin.Metadata.Name, new ImTextSettings(48, 0.5f));

        if (!_selectedPluginInfo.UsesTabbedLayout)
        {
            foreach (var drawer in _selectedPluginInfo.Drawers)
                drawer.Draw(gui, _drawContext);

            return;
        }

        var drawers = _selectedPluginInfo.Drawers;
        var tabDrawerIndex = -1;
        for (var i = 0; i < drawers.Count; i++)
        {
            if (drawers[i] is ZeepSettingsTabbedSectionsDrawer)
            {
                tabDrawerIndex = i;
                break;
            }
        }

        if (tabDrawerIndex < 0)
        {
            foreach (var drawer in drawers)
                drawer.Draw(gui, _drawContext);

            return;
        }

        var remainingHeight = maxContentHeight > 0f
            ? Mathf.Max(0f, maxContentHeight - gui.GetRowsHeightWithSpacing(1))
            : 0f;

        var flatDrawerCount = drawers.Count - tabDrawerIndex - 1;
        if (remainingHeight > 0f)
        {
            _drawContext.AvailableContentHeight = flatDrawerCount > 0
                ? Mathf.Max(MinTabContentHeight, remainingHeight * (1f - TabbedFlatTailFraction))
                : remainingHeight;
        }
        else
        {
            _drawContext.AvailableContentHeight = 0f;
        }

        drawers[tabDrawerIndex].Draw(gui, _drawContext);
        _drawContext.AvailableContentHeight = 0f;

        if (flatDrawerCount <= 0)
            return;

        if (remainingHeight > 0f)
        {
            using (gui.Scrollable())
            {
                for (var i = tabDrawerIndex + 1; i < drawers.Count; i++)
                    drawers[i].Draw(gui, _drawContext);
            }
        }
        else
        {
            for (var i = tabDrawerIndex + 1; i < drawers.Count; i++)
                drawers[i].Draw(gui, _drawContext);
        }
    }

    private void DrawKeyPopup(ImGui gui)
    {
        if (!_openKeyPopup)
        {
            return;
        }

        var id = gui.PushId("ZeepSettingsKeyCodePopup");
        {
            gui.BeginPopup();
            var rect = ImWindow.GetInitialWindowRect(gui, new ImSize(600, 300));

            gui.Canvas.PushClipRect(rect);
            gui.Canvas.PushRectMask(rect, 0);
            gui.Box(rect, gui.Style.Window.Box);

            gui.RegisterControl(id, rect);
            gui.RegisterGroup(id, rect);

            var contentRect = rect.WithPadding(8);
            gui.Layout.Push(ImAxis.Vertical, contentRect);

            {
                gui.Text($"Closing in {_keyCodeCountdown:00.00} seconds...", new ImTextSettings(12, 0.5f));

                TryCaptureKeyCode(gui, ref _currentKeyCode);

                gui.Text("The current key is: " + _currentKeyCode, new ImTextSettings(26, 0.5f, 0.5f, true));

                gui.AddSpacing(gui.GetLayoutHeight() - gui.GetRowsHeightWithSpacing(2));

                if (gui.Button("Save"))
                {
                    CloseKeyPopup(true);
                }

                if (gui.Button("Cancel"))
                {
                    CloseKeyPopup(false);
                }
            }

            gui.Layout.Pop();
            gui.Canvas.PopClipRect();
            gui.Canvas.PopClipRect();
            gui.EndPopup();
        }

        gui.PopId();

        _keyCodeCountdown -= Time.deltaTime;
        if (_keyCodeCountdown <= 0)
        {
            CloseKeyPopup(false);
        }
    }

    private void BlockMouseInput() => LevelEditorApi.BlockMouseInput(this);

    private void UnblockMouseInput() => LevelEditorApi.UnblockMouseInput(this);

    private void UpdateMouseInputBlock(bool? mouseOver = null)
    {
        bool mouse = mouseOver ?? _mouse;
        if ((_open && mouse) || _openKeyPopup)
        {
            BlockMouseInput();
        }
        else
        {
            UnblockMouseInput();
        }
    }

    private void OpenKeyPopup(ConfigEntry<KeyCode> entry)
    {
        _openKeyPopup = true;
        UpdateMouseInputBlock();
        _keyCodeCountdown = 10;
        _currentKeyCodeEntry = entry;
        _currentKeyCode = entry.Value;
    }

    private void CloseKeyPopup(bool save)
    {
        _openKeyPopup = false;
        UpdateMouseInputBlock();
        if (!save)
        {
            return;
        }

        _currentKeyCodeEntry.Value = _currentKeyCode;
    }

    private const int PrimaryMouseButtonCount = 3;

    private static void TryCaptureKeyCode(ImGui gui, ref KeyCode keyCode)
    {
        if (gui.Input.KeyboardEventsCount > 0)
        {
            var keyId = gui.Input.KeyboardEventsCount - 1;
            var evt = gui.Input.GetKeyboardEvent(keyId);
            if (evt.Key != KeyCode.None && !IsPrimaryMouseButton(evt.Key))
            {
                keyCode = evt.Key;
            }

            gui.Input.UseKeyboardEvent(keyId);
            return;
        }

        ref readonly var mouseEvt = ref gui.Input.MouseEvent;
        if (mouseEvt.Type != ImMouseEventType.Down ||
            mouseEvt.Device != ImMouseDevice.Mouse ||
            mouseEvt.Button < PrimaryMouseButtonCount)
        {
            return;
        }

        var mouseKeyCode = MouseButtonToKeyCode(mouseEvt.Button);
        if (mouseKeyCode == KeyCode.None)
        {
            return;
        }

        keyCode = mouseKeyCode;
        gui.Input.UseMouseEvent();
    }

    private static bool IsPrimaryMouseButton(KeyCode keyCode)
    {
        return keyCode is KeyCode.Mouse0 or KeyCode.Mouse1 or KeyCode.Mouse2;
    }

    private static KeyCode MouseButtonToKeyCode(int button)
    {
        if (button is < 0 or > 6)
        {
            return KeyCode.None;
        }

        return (KeyCode)((int)KeyCode.Mouse0 + button);
    }
}
