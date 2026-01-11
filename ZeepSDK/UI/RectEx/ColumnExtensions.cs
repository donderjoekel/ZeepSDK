using System.Linq;
using Imui.Core;

namespace ZeepSDK.UI.RectEx;

public static class ColumnExtensions
{
    private const float SPACE = 2f;

    public static ImRect[] Column(this ImRect rect, int count, float space = SPACE)
    {
        rect = rect.Invert();
        ImRect[] result = rect.Row(count, space);
        return result.Select(x => x.Invert()).ToArray();
    }

    public static ImRect[] Column(this ImRect rect, float[] weights, float space = SPACE)
    {
        return Column(rect, weights, null, space);
    }

    public static ImRect[] Column(this ImRect rect, float[] weights, float[] widths, float space = SPACE)
    {
        rect = rect.Invert();
        ImRect[] result = rect.Row(weights, widths, space);
        return result.Select(x => x.Invert()).ToArray();
    }
}