using System;
using BepInEx.Configuration;
using Imui.Controls;
using Imui.Core;
using Imui.Rendering;
using UnityEngine;
using ZeepSDK.UI;

namespace ZeepSDK.Settings.Drawers;

/// <summary>
/// Renders BepInEx config entries in the mod settings panel.
/// </summary>
internal static class ZeepSettingsEntryRenderer
{
    /// <summary>
    /// Draws the given config entry row.
    /// </summary>
    /// <param name="gui">The ImGui instance to draw with.</param>
    /// <param name="entry">The config entry to draw.</param>
    /// <param name="context">Shared services available while drawing settings.</param>
    /// <param name="label">Optional display label. Uses the config key when omitted.</param>
    public static void Draw(ImGui gui, ConfigEntryBase entry, ZeepSettingsDrawContext context, string label = null)
    {
        DrawEntryValue(gui, entry, label ?? entry.Definition.Key, context);
    }

    private static void DrawEntryValue(ImGui gui, ConfigEntryBase entry, string key, ZeepSettingsDrawContext context)
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
            DrawEnumEntry(gui, entry, controlSize);
        }
        else
        {
            switch (entry)
            {
                case ConfigEntry<KeyCode> keyCodeEntry:
                    DrawKeyCodeEntry(gui, keyCodeEntry, context, controlSize);
                    break;
                case ConfigEntry<string> stringEntry:
                    DrawStringEntry(gui, stringEntry, controlSize);
                    break;
                case ConfigEntry<bool> boolEntry:
                    if (ZeepSettingsEntryMetadata.IsButton(boolEntry))
                        DrawButtonEntry(gui, boolEntry, controlSize);
                    else
                        DrawBoolEntry(gui, boolEntry, controlSize);
                    break;
                case ConfigEntry<int> intEntry:
                    DrawIntEntry(gui, intEntry, controlSize);
                    break;
                case ConfigEntry<float> floatEntry:
                    DrawFloatEntry(gui, floatEntry, controlSize);
                    break;
                case ConfigEntry<double> doubleEntry:
                    DrawDoubleEntry(gui, doubleEntry, controlSize);
                    break;
                case ConfigEntry<Vector2> vector2Entry:
                    DrawVector2Entry(gui, vector2Entry, controlSize);
                    break;
                case ConfigEntry<Vector3> vector3Entry:
                    DrawVector3Entry(gui, vector3Entry, controlSize);
                    break;
                case ConfigEntry<Vector4> vector4Entry:
                    DrawVector4Entry(gui, vector4Entry, controlSize);
                    break;
                case ConfigEntry<Color> colorEntry:
                    DrawColorEntry(gui, colorEntry, controlSize);
                    break;
                default:
                    DrawUnsupportedType(gui, entry);
                    break;
            }
        }

        DrawTooltip(gui, entry);

        if (!ZeepSettingsEntryMetadata.IsButton(entry))
        {
            if (gui.Button("Reset", (buttonWidth, height)))
                entry.BoxedValue = entry.DefaultValue;
        }

        gui.EndHorizontal();
    }

    private static void DrawButtonEntry(ImGui gui, ConfigEntry<bool> entry, ImSize size)
    {
        if (gui.Button("Execute", size))
            entry.BoxedValue = !entry.Value;
    }

    private static void DrawKeyCodeEntry(
        ImGui gui,
        ConfigEntry<KeyCode> entry,
        ZeepSettingsDrawContext context,
        ImSize size)
    {
        var popupButtonSize = new ImSize(size.Width - 100 - gui.Style.Layout.Spacing, size.Height);
        var clearButtonSize = new ImSize(100, size.Height);

        if (gui.Button(entry.Value.ToString(), popupButtonSize))
            context.OpenKeyCodePopup(entry);

        if (gui.Button("Clear", clearButtonSize))
            entry.BoxedValue = KeyCode.None;
    }

    private static void DrawEnumEntry(ImGui gui, ConfigEntryBase entry, ImSize size)
    {
        var value = (int)entry.BoxedValue;
        if (gui.Dropdown(ref value, Enum.GetNames(entry.SettingType), size))
            entry.BoxedValue = value;
    }

    private static void DrawStringEntry(ImGui gui, ConfigEntry<string> entry, ImSize size)
    {
        if (!TryDrawAcceptableValueList(gui, entry, size))
        {
            var current = entry.Value;
            var value = gui.TextEdit(current, size);
            if (value != current)
                entry.Value = value;
        }
    }

    private static void DrawBoolEntry(ImGui gui, ConfigEntry<bool> entry, ImSize size)
    {
        if (!TryDrawAcceptableValueList(gui, entry, size))
        {
            var current = entry.Value;
            if (gui.Checkbox(ref current, string.Empty, size))
                entry.Value = current;
        }
    }

    private static void DrawIntEntry(ImGui gui, ConfigEntry<int> entry, ImSize size)
    {
        if (TryDrawAcceptableValueRange(gui, entry, size))
            return;

        if (TryDrawAcceptableValueList(gui, entry, size))
            return;

        var current = entry.Value;
        if (gui.NumericEdit(ref current, size))
            entry.Value = current;
    }

    private static void DrawFloatEntry(ImGui gui, ConfigEntry<float> entry, ImSize size)
    {
        if (TryDrawAcceptableValueRange(gui, entry, size))
            return;

        if (TryDrawAcceptableValueList(gui, entry, size))
            return;

        var current = entry.Value;
        if (gui.NumericEdit(ref current, size))
            entry.Value = current;
    }

    private static void DrawDoubleEntry(ImGui gui, ConfigEntry<double> entry, ImSize size)
    {
        if (TryDrawAcceptableValueRange(gui, entry, size))
            return;

        if (TryDrawAcceptableValueList(gui, entry, size))
            return;

        var current = entry.Value;
        if (gui.NumericEdit(ref current, size))
            entry.Value = current;
    }

    private static void DrawVector2Entry(ImGui gui, ConfigEntry<Vector2> entry, ImSize size)
    {
        if (!TryDrawAcceptableValueList(gui, entry, size))
        {
            var current = entry.Value;
            if (gui.Vector(ref current, size))
                entry.Value = current;
        }
    }

    private static void DrawVector3Entry(ImGui gui, ConfigEntry<Vector3> entry, ImSize size)
    {
        if (!TryDrawAcceptableValueList(gui, entry, size))
        {
            var current = entry.Value;
            if (gui.Vector(ref current, size))
                entry.Value = current;
        }
    }

    private static void DrawVector4Entry(ImGui gui, ConfigEntry<Vector4> entry, ImSize size)
    {
        if (!TryDrawAcceptableValueList(gui, entry, size))
        {
            var current = entry.Value;
            if (gui.Vector(ref current, size))
                entry.Value = current;
        }
    }

    private static void DrawColorEntry(ImGui gui, ConfigEntry<Color> entry, ImSize size)
    {
        if (!TryDrawAcceptableValueList(gui, entry, size))
        {
            var current = entry.Value;
            if (gui.ColorEdit(ref current, size))
                entry.Value = current;
        }
    }

    private static bool TryDrawAcceptableValueRange(ImGui gui, ConfigEntry<int> entry, ImSize size)
    {
        if (entry.Description.AcceptableValues is not AcceptableValueRange<int> range)
            return false;

        var current = entry.Value;
        if (gui.NumericEdit(ref current, size, min: range.MinValue, max: range.MaxValue, flags: ImNumericEditFlag.Slider))
            entry.Value = current;

        return true;
    }

    private static bool TryDrawAcceptableValueRange(ImGui gui, ConfigEntry<float> entry, ImSize size)
    {
        if (entry.Description.AcceptableValues is not AcceptableValueRange<float> range)
            return false;

        var current = entry.Value;
        if (gui.NumericEdit(ref current, size, min: range.MinValue, max: range.MaxValue, flags: ImNumericEditFlag.Slider))
            entry.Value = current;

        return true;
    }

    private static bool TryDrawAcceptableValueRange(ImGui gui, ConfigEntry<double> entry, ImSize size)
    {
        if (entry.Description.AcceptableValues is not AcceptableValueRange<double> range)
            return false;

        var current = entry.Value;
        if (gui.NumericEdit(ref current, size, min: range.MinValue, max: range.MaxValue, flags: ImNumericEditFlag.Slider))
            entry.Value = current;

        return true;
    }

    private static bool TryDrawAcceptableValueList<T>(ImGui gui, ConfigEntry<T> entry, ImSize size)
        where T : IEquatable<T>
    {
        if (entry.Description.AcceptableValues is not AcceptableValueList<T> acceptableValues)
            return false;

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

    private static void DrawUnsupportedType(ImGui gui, ConfigEntryBase entry)
    {
        gui.Text($"No drawer available for type '{entry.SettingType.Name}'", Color.red);
    }

    private static void DrawTooltip(ImGui gui, ConfigEntryBase entry)
    {
        if (!string.IsNullOrEmpty(entry.Description.Description))
            gui.TooltipAtLastControl(entry.Description.Description);
    }
}
