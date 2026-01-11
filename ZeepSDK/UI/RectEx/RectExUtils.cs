using Imui.Core;

namespace ZeepSDK.UI.RectEx;

internal static class RectExUtils
{
    public static ImRect MinMaxRect(float xmin, float xmax, float ymin, float ymax)
    {
        return new ImRect(xmin, ymin, xmax - xmin, ymax - ymin);
    }
}