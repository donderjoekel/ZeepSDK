using System.Collections.Generic;
using BepInEx.Configuration;
using Imui.Controls;
using Imui.Core;
using Imui.IO.Events;
using Imui.Rendering;
using UnityEngine;
using ZeepSDK.Controls;
using ZeepSDK.LevelEditor;
using ZeepSDK.UI;

namespace ZeepSDK.Terminal.UI;

internal sealed class ZeepTerminalDrawer : IZeepGUIDrawer
{
    private readonly ConfigEntry<KeyCode> toggleTerminalKey;
    private readonly TerminalOutputBuffer outputBuffer = new();
    private readonly TerminalInputController inputController = new();
    private readonly TerminalAutocomplete autocomplete = new();

    private bool open;
    private bool mouse;
    private string input = string.Empty;
    private IReadOnlyList<string> suggestions = [];
    private int selectedSuggestion;

    public bool IsOpen => open;

    public ZeepTerminalDrawer(ConfigEntry<KeyCode> toggleTerminalKey)
    {
        this.toggleTerminalKey = toggleTerminalKey;
        ControlsApi.DisableAllInputExceptEventSystem(() => open && mouse);
    }

    public void Open()
    {
        if (open)
            return;

        open = true;
        TerminalApi.DispatchOpened();
    }

    public void Close()
    {
        if (!open)
            return;

        open = false;
        UpdateMouseInputBlock();
        TerminalApi.DispatchClosed();
    }

    public void ClearOutput() => outputBuffer.Clear();

    public void OnZeepGUI(ImGui gui)
    {
        if (toggleTerminalKey.Value != KeyCode.None && Input.GetKeyDown(toggleTerminalKey.Value))
            Toggle();

        if (!open)
            return;

        if (gui.BeginWindow("Terminal", ref open, ref mouse,
                windowOpened: null,
                windowClosed: () =>
                {
                    UpdateMouseInputBlock();
                    TerminalApi.DispatchClosed();
                },
                mouseEntered: () => UpdateMouseInputBlock(mouseOver: true),
                mouseExited: () => UpdateMouseInputBlock(mouseOver: false),
                size: (900, 520)))
        {
            DrawOutput(gui);
            gui.Separator();
            DrawInput(gui);
            DrawSuggestions(gui);
            gui.EndWindow();
        }
    }

    private void Toggle()
    {
        if (open)
            Close();
        else
            Open();
    }

    private void DrawOutput(ImGui gui)
    {
        gui.AddSpacing();
        gui.BeginScrollable();

        var settings = new ImTextSettings(gui.Style.Layout.TextSize, 0.0f, 0.5f, false);
        var lineWidth = gui.GetLayoutWidth();
        var rowHeight = gui.GetRowHeight();

        foreach (CollectedTerminalLine line in outputBuffer.Lines)
        {
            Color32 background = line.Kind switch
            {
                TerminalOutputKind.Input => new Color32(48, 72, 120, 48),
                TerminalOutputKind.Error => new Color32(120, 32, 32, 48),
                _ => new Color32(0, 0, 0, 0)
            };

            Color32 foreground = line.Kind switch
            {
                TerminalOutputKind.Input => new Color32(120, 180, 255, 255),
                TerminalOutputKind.Error => new Color32(255, 120, 120, 255),
                _ => gui.Style.Text.Color
            };

            var rect = gui.AddLayoutRect(lineWidth, rowHeight);
            var textRect = rect.WithPadding(left: gui.Style.Layout.InnerSpacing);
            if (background.a > 0)
                gui.Canvas.Rect(rect, background);

            gui.Canvas.Text(line.Text, foreground, textRect, in settings);
        }

        gui.EndScrollable();
    }

    private void DrawInput(ImGui gui)
    {
        HandleInputKeyboard(gui);

        gui.Text(">", new ImTextSettings(gui.Style.Layout.TextSize, 0.5f));
        gui.TextEdit(ref input, (gui.GetLayoutWidth() - 24, gui.GetRowHeight()), hint: "Enter a command");
        suggestions = autocomplete.GetSuggestions(input);
        selectedSuggestion = Mathf.Clamp(selectedSuggestion, 0, Mathf.Max(0, suggestions.Count - 1));
    }

    private void DrawSuggestions(ImGui gui)
    {
        if (suggestions.Count == 0 || string.IsNullOrWhiteSpace(input))
            return;

        using (gui.List((gui.GetLayoutWidth(), ImList.GetEnclosingHeight(gui, gui.GetRowsHeightWithSpacing(Mathf.Min(suggestions.Count, 5))))))
        {
            for (var i = 0; i < suggestions.Count; i++)
            {
                if (gui.ListItem(i == selectedSuggestion, suggestions[i]))
                {
                    input = suggestions[i] + " ";
                    selectedSuggestion = i;
                }
            }
        }
    }

    private void HandleInputKeyboard(ImGui gui)
    {
        if (gui.Input.KeyboardEventsCount == 0)
            return;

        for (var i = 0; i < gui.Input.KeyboardEventsCount; i++)
        {
            ImKeyboardEvent keyboardEvent = gui.Input.GetKeyboardEvent(i);
            if (keyboardEvent.Type != ImKeyboardEventType.Down)
                continue;

            switch (keyboardEvent.Key)
            {
                case KeyCode.Return:
                case KeyCode.KeypadEnter:
                    SubmitInput();
                    gui.Input.UseKeyboardEvent(i);
                    break;
                case KeyCode.Tab:
                    input = autocomplete.GetTabCompletion(input, suggestions);
                    gui.Input.UseKeyboardEvent(i);
                    break;
                case KeyCode.UpArrow:
                    input = inputController.NavigateHistory(-1, input);
                    inputController.ResetHistoryNavigation();
                    gui.Input.UseKeyboardEvent(i);
                    break;
                case KeyCode.DownArrow:
                    input = inputController.NavigateHistory(1, input);
                    gui.Input.UseKeyboardEvent(i);
                    break;
            }
        }
    }

    private void SubmitInput()
    {
        string commandLine = input.Trim();
        if (string.IsNullOrWhiteSpace(commandLine))
            return;

        inputController.AddToHistory(commandLine);
        inputController.ResetHistoryNavigation();

        var collector = new TerminalOutputCollector();
        TerminalExecutor.TryExecute(commandLine, collector);
        outputBuffer.Append(collector.Lines);

        input = string.Empty;
        suggestions = [];
        selectedSuggestion = 0;
    }

    private void BlockMouseInput() => LevelEditorApi.BlockMouseInput(this);

    private void UnblockMouseInput() => LevelEditorApi.UnblockMouseInput(this);

    private void UpdateMouseInputBlock(bool? mouseOver = null)
    {
        bool hovered = mouseOver ?? mouse;
        if (open && hovered)
            BlockMouseInput();
        else
            UnblockMouseInput();
    }
}
