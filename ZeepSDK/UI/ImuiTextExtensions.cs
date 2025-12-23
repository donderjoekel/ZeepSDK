using System;
using Imui.Controls;
using Imui.Core;
using Imui.Rendering;
using Imui.Style;
using UnityEngine;

namespace ZeepSDK.UI;

/// <summary>
/// Extension methods for rendering text with tooltips in ImGui
/// </summary>
internal static class ImuiTextExtensions
{
    /// <summary>
    /// Minimum width for text layout calculations
    /// </summary>
    public const float MIN_WIDTH = 1;
    
    /// <summary>
    /// Minimum height for text layout calculations
    /// </summary>
    public const float MIN_HEIGHT = 1;

    /// <summary>
    /// Renders text with a tooltip using default text settings
    /// </summary>
    /// <param name="gui">The ImGui instance</param>
    /// <param name="text">The text to display</param>
    /// <param name="tooltip">The tooltip text to show when hovering over the text</param>
    /// <param name="wrap">Whether to wrap the text</param>
    /// <param name="overflow">How to handle text overflow</param>
    public static void Text(
        this ImGui gui,
        ReadOnlySpan<char> text,
        ReadOnlySpan<char> tooltip,
        bool wrap = false,
        ImTextOverflow overflow = ImTextOverflow.Overflow)
    {
        Text(gui, text, tooltip, ImText.GetTextSettings(gui, wrap, overflow));
    }

    /// <summary>
    /// Renders text with a tooltip and custom color using default text settings
    /// </summary>
    /// <param name="gui">The ImGui instance</param>
    /// <param name="text">The text to display</param>
    /// <param name="tooltip">The tooltip text to show when hovering over the text</param>
    /// <param name="color">The color of the text</param>
    /// <param name="wrap">Whether to wrap the text</param>
    /// <param name="overflow">How to handle text overflow</param>
    public static void Text(
        this ImGui gui,
        ReadOnlySpan<char> text,
        ReadOnlySpan<char> tooltip,
        Color32 color,
        bool wrap = false,
        ImTextOverflow overflow = ImTextOverflow.Overflow)
    {
        Text(gui, text, tooltip, ImText.GetTextSettings(gui, wrap, overflow), color);
    }

    /// <summary>
    /// Renders text with a tooltip at a specific rectangle using default text settings
    /// </summary>
    /// <param name="gui">The ImGui instance</param>
    /// <param name="text">The text to display</param>
    /// <param name="tooltip">The tooltip text to show when hovering over the text</param>
    /// <param name="rect">The rectangle where the text should be rendered</param>
    /// <param name="wrap">Whether to wrap the text</param>
    /// <param name="overflow">How to handle text overflow</param>
    public static void Text(
        this ImGui gui,
        ReadOnlySpan<char> text,
        ReadOnlySpan<char> tooltip,
        ImRect rect,
        bool wrap = false,
        ImTextOverflow overflow = ImTextOverflow.Overflow)
    {
        Text(gui, text, tooltip, ImText.GetTextSettings(gui, wrap, overflow), rect);
    }

    /// <summary>
    /// Renders text with a tooltip, custom color, and at a specific rectangle using default text settings
    /// </summary>
    /// <param name="gui">The ImGui instance</param>
    /// <param name="text">The text to display</param>
    /// <param name="tooltip">The tooltip text to show when hovering over the text</param>
    /// <param name="color">The color of the text</param>
    /// <param name="rect">The rectangle where the text should be rendered</param>
    /// <param name="wrap">Whether to wrap the text</param>
    /// <param name="overflow">How to handle text overflow</param>
    public static void Text(
        this ImGui gui,
        ReadOnlySpan<char> text,
        ReadOnlySpan<char> tooltip,
        Color32 color,
        ImRect rect,
        bool wrap = false,
        ImTextOverflow overflow = ImTextOverflow.Overflow)
    {
        Text(gui, text, tooltip, ImText.GetTextSettings(gui, wrap, overflow), color, rect);
    }

    /// <summary>
    /// Renders text with automatic size adjustment to fit within the specified rectangle
    /// </summary>
    /// <param name="gui">The ImGui instance</param>
    /// <param name="text">The text to display</param>
    /// <param name="tooltip">The tooltip text to show when hovering over the text</param>
    /// <param name="rect">The rectangle where the text should be rendered</param>
    /// <param name="wrap">Whether to wrap the text</param>
    public static void TextAutoSize(
        this ImGui gui,
        ReadOnlySpan<char> text,
        ReadOnlySpan<char> tooltip,
        ImRect rect,
        bool wrap = false)
    {
        TextAutoSize(gui, text, tooltip, gui.Style.Text.Color, rect, wrap);
    }

    /// <summary>
    /// Renders text with automatic size adjustment to fit within the specified rectangle, with custom color and alignment
    /// </summary>
    /// <param name="gui">The ImGui instance</param>
    /// <param name="text">The text to display</param>
    /// <param name="tooltip">The tooltip text to show when hovering over the text</param>
    /// <param name="color">The color of the text</param>
    /// <param name="rect">The rectangle where the text should be rendered</param>
    /// <param name="wrap">Whether to wrap the text</param>
    /// <param name="overflow">How to handle text overflow</param>
    /// <param name="alignment">The alignment of the text</param>
    public static void TextAutoSize(
        this ImGui gui,
        ReadOnlySpan<char> text,
        ReadOnlySpan<char> tooltip,
        Color32 color,
        ImRect rect,
        bool wrap = false,
        ImTextOverflow overflow = ImTextOverflow.Overflow,
        ImAlignment alignment = default)
    {
        // (artem-s): at least try to skip costly auto-sizing
        if (gui.Canvas.Cull(rect))
        {
            return;
        }

        var settings = ImText.GetTextSettings(gui, wrap, overflow);
        settings.Align = alignment;
        settings.Size = gui.AutoSizeTextSlow(text, settings, rect.Size);
        Text(gui, text, tooltip, settings, color, rect);
    }

    /// <summary>
    /// Renders text with a tooltip using custom text settings
    /// </summary>
    /// <param name="gui">The ImGui instance</param>
    /// <param name="text">The text to display</param>
    /// <param name="tooltip">The tooltip text to show when hovering over the text</param>
    /// <param name="settings">The text settings to use</param>
    public static void Text(
        this ImGui gui,
        ReadOnlySpan<char> text,
        ReadOnlySpan<char> tooltip,
        in ImTextSettings settings)
    {
        Text(gui, text, tooltip, in settings, gui.Style.Text.Color);
    }

    /// <summary>
    /// Renders text with a tooltip using custom text settings and color, with automatic layout
    /// </summary>
    /// <param name="gui">The ImGui instance</param>
    /// <param name="text">The text to display</param>
    /// <param name="tooltip">The tooltip text to show when hovering over the text</param>
    /// <param name="settings">The text settings to use</param>
    /// <param name="color">The color of the text</param>
    public static void Text(
        this ImGui gui,
        ReadOnlySpan<char> text,
        ReadOnlySpan<char> tooltip,
        in ImTextSettings settings,
        Color32 color)
    {
        gui.AddSpacingIfLayoutFrameNotEmpty();

        var space = gui.Layout.GetAvailableSize().Max(MIN_WIDTH, MIN_HEIGHT);
        var rect = gui.Layout.GetRect(space);
        gui.Canvas.Text(text, color, rect, in settings, out var textRect);
        gui.Layout.AddRect(textRect);
        DrawTooltip(gui, tooltip, textRect);
    }

    /// <summary>
    /// Renders text with a tooltip at a specific rectangle using custom text settings
    /// </summary>
    /// <param name="gui">The ImGui instance</param>
    /// <param name="text">The text to display</param>
    /// <param name="tooltip">The tooltip text to show when hovering over the text</param>
    /// <param name="settings">The text settings to use</param>
    /// <param name="rect">The rectangle where the text should be rendered</param>
    public static void Text(
        this ImGui gui,
        ReadOnlySpan<char> text,
        ReadOnlySpan<char> tooltip,
        in ImTextSettings settings,
        ImRect rect)
    {
        gui.Canvas.Text(text, gui.Style.Text.Color, rect, in settings);
        DrawTooltip(gui, tooltip, rect);
    }

    /// <summary>
    /// Renders text with a tooltip at a specific rectangle using custom text settings and color
    /// </summary>
    /// <param name="gui">The ImGui instance</param>
    /// <param name="text">The text to display</param>
    /// <param name="tooltip">The tooltip text to show when hovering over the text</param>
    /// <param name="settings">The text settings to use</param>
    /// <param name="color">The color of the text</param>
    /// <param name="rect">The rectangle where the text should be rendered</param>
    public static void Text(
        this ImGui gui,
        ReadOnlySpan<char> text,
        ReadOnlySpan<char> tooltip,
        in ImTextSettings settings,
        Color32 color,
        ImRect rect)
    {
        gui.Canvas.Text(text, color, rect, in settings);
        DrawTooltip(gui, tooltip, rect);
    }

    private static void DrawTooltip(ImGui gui, ReadOnlySpan<char> tooltip, ImRect rect)
    {
        if (rect.Contains(gui.Input.MousePosition))
        {
            gui.Tooltip(tooltip, gui.Input.MousePosition + gui.Style.Tooltip.OffsetPixels / gui.Canvas.ScreenScale);
        }
    }
}