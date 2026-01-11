using Imui.Core;

namespace ZeepSDK.UI.RectEx;

public static class MoveToExtensions
{
    private const float SPACE = 2f;

    public static ImRect MoveRight(this ImRect rect, float space = SPACE)
    {
        rect = rect.Abs();
        rect.X += rect.W + space;
        return rect;
    }

    public static ImRect MoveRightFor(this ImRect rect, float newWidth, float space = SPACE)
    {
        rect = rect.Abs();
        rect.X += rect.W + space;
        rect.W = newWidth;
        return rect.Abs();
    }

    public static ImRect MoveLeft(this ImRect rect, float space = SPACE)
    {
        rect = rect.Abs();
        rect.X -= rect.W + space;
        return rect;
    }

    public static ImRect MoveLeftFor(this ImRect rect, float newWidth, float space = SPACE)
    {
        rect = rect.Abs();
        rect.X -= newWidth + space;
        rect.W = newWidth;
        return rect.Abs();
    }

    public static ImRect MoveUp(this ImRect rect, float space = SPACE)
    {
        rect = rect.Abs();
        rect.Y -= rect.H + space;
        return rect;
    }

    public static ImRect MoveUpFor(this ImRect rect, float newHeight, float space = SPACE)
    {
        rect = rect.Abs();
        rect.Y -= newHeight + space;
        rect.H = newHeight;
        return rect.Abs();
    }

    public static ImRect MoveDown(this ImRect rect, float space = SPACE)
    {
        rect = rect.Abs();
        rect.Y += rect.H + space;
        return rect;
    }

    public static ImRect MoveDownFor(this ImRect rect, float newHeight, float space = SPACE)
    {
        rect = rect.Abs();
        rect.Y += rect.H + space;
        rect.H = newHeight;
        return rect.Abs();
    }
}