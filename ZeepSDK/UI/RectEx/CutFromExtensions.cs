using System;
using Imui.Core;

namespace ZeepSDK.UI.RectEx;

public static class CutFromExtensions
{
    private const float SPACE = 2;

    public static ImRect[] CutFromRight(this ImRect rect, float width, float space = SPACE)
    {
        ImRect second = RectExUtils.MinMaxRect(
            xmin: rect.Right - width,
            xmax: rect.Right,
            ymin: rect.Bottom,
            ymax: rect.Top
        );
        float min = Math.Min(rect.Left, second.Left - space);
        ImRect first = RectExUtils.MinMaxRect(
            xmin: min,
            xmax: second.Left - space,
            ymin: rect.Bottom,
            ymax: rect.Top
        );
        return [first, second];
    }

    public static ImRect[] CutFromBottom(this ImRect rect, float height, float space = SPACE)
    {
        ImRect second = RectExUtils.MinMaxRect(
            xmin: rect.Left,
            xmax: rect.Right,
            ymin: rect.Top - height,
            ymax: rect.Top
        );
        float min = Math.Min(rect.Bottom, second.Bottom - space);
        ImRect first = RectExUtils.MinMaxRect(
            xmin: rect.Left,
            xmax: rect.Right,
            ymin: min,
            ymax: second.Bottom - space
        );
        return [first, second];
    }

    public static ImRect[] CutFromLeft(this ImRect rect, float width, float space = SPACE)
    {
        ImRect first = RectExUtils.MinMaxRect(
            xmin: rect.Left,
            xmax: rect.Left + width,
            ymin: rect.Bottom,
            ymax: rect.Top
        );
        float max = Math.Max(rect.Right, first.Right + space);
        ImRect second = RectExUtils.MinMaxRect(
            xmin: first.Right + space,
            xmax: max,
            ymin: rect.Bottom,
            ymax: rect.Top
        );
        return [first, second];
    }

    public static ImRect[] CutFromTop(this ImRect rect, float height, float space = SPACE)
    {
        ImRect first = RectExUtils.MinMaxRect(
            xmin: rect.Left,
            xmax: rect.Right,
            ymin: rect.Bottom,
            ymax: rect.Bottom + height
        );
        float max = Math.Max(rect.Top, first.Top + space);
        ImRect second = RectExUtils.MinMaxRect(
            xmin: rect.Left,
            xmax: rect.Right,
            ymin: first.Top + space,
            ymax: max
        );
        return [first, second];
    }
}