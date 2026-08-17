using System;
using System.Linq;
using Imui.Core;

namespace ZeepSDK.UI.RectEx;

/// <summary>Provides rectangle normalization and sizing helpers.</summary>
public static class MiscellaniousExtensions
{
    /// <summary>Normalizes negative rectangle dimensions.</summary>
    public static ImRect Abs(this ImRect rect)
    {
        if (rect.W < 0)
        {
            rect.X += rect.W;
            rect.W *= -1;
        }

        if (rect.H < 0)
        {
            rect.Y += rect.H;
            rect.H *= -1;
        }

        return rect;
    }

    /// <summary>Swaps horizontal and vertical rectangle dimensions.</summary>
    public static ImRect Invert(this ImRect rect)
    {
        return new ImRect(
            x: rect.Y,
            y: rect.X,
            w: rect.H,
            h: rect.W
        );
    }

    /// <summary>Returns bounds containing this rectangle and all supplied rectangles.</summary>
    public static ImRect Union(this ImRect rect, params ImRect[] other)
    {
        if (other == null || other.Length == 0)
        {
            return rect;
        }

        if (other.Length == 1 && other[0] == rect)
        {
            return rect;
        }

        float xMin = Math.Min(rect.Left, other.Select(x => x.Left).Aggregate(Math.Min));
        float yMin = Math.Min(rect.Bottom, other.Select(x => x.Bottom).Aggregate(Math.Min));
        float xMax = Math.Max(rect.Right, other.Select(x => x.Right).Aggregate(Math.Max));
        float yMax = Math.Max(rect.Top, other.Select(x => x.Top).Aggregate(Math.Max));
        return RectExUtils.MinMaxRect(
            xmin: xMin,
            xmax: xMax,
            ymin: yMin,
            ymax: yMax
        );
    }

    /// <summary>Insets a rectangle by a border width.</summary>
    public static ImRect Intend(this ImRect rect, float border)
    {
        rect = rect.Abs();

        ImRect result = new(
            x: rect.X + border,
            y: rect.Y + border,
            w: rect.W - 2 * border,
            h: rect.H - 2 * border
        );

        if (result.W < 0)
        {
            result.X += result.W / 2;
            result.W = 0;
        }

        if (result.H < 0)
        {
            result.Y += result.H / 2;
            result.H = 0;
        }

        return result;
    }

    /// <summary>Expands a rectangle by a border width.</summary>
    public static ImRect Extend(this ImRect rect, float border)
    {
        rect = rect.Abs();
        return new ImRect(
            x: rect.X - border,
            y: rect.Y - border,
            w: rect.W + 2 * border,
            h: rect.H + 2 * border
        );
    }

    /// <summary>Returns the first horizontal line with the requested height.</summary>
    public static ImRect FirstLine(this ImRect rect, float height = 18)
    {
        rect = rect.Abs();
        rect.H = height;
        return rect.Abs();
    }
}
