using System;
using Imui.Controls;
using Imui.Core;
using Imui.Style;
using UnityEngine;
using ZeepSDK.Controls;
using ZeepSDK.UI;
using ZeepSDK.Utilities;

namespace ZeepSDK.Crashlytics;

internal sealed class CrashlyticsConsentDrawer : IZeepGUIDrawer, IDisposable
{
    private readonly Action<bool> _onDecision;
    private readonly DisposableBag _inputOverride;
    private bool _active = true;
    private bool _acceptDefaultArmed;

    public CrashlyticsConsentDrawer(Action<bool> onDecision)
    {
        _onDecision = onDecision ?? throw new ArgumentNullException(nameof(onDecision));
        _inputOverride = ControlsApi.DisableAllInputExceptEventSystem(() => _active);
    }

    public void OnZeepGUI(ImGui gui)
    {
        if (!_active)
            return;

        uint id = gui.PushId("CrashlyticsConsentPopup");
        try
        {
            gui.BeginPopup();
            ImRect rect = ImWindow.GetInitialWindowRect(gui, new ImSize(820, 460));

            gui.Canvas.PushClipRect(rect);
            gui.Canvas.PushRectMask(rect, 0);
            gui.Box(rect, gui.Style.Window.Box);
            gui.RegisterControl(id, rect);
            gui.RegisterGroup(id, rect);
            gui.Layout.Push(ImAxis.Vertical, rect.WithPadding(24));

            gui.Text("Crash reporting", new ImTextSettings(36, 0.5f));
            gui.AddSpacing(8);
            gui.Text(
                "Help ZeepSDK and other mod developers fix crashes faster by sharing crash reports. " +
                "Good reports make bugs much easier to reproduce and make mod developers' lives easier.",
                new ImTextSettings(18, 0f, 0f, true));
            gui.AddSpacing(12);
            gui.Text(
                "Reports are sent to Bugsnag only after you accept. They include your Steam identity, " +
                "installed mod names and versions, and exception details. You can change this choice later " +
                "in Zeep Settings.",
                new ImTextSettings(18, 0f, 0f, true));

            const float buttonWidth = 220;
            const float buttonHeight = 52;
            const float buttonSpacing = 12;
            gui.AddSpacing(Mathf.Max(12, gui.GetLayoutHeight() - buttonHeight));

            using (gui.Horizontal(gui.GetLayoutWidth(), buttonHeight))
            {
                gui.AddSpacing(Mathf.Max(0, gui.GetLayoutWidth() - buttonWidth * 2 - buttonSpacing));

                if (gui.Button("Decline", new ImSize(buttonWidth, buttonHeight)))
                    Decide(false);

                gui.AddSpacing(buttonSpacing);

                if (_active && DrawAcceptButton(gui, buttonWidth, buttonHeight))
                    Decide(true);
            }

            if (_acceptDefaultArmed)
                TryAcceptDefault(gui);
            else
                _acceptDefaultArmed = true;

            gui.Layout.Pop();
            gui.Canvas.PopClipRect();
            gui.Canvas.PopClipRect();
            gui.EndPopup();
        }
        finally
        {
            gui.PopId();
        }
    }

    private static bool DrawAcceptButton(ImGui gui, float width, float height)
    {
        ImStyleButton original = gui.Style.Button;
        ImStyleButton highlighted = gui.Style.AccentButton;
        highlighted.Normal.BackColor = new Color32(38, 150, 76, 255);
        highlighted.Normal.FrontColor = new Color32(255, 255, 255, 255);
        highlighted.Hovered.BackColor = new Color32(48, 180, 92, 255);
        highlighted.Hovered.FrontColor = new Color32(255, 255, 255, 255);
        highlighted.Pressed.BackColor = new Color32(30, 120, 60, 255);
        highlighted.Pressed.FrontColor = new Color32(255, 255, 255, 255);

        gui.Style.Button = highlighted;
        try
        {
            return gui.Button("Accept", new ImSize(width, height));
        }
        finally
        {
            gui.Style.Button = original;
        }
    }

    public void Dispose()
    {
        if (!_active)
            return;

        _active = false;
        _inputOverride.Dispose();
    }

    private void TryAcceptDefault(ImGui gui)
    {
        if (!_active)
            return;

        for (int index = gui.Input.KeyboardEventsCount - 1; index >= 0; index--)
        {
            KeyCode key = gui.Input.GetKeyboardEvent(index).Key;
            if (key != KeyCode.Return && key != KeyCode.KeypadEnter)
                continue;

            gui.Input.UseKeyboardEvent(index);
            Decide(true);
            return;
        }
    }

    private void Decide(bool accepted)
    {
        if (!_active)
            return;

        _active = false;
        _inputOverride.Dispose();
        _onDecision(accepted);
    }
}
