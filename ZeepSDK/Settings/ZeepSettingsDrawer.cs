using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using Imui.Controls;
using Imui.Core;
using Imui.Rendering;
using UnityEngine;
using ZeepSDK.Controls;
using ZeepSDK.UI;

namespace ZeepSDK.Settings;

internal class ZeepSettingsDrawer : IZeepGUIDrawer
{
    private record PluginEntryInfo(ConfigDefinition Definition, ConfigEntryBase Entry);

    private class SelectedPluginInfo
    {
        public PluginInfo Plugin { get; }
        public Dictionary<string, SortedList<string, PluginEntryInfo>> Entries { get; }

        public SelectedPluginInfo(PluginInfo plugin)
        {
            Plugin = plugin;
            Entries = [];

            if (Plugin == null)
                return;

            foreach ((ConfigDefinition definition, ConfigEntryBase entry) in Plugin.Instance.Config)
            {
                if (!Entries.TryGetValue(definition.Section, out var entries))
                {
                    Entries.Add(definition.Section, entries = []);
                }

                entries.Add(definition.Key, new PluginEntryInfo(definition, entry));
            }
        }
    }

    private readonly List<PluginInfo> _plugins = [];
    private bool _open = false;
    private bool _openKeyPopup = false;
    private bool _mouse = false;

    private SelectedPluginInfo _selectedPluginInfo;
    private ConfigEntryBase _currentKeyCodeEntry;
    private KeyCode _currentKeyCode;
    private float _keyCodeCountdown = 10f;

    public ZeepSettingsDrawer()
    {
        ControlsApi.DisableAllInputExceptEventSystem(() => (_open && _mouse) || _openKeyPopup);
    }

    public void Open()
    {
        _selectedPluginInfo = new SelectedPluginInfo(null);

        _plugins.Clear();
        _plugins.AddRange(
            Chainloader
                .PluginInfos.Values.Where(x => x.Instance.Config.Any(y => !IsHidden(y.Value)))
                .OrderBy(x => x.Metadata.Name)
        );

        _open = true;
    }

    public void Close()
    {
        _open = false;
        _openKeyPopup = false;
    }

    public void OnZeepGUI(ImGui gui)
    {
        const int windowHeight = 720;
        if (gui.BeginWindow("Zeep Settings", ref _open, ref _mouse, (1280, windowHeight)))
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
                    using (gui.Scrollable())
                    {
                        DrawSelectedPlugin(gui);
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

    private void DrawKeyPopup(ImGui gui)
    {
        if (!_openKeyPopup)
            return;

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

                if (gui.Input.KeyboardEventsCount > 0)
                {
                    var keyId = gui.Input.KeyboardEventsCount - 1;
                    var evt = gui.Input.GetKeyboardEvent(keyId);
                    if (evt.Key != KeyCode.None)
                        _currentKeyCode = evt.Key;
                    gui.Input.UseKeyboardEvent(keyId);
                }

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
            gui.Canvas.PopRectMask();
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

    private void OpenKeyPopup(ConfigEntryBase entry)
    {
        _openKeyPopup = true;
        _keyCodeCountdown = 10;
        _currentKeyCodeEntry = entry;
        _currentKeyCode = (KeyCode)_currentKeyCodeEntry.BoxedValue;
    }

    private void CloseKeyPopup(bool save)
    {
        _openKeyPopup = false;
        if (!save)
            return;

        _currentKeyCodeEntry.BoxedValue = _currentKeyCode;
    }

    private void DrawSelectedPlugin(ImGui gui)
    {
        if (_selectedPluginInfo == null || _selectedPluginInfo.Plugin == null)
            return;

        gui.Text(_selectedPluginInfo.Plugin.Metadata.Name, new ImTextSettings(48, 0.5f));

        DrawEntries(gui);
    }

    private void DrawEntries(ImGui gui)
    {
        foreach ((string section, SortedList<string, PluginEntryInfo> entries) in _selectedPluginInfo.Entries)
        {
            using (gui.Indent())
            {
                gui.Text(section, new ImTextSettings(36));

                foreach ((string key, (ConfigDefinition definition, ConfigEntryBase entry)) in entries)
                {
                    DrawEntry(gui, entry, key);
                }

                gui.AddSpacing();
            }
        }
    }

    private void DrawEntry(ImGui gui, ConfigEntryBase entry, string key)
    {
        if (IsHidden(entry))
            return;

        using (gui.Indent())
        {
            DrawEntryValue(gui, entry, key);
            gui.AddSpacing(gui.Style.Layout.Spacing * 2);
            gui.Separator();
            gui.AddSpacing(gui.Style.Layout.Spacing * 2);
        }
    }

    private void DrawButtonEntry(ImGui gui, ConfigEntryBase entry, string key, ImSize size)
    {
        if (gui.Button("Execute", size))
        {
            entry.BoxedValue = !(bool)entry.BoxedValue;
        }
    }

    private void DrawEntryValue(ImGui gui, ConfigEntryBase entry, string key)
    {
        const int buttonWidth = 100;
        gui.BeginHorizontal(gui.GetLayoutWidth());

        var totalWidth = gui.GetLayoutWidth();
        var section = totalWidth / 3;
        var height = gui.GetRowHeight();
        var spacing = gui.Style.Layout.Spacing;

        var textRect = gui.Layout.GetRect(section - spacing, height);
        gui.Text(key, entry.Description.Description, textRect, false, ImTextOverflow.Ellipsis);
        gui.Layout.AddRect(textRect);
        gui.AddSpacing();

        var controlSize = new ImSize((section * 2) - spacing - buttonWidth, height);

        if (entry.SettingType.IsEnum && entry.SettingType != typeof(KeyCode))
        {
            DrawEnumEntry(gui, entry, key, controlSize);
        }
        else
        {
            switch (entry)
            {
                case ConfigEntry<KeyCode> keyCodeEntry:
                    DrawKeyCodeEntry(gui, keyCodeEntry, key, controlSize);
                    break;
                case ConfigEntry<string> stringEntry:
                    DrawStringEntry(gui, stringEntry, key, controlSize);
                    break;
                case ConfigEntry<bool> boolEntry:
                    if (IsButton(boolEntry))
                    {
                        DrawButtonEntry(gui,boolEntry, key, controlSize);
                    }
                    else
                    {
                        DrawBoolEntry(gui, boolEntry, key, controlSize);
                    }
                    break;
                case ConfigEntry<int> intEntry:
                    DrawIntEntry(gui, intEntry, key, controlSize);
                    break;
                case ConfigEntry<float> floatEntry:
                    DrawFloatEntry(gui, floatEntry, key, controlSize);
                    break;
                case ConfigEntry<double> doubleEntry:
                    DrawDoubleEntry(gui, doubleEntry, key, controlSize);
                    break;
                case ConfigEntry<Vector2> vector2Entry:
                    DrawVector2Entry(gui, vector2Entry, key, controlSize);
                    break;
                case ConfigEntry<Vector3> vector3Entry:
                    DrawVector3Entry(gui, vector3Entry, key, controlSize);
                    break;
                case ConfigEntry<Vector4> vector4Entry:
                    DrawVector4Entry(gui, vector4Entry, key, controlSize);
                    break;
                case ConfigEntry<Color> colorEntry:
                    DrawColorEntry(gui, colorEntry, key, controlSize);
                    break;
                default:
                    DrawUnsupportedType(gui, entry, key);
                    break;
            }
        }

        DrawTooltip(gui, entry);

        if (!IsButton(entry))
        {
            if (gui.Button("Reset", (buttonWidth, height)))
            {
                entry.BoxedValue = entry.DefaultValue;
            }
        }

        gui.EndHorizontal();
    }

    private void DrawKeyCodeEntry(ImGui gui, ConfigEntry<KeyCode> entry, string key, ImSize size)
    {
        var popupButtonSize = new ImSize(size.Width - 100 - gui.Style.Layout.Spacing, size.Height);
        var clearButtonSize = new ImSize(100, size.Height);
        
        if (gui.Button(((KeyCode)entry.BoxedValue).ToString(), popupButtonSize))
        {
            OpenKeyPopup(entry);
        }

        if (gui.Button("Clear", clearButtonSize))
        {
            entry.BoxedValue = KeyCode.None;
        }
    }

    private void DrawEnumEntry(ImGui gui, ConfigEntryBase entry, string key, ImSize size)
    {
        var value = (int)entry.BoxedValue;
        if (gui.Dropdown(ref value, Enum.GetNames(entry.SettingType), size))
        {
            entry.BoxedValue = value;
        }
    }

    private void DrawStringEntry(ImGui gui, ConfigEntry<string> entry, string key, ImSize size)
    {
        if (!DrawAcceptableValues(gui, entry, size))
        {
            var current = (string)entry.BoxedValue;
            var value = gui.TextEdit(current, size);
            if (value != current)
            {
                entry.BoxedValue = value;
            }
        }
    }

    private void DrawBoolEntry(ImGui gui, ConfigEntry<bool> entry, string key, ImSize size)
    {
        if (!DrawAcceptableValues(gui, entry, size))
        {
            var current = entry.Value;
            if (gui.Checkbox(ref current, string.Empty, size))
            {
                entry.Value = current;
            }
        }
    }

    private void DrawIntEntry(ImGui gui, ConfigEntry<int> entry, string key, ImSize size)
    {
        if (!DrawAcceptableValues(gui, entry, size))
        {
            var current = entry.Value;
            if (gui.NumericEdit(ref current, size))
            {
                entry.Value = current;
            }
        }
    }

    private void DrawFloatEntry(ImGui gui, ConfigEntry<float> entry, string key, ImSize size)
    {
        if (!DrawAcceptableValues(gui, entry, size))
        {
            var current = entry.Value;
            if (gui.NumericEdit(ref current, size))
            {
                entry.Value = current;
            }
        }
    }

    private void DrawDoubleEntry(ImGui gui, ConfigEntry<double> entry, string key, ImSize size)
    {
        if (!DrawAcceptableValues(gui, entry, size))
        {
            var current = entry.Value;
            if (gui.NumericEdit(ref current, size))
            {
                entry.Value = current;
            }
        }
    }

    private void DrawVector2Entry(ImGui gui, ConfigEntry<Vector2> entry, string key, ImSize size)
    {
        if (!DrawAcceptableValues(gui, entry, size))
        {
            var current = entry.Value;
            if (gui.Vector(ref current, size))
            {
                entry.Value = current;
            }
        }
    }

    private void DrawVector3Entry(ImGui gui, ConfigEntry<Vector3> entry, string key, ImSize size)
    {
        if (!DrawAcceptableValues(gui, entry, size))
        {
            var current = entry.Value;
            if (gui.Vector(ref current, size))
            {
                entry.Value = current;
            }
        }
    }

    private void DrawVector4Entry(ImGui gui, ConfigEntry<Vector4> entry, string key, ImSize size)
    {
        if (!DrawAcceptableValues(gui, entry, size))
        {
            var current = entry.Value;
            if (gui.Vector(ref current, size))
            {
                entry.Value = current;
            }
        }
    }

    private void DrawColorEntry(ImGui gui, ConfigEntry<Color> entry, string key, ImSize size)
    {
        if (!DrawAcceptableValues(gui, entry, size))
        {
            var current = entry.Value;
            if (gui.ColorEdit(ref current, size))
            {
                entry.Value = current;
            }
        }
    }

    private bool DrawAcceptableValues<T>(ImGui gui, ConfigEntry<T> entry, ImSize size)
        where T : IEquatable<T>
    {
        if (entry.Description.AcceptableValues is not AcceptableValueList<T> acceptableValues)
        {
            return false;
        }

        if (gui.BeginDropdown(entry.Value.ToString(), size))
        {
            foreach (var value in acceptableValues.AcceptableValues)
            {
                if (gui.Menu(value.ToString(), Equals(value, entry.Value)))
                {
                    entry.Value = value;
                    gui.CloseDropdown();
                }
            }

            gui.EndDropdown();
        }

        return true;
    }

    private void DrawUnsupportedType(ImGui gui, ConfigEntryBase entry, string key)
    {
        gui.Text($"No drawer available for type '{entry.SettingType.Name}'", Color.red);
    }

    private void DrawTooltip(ImGui gui, ConfigEntryBase entry)
    {
        if (!string.IsNullOrEmpty(entry.Description.Description))
        {
            gui.TooltipAtLastControl(entry.Description.Description);
        }
    }

    private void DrawResetButton(ImGui gui, ConfigEntryBase entry)
    {
        if (gui.Button("Reset"))
        {
            entry.BoxedValue = entry.DefaultValue;
        }

        gui.TooltipAtLastControl("Resets this setting to the default value");
    }

    private bool IsHidden(ConfigEntryBase entry)
    {
        return entry.Description.Description.StartsWith("[hide]", StringComparison.OrdinalIgnoreCase) ||
               entry.Description.Description.StartsWith("[hidden]", StringComparison.OrdinalIgnoreCase);
    }

    private bool IsButton(ConfigEntryBase entry)
    {
        return entry.Description.Description.StartsWith("[button]", StringComparison.OrdinalIgnoreCase) &&
               entry.SettingType == typeof(bool);
    }
}
