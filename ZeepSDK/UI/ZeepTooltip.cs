using System.Collections.Generic;
using Imui.Controls;
using Imui.Core;
using Imui.Rendering;
using Imui.Style;
using UnityEngine;

namespace ZeepSDK.UI;

internal class ZeepTooltip : IZeepGUIDrawer
{
    private readonly HashSet<IZeepTooltip> _tooltips = [];

    public void AddTooltip(IZeepTooltip zeepTooltip)
    {
        _tooltips.Add(zeepTooltip);
    }

    public void RemoveTooltip(IZeepTooltip zeepTooltip)
    {
        _tooltips.Remove(zeepTooltip);
    }
    
    public void OnZeepGUI(ImGui gui)
    {
        var boundsRect = gui.Layout.GetBoundsRect();        
        var mousePosition = gui.Input.MousePosition + gui.Style.Tooltip.OffsetPixels;
        var altMousePosition = gui.Input.MousePosition + new Vector2(-gui.Style.Tooltip.OffsetPixels.x, gui.Style.Tooltip.OffsetPixels.y);

        foreach (var zeepTooltip in _tooltips)
        {
            if (zeepTooltip == null)
                continue;

            if (!zeepTooltip.IsOver)
                continue;

            var size = gui.MeasureTextSize(
                zeepTooltip.Content,
                ImTooltip.GetTextSettings(gui));

            var tooltipRect = new ImRect(mousePosition, size)
                .WithPadding(-gui.Style.Tooltip.Padding);

            if (!boundsRect.Contains(tooltipRect.TopLeft) || !boundsRect.Contains(tooltipRect.BottomRight))
            {
                tooltipRect = new ImRect(
                    new Vector2(altMousePosition.x - size.x, altMousePosition.y),
                    tooltipRect.Size);
            }

            gui.Tooltip(zeepTooltip.Content, tooltipRect);
        }
    }
}