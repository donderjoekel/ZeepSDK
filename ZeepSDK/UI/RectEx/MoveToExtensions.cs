using Imui.Core;

namespace ZeepSDK.UI.RectEx;

/// <summary>Provides rectangle translation helpers.</summary>
public static class MoveToExtensions
{
    private const float SPACE = 2f;

    /// <summary>Moves a rectangle right by its width and spacing.</summary>
    public static ImRect MoveRight(this ImRect rect, float space = SPACE)
    {
        rect = rect.Abs();
        rect.X += rect.W + space;
        return rect;
    }

    /// <summary>Moves a rectangle right and applies a new width.</summary>
    public static ImRect MoveRightFor(this ImRect rect, float newWidth, float space = SPACE)
    {
        rect = rect.Abs();
        rect.X += rect.W + space;
        rect.W = newWidth;
        return rect.Abs();
    }

    /// <summary>Moves a rectangle left by its width and spacing.</summary>
    public static ImRect MoveLeft(this ImRect rect, float space = SPACE)
    {
        rect = rect.Abs();
        rect.X -= rect.W + space;
        return rect;
    }

    /// <summary>Moves a rectangle left and applies a new width.</summary>
    public static ImRect MoveLeftFor(this ImRect rect, float newWidth, float space = SPACE)
    {
        rect = rect.Abs();
        rect.X -= newWidth + space;
        rect.W = newWidth;
        return rect.Abs();
    }

    /// <summary>Moves a rectangle up by its height and spacing.</summary>
    public static ImRect MoveUp(this ImRect rect, float space = SPACE)
    {
        rect = rect.Abs();
        rect.Y -= rect.H + space;
        return rect;
    }

    /// <summary>Moves a rectangle up and applies a new height.</summary>
    public static ImRect MoveUpFor(this ImRect rect, float newHeight, float space = SPACE)
    {
        rect = rect.Abs();
        rect.Y -= newHeight + space;
        rect.H = newHeight;
        return rect.Abs();
    }

    /// <summary>Moves a rectangle down by its height and spacing.</summary>
    public static ImRect MoveDown(this ImRect rect, float space = SPACE)
    {
        rect = rect.Abs();
        rect.Y += rect.H + space;
        return rect;
    }

    /// <summary>Moves a rectangle down and applies a new height.</summary>
    public static ImRect MoveDownFor(this ImRect rect, float newHeight, float space = SPACE)
    {
        rect = rect.Abs();
        rect.Y += rect.H + space;
        rect.H = newHeight;
        return rect.Abs();
    }
}
