using System;
using System.Collections.Generic;
using System.Linq;
using Imui.Core;
using ZeepSDK.UI.RectEx.Internal;

namespace ZeepSDK.UI.RectEx;

public static class RowExtensions
{
    private const float SPACE = 2f;

    public static ImRect[] Row(this ImRect rect, int count, float space = SPACE)
    {
        rect = rect.Abs();
        switch (count)
        {
            case 1:
            {
                return [rect];
            }
            case 2:
            {
                return RowTwoSlices(rect, space);
            }
            case 3:
            {
                return RowThreeSlices(rect, space);
            }
            default:
            {
                float[] weights = Enumerable.Repeat(1f, count).ToArray();
                float[] widths = Enumerable.Repeat(0f, count).ToArray();
                return Row(rect, weights, widths, space);
            }
        }
    }

    public static ImRect[] Row(this ImRect rect, float[] weights, float space = SPACE)
    {
        return Row(rect, weights, null, space);
    }

    public static ImRect[] Row(this ImRect rect, float[] weights, float[] widths, float space = SPACE)
    {
        if (weights == null)
        {
            throw new ArgumentException("Weights is null. You must specify it");
        }

        if (widths == null)
        {
            widths = Enumerable.Repeat(0f, weights.Length).ToArray();
        }

        rect = rect.Abs();
        return RowSafe(rect, weights, widths, space);
    }

    private static ImRect[] RowTwoSlices(ImRect rect, float space)
    {
        ImRect first = new(
            x: rect.X,
            y: rect.Y,
            w: (rect.W - space) / 2,
            h: rect.H
        );
        ImRect second = new(
            x: first.X + space + first.W,
            y: first.Y,
            w: first.W,
            h: first.H
        );
        return [first, second];
    }

    private static ImRect[] RowThreeSlices(ImRect rect, float space)
    {
        ImRect first = new(
            x: rect.X,
            y: rect.Y,
            w: (rect.W - 2 * space) / 3,
            h: rect.H
        );
        ImRect second = new(
            x: first.X + first.W + space,
            y: rect.Y,
            w: first.W,
            h: first.H
        );
        ImRect third = new(
            x: second.X + second.W + space,
            y: second.Y,
            w: second.W,
            h: second.H
        );
        return [first, second, third];
    }

    private static ImRect[] RowSafe(ImRect rect, float[] weights, float[] widths, float space)
    {
        IEnumerable<Cell> cells = weights.Merge(widths, (weight, width) => new Cell(weight, width)).Where(cell => cell.HasWidth);

        float weightUnit = GetWeightUnit(rect.W, cells, space);

        List<ImRect> result = [];
        float nextX = rect.X;
        foreach (Cell cell in cells)
        {
            result.Add(new ImRect(
                x: nextX,
                y: rect.Y,
                w: cell.GetWidth(weightUnit),
                h: rect.H
            ));

            nextX += cell.HasWidth ? (cell.GetWidth(weightUnit) + space) : 0;
        }

        return result.ToArray();
    }

    private static float GetWeightUnit(float fullWidth, IEnumerable<Cell> cells, float space)
    {
        float result = 0;
        float weightsSum = cells.Sum(cell => cell.Weight);

        if (weightsSum > 0)
        {
            float fixedWidth = cells.Sum(cell => cell.FixedWidth);
            float spacesWidth = (cells.Count(cell => cell.HasWidth) - 1) * space;
            result = (fullWidth - fixedWidth - spacesWidth) / weightsSum;
        }

        return result;
    }

    private class Cell
    {
        public float Weight { get; private set; }
        public float FixedWidth { get; private set; }

        public Cell(float weight, float fixedWidth)
        {
            this.Weight = weight;
            this.FixedWidth = fixedWidth;

        }

        public bool HasWidth => FixedWidth > 0 || Weight > 0;

        public float GetWidth(float weightUnit)
        {
            return FixedWidth + Weight * weightUnit;
        }
    }
}