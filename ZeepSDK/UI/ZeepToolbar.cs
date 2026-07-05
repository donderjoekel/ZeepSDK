using System.Collections.Generic;
using Imui.Controls;
using Imui.Core;
using UnityEngine;
using ZeepSDK.Settings;
using ZeepSDK.Terminal;

namespace ZeepSDK.UI;

internal class ZeepToolbar : IZeepGUIDrawer
{
    private readonly HashSet<IZeepToolbarDrawer> _drawers = [];
    private bool _visible;

    public void AddToolbarDrawer(IZeepToolbarDrawer drawer)
    {
        _drawers.Add(drawer);
    }

    public void RemoveToolbarDrawer(IZeepToolbarDrawer drawer)
    {
        _drawers.Remove(drawer);
    }

    public void OnZeepGUI(ImGui gui)
    {
        if (Input.GetKeyDown(Plugin.Instance.ToggleMenuBarKey.Value))
            _visible = !_visible;

        if (!_visible)
            return;
        
        gui.BeginMenuBar();
        if (gui.BeginMenu("File"))
        {
            if (gui.Menu("Settings"))
            {
                SettingsApi.OpenModSettings();
            }

            if (gui.Menu("Terminal"))
            {
                TerminalApi.Toggle();
            }

            gui.Separator();
            if (gui.Menu("Exit"))
            {
                Application.Quit();
            }

            gui.EndMenu();
        }

        foreach (var drawer in _drawers)
        {
            if (gui.BeginMenu(drawer.MenuTitle))
            {
                drawer.DrawMenuItems(gui);
                gui.EndMenu();
            }
        }

        gui.EndMenuBar();
    }
}