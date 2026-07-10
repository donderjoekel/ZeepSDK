using System.Linq;
using Imui.Core;

namespace ZeepSDK.UI.RectEx;

/// <summary>Provides vertical rectangle layout helpers.</summary>
public static class RowExtensions
{
    private const float SPACE = 2f;

    /// <summary>Splits a rectangle into equally weighted rows.</summary>
    public static ImRect[] Row(this ImRect rect, int count, float space = SPACE)
    {
        rect = rect.Invert();
        ImRect[] result = ColumnExtensions.Column(rect, count, space);
        return result.Select(x => x.Invert()).ToArray();
    }

    /// <summary>Splits a rectangle into weighted rows.</summary>
    public static ImRect[] Row(this ImRect rect, float[] weights, float space = SPACE)
    {
        return Row(rect, weights, null, space);
    }

    /// <summary>Splits a rectangle into weighted and fixed-height rows.</summary>
    public static ImRect[] Row(this ImRect rect, float[] weights, float[] widths, float space = SPACE)
    {
        rect = rect.Invert();
        ImRect[] result = ColumnExtensions.Column(rect, weights, widths, space);
        return result.Select(x => x.Invert()).ToArray();
    }
}
