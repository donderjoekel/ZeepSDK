using System.Collections.Generic;
using Imui.Controls;
using Imui.Core;
using UnityEngine;

namespace ZeepSDK.Settings.Drawers;

/// <summary>
/// Draws grouped mod settings content in a tabbed layout.
/// </summary>
public sealed class ZeepSettingsTabbedSectionsDrawer : IZeepSettingsDrawer
{
    internal readonly struct TabContent
    {
        public string Label { get; }
        public IReadOnlyList<IZeepSettingsDrawer> Drawers { get; }

        public TabContent(string label, IReadOnlyList<IZeepSettingsDrawer> drawers)
        {
            Label = label;
            Drawers = drawers;
        }
    }

    private readonly IReadOnlyList<TabContent> _tabs;
    private int _selectedTabIndex;

    internal ZeepSettingsTabbedSectionsDrawer(IReadOnlyList<TabContent> tabs)
    {
        _tabs = tabs;
    }

    /// <inheritdoc />
    public void Draw(ImGui gui, ZeepSettingsDrawContext context)
    {
        if (_tabs == null || _tabs.Count == 0)
            return;

        _selectedTabIndex = Mathf.Clamp(_selectedTabIndex, 0, _tabs.Count - 1);

        using (gui.Indent())
        {
            gui.PushId("tabs");
            {
                using (gui.Horizontal(gui.GetLayoutWidth()))
                {
                    for (var i = 0; i < _tabs.Count; i++)
                    {
                        var tab = _tabs[i];
                        gui.PushId(gui.GetControlId((uint)i));
                        {
                            var controlId = gui.GetNextControlId();
                            var rect = ImTabsPane.AddButtonRect(gui, tab.Label, default);
                            if (ImTabsPane.TabBarButton(gui, controlId, _selectedTabIndex == i, tab.Label, rect))
                                _selectedTabIndex = i;
                        }
                        gui.PopId();
                    }
                }
            }
            gui.PopId();

            if (gui.Style.Tabs.SeparatorThickness > 0f)
            {
                var sepRect = gui.AddLayoutRect(gui.GetLayoutWidth(), gui.Style.Tabs.SeparatorThickness);
                gui.Canvas.Rect(sepRect, gui.Style.Tabs.SeparatorColor);
            }

            gui.AddSpacing(gui.Style.Layout.Spacing);

            gui.PushId("content");
            {
                gui.PushId(gui.GetControlId((uint)_selectedTabIndex));
                {
                    foreach (var drawer in _tabs[_selectedTabIndex].Drawers)
                        drawer.Draw(gui, context);
                }
                gui.PopId();
            }
            gui.PopId();
        }
    }
}
