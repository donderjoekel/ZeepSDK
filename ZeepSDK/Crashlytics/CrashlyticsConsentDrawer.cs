using System;
using Imui.Controls;
using Imui.Core;
using Imui.Rendering;
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
            ImRect rect = ImWindow.GetInitialWindowRect(gui, new ImSize(760, 430));

            gui.Canvas.PushClipRect(rect);
            gui.Canvas.PushRectMask(rect, 0);
            gui.Box(rect, gui.Style.Window.Box);
            gui.RegisterControl(id, rect);
            gui.RegisterGroup(id, rect);
            gui.Layout.Push(ImAxis.Vertical, rect.WithPadding(24));

            gui.Text("Crash reporting", new ImTextSettings(36, 0.5f));
            gui.AddSpacing(12);
            gui.Text(
                "ZeepSDK crash reporting is disabled by default. If enabled, reports sent to Bugsnag include " +
                "your Steam identity, installed mod names and versions, and exception details.",
                new ImTextSettings(18, 0.5f, 0.5f, true));
            gui.AddSpacing(18);
            gui.Text(
                "Choose Accept to enable crash reporting or Decline to keep it disabled. " +
                "You can change this later in Zeep Settings.",
                new ImTextSettings(18, 0.5f, 0.5f, true));

            gui.AddSpacing(gui.GetLayoutHeight() - gui.GetRowsHeightWithSpacing(2));

            if (gui.Button("Accept (default)"))
                Decide(true);

            if (_active && gui.Button("Decline"))
                Decide(false);

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
