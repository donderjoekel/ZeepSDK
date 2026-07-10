using System.Collections.Generic;
using Imui.Controls;
using Imui.Core;

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
    private readonly float _defaultHeight;

    internal ZeepSettingsTabbedSectionsDrawer(
        IReadOnlyList<TabContent> tabs,
        float defaultHeight = 560f)
    {
        _tabs = tabs;
        _defaultHeight = defaultHeight;
    }

    /// <inheritdoc />
    public void Draw(ImGui gui, ZeepSettingsDrawContext context)
    {
        if (_tabs == null || _tabs.Count == 0)
            return;

        var height = context.AvailableContentHeight > 0f
            ? context.AvailableContentHeight
            : _defaultHeight;

        using (gui.Indent())
        {
            gui.BeginTabsPane(gui.AddLayoutRect(gui.GetLayoutWidth(), height));
            foreach (var tab in _tabs)
            {
                if (gui.BeginTab(tab.Label))
                {
                    foreach (var drawer in tab.Drawers)
                        drawer.Draw(gui, context);

                    gui.EndTab();
                }
            }

            gui.EndTabsPane();
        }
    }
}
