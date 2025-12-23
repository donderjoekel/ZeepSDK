using System;
using System.Collections.Generic;
using Imui.Core;
using Imui.Style;
using UnityEngine;
using ZeepSDK.UI.Patches;

namespace ZeepSDK.UI;

internal class ZeepGUI : MonoBehaviour
{
    private readonly HashSet<IZeepGUIDrawer> _drawers = [];
    private ImGui _gui;
    
    private void Awake()
    {
        ImuiUnityGUIBackend_Awake.Awake += backend =>
        {
            _gui = new ImGui(backend, backend);
        };
        
        Plugin.Instance.Theme.SettingChanged += OnThemeChanged;
    }

    private void OnThemeChanged(object sender, EventArgs e)
    {
        if (_gui == null)
            return;

        var style = Plugin.Instance.Theme.Value;
        var theme = ZeepStyleToImTheme(style);
        _gui.SetTheme(theme);
    }

    private static ImTheme ZeepStyleToImTheme(ZeepStyle style)
    {
        return style switch
        {
            ZeepStyle.Light => ImThemeBuiltin.Light(),
            ZeepStyle.LightTough => ImThemeBuiltin.LightTouch(),
            ZeepStyle.Dark => ImThemeBuiltin.Dark(),
            ZeepStyle.DarkTouch => ImThemeBuiltin.DarkTouch(),
            ZeepStyle.Dear => ImThemeBuiltin.Dear(),
            ZeepStyle.Orange => ImThemeBuiltin.Orange(),
            ZeepStyle.Terminal => ImThemeBuiltin.Terminal(),
            ZeepStyle.Wire => ImThemeBuiltin.Wire(),
            _ => throw new ArgumentOutOfRangeException(nameof(style), style, null)
        };
    }

    public void AddZeepGUIDrawer(IZeepGUIDrawer drawer)
    {
        _drawers.Add(drawer);
    }

    public void RemoveZeepGUIDrawer(IZeepGUIDrawer drawer)
    {
        _drawers.Remove(drawer);
    }

    private void Update()
    {
        if (_gui == null) return;

        _gui.BeginFrame();

        foreach (IZeepGUIDrawer drawer in _drawers)
        {
            drawer.OnZeepGUI(_gui);
        }

        _gui.EndFrame();
        _gui.Render();
    }
}