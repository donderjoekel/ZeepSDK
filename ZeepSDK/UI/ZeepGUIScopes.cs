using System;
using Imui.Controls;
using Imui.Core;
using UnityEngine;

/// <summary>
/// Provides extension methods for creating scoped GUI layouts using the using statement pattern
/// </summary>
public static class ZeepGUIScopes
{
    /// <summary>
    /// A scope for horizontal layout that automatically ends when disposed
    /// </summary>
    public readonly struct HorizontalScope : IDisposable
    {
        private readonly ImGui _gui;

        /// <summary>
        /// Initializes a new horizontal scope
        /// </summary>
        /// <param name="gui">The ImGui instance</param>
        /// <param name="size">The size of the horizontal layout</param>
        public HorizontalScope(ImGui gui, Vector2 size)
        {
            _gui = gui;
            _gui.BeginHorizontal(size);
        }

        /// <summary>
        /// Ends the horizontal layout
        /// </summary>
        public void Dispose()
        {
            _gui.EndHorizontal();
        }
    }

    /// <summary>
    /// A scope for vertical layout that automatically ends when disposed
    /// </summary>
    public readonly struct VerticalScope : IDisposable
    {
        private readonly ImGui _gui;

        /// <summary>
        /// Initializes a new vertical scope
        /// </summary>
        /// <param name="gui">The ImGui instance</param>
        /// <param name="size">The size of the vertical layout</param>
        public VerticalScope(ImGui gui, Vector2 size)
        {
            _gui = gui;
            _gui.BeginVertical(size);
        }

        /// <summary>
        /// Ends the vertical layout
        /// </summary>
        public void Dispose()
        {
            _gui.EndHorizontal();
        }
    }

    /// <summary>
    /// A scope for indentation that automatically ends when disposed
    /// </summary>
    public readonly struct IndentScope : IDisposable
    {
        private readonly ImGui _gui;

        /// <summary>
        /// Initializes a new indent scope
        /// </summary>
        /// <param name="gui">The ImGui instance</param>
        public IndentScope(ImGui gui)
        {
            _gui = gui;
            _gui.BeginIndent();
        }

        /// <summary>
        /// Ends the indentation
        /// </summary>
        public void Dispose()
        {
            _gui.EndIndent();
        }
    }

    /// <summary>
    /// A scope for list layout that automatically ends when disposed
    /// </summary>
    public readonly struct ListScope : IDisposable
    {
        private readonly ImGui _gui;

        /// <summary>
        /// Initializes a new list scope with a size
        /// </summary>
        /// <param name="gui">The ImGui instance</param>
        /// <param name="size">The size of the list</param>
        public ListScope(ImGui gui, ImSize size)
        {
            _gui = gui;
            _gui.BeginList(size);
        }

        /// <summary>
        /// Initializes a new list scope with a rectangle
        /// </summary>
        /// <param name="gui">The ImGui instance</param>
        /// <param name="rect">The rectangle defining the list bounds</param>
        public ListScope(ImGui gui, ImRect rect)
        {
            _gui = gui;
            _gui.BeginList(rect);
        }

        /// <summary>
        /// Ends the list layout
        /// </summary>
        public void Dispose()
        {
            _gui.EndList();
        }
    }

    /// <summary>
    /// A scope for scrollable layout that automatically ends when disposed
    /// </summary>
    public readonly struct ScrollableScope : IDisposable
    {
        private readonly ImGui _gui;

        /// <summary>
        /// Initializes a new scrollable scope
        /// </summary>
        /// <param name="gui">The ImGui instance</param>
        public ScrollableScope(ImGui gui)
        {
            _gui = gui;
            _gui.BeginScrollable();
        }

        /// <summary>
        /// Ends the scrollable layout
        /// </summary>
        public void Dispose()
        {
            _gui.EndScrollable();
        }
    }
    
    /// <summary>
    /// Begins a horizontal layout scope with the specified size
    /// </summary>
    /// <param name="gui">The ImGui instance</param>
    /// <param name="size">The size of the horizontal layout</param>
    /// <returns>A disposable scope that ends the horizontal layout when disposed</returns>
    public static IDisposable Horizontal(this ImGui gui, Vector2 size)
    {
        return new HorizontalScope(gui, size);
    }

    /// <summary>
    /// Begins a horizontal layout scope with the specified width and height
    /// </summary>
    /// <param name="gui">The ImGui instance</param>
    /// <param name="width">The width of the horizontal layout (0 for auto)</param>
    /// <param name="height">The height of the horizontal layout (0 for auto)</param>
    /// <returns>A disposable scope that ends the horizontal layout when disposed</returns>
    public static IDisposable Horizontal(this ImGui gui, float width = 0, float height = 0)
    {
        return new HorizontalScope(gui, new Vector2(width, height));
    }

    /// <summary>
    /// Begins a vertical layout scope with the specified size
    /// </summary>
    /// <param name="gui">The ImGui instance</param>
    /// <param name="size">The size of the vertical layout</param>
    /// <returns>A disposable scope that ends the vertical layout when disposed</returns>
    public static IDisposable Vertical(this ImGui gui, Vector2 size)
    {
        return new VerticalScope(gui, size);
    }

    /// <summary>
    /// Begins a vertical layout scope with the specified width and height
    /// </summary>
    /// <param name="gui">The ImGui instance</param>
    /// <param name="width">The width of the vertical layout (0 for auto)</param>
    /// <param name="height">The height of the vertical layout (0 for auto)</param>
    /// <returns>A disposable scope that ends the vertical layout when disposed</returns>
    public static IDisposable Vertical(this ImGui gui, float width = 0, float height = 0)
    {
        return new VerticalScope(gui, new Vector2(width, height));
    }

    /// <summary>
    /// Begins an indentation scope
    /// </summary>
    /// <param name="gui">The ImGui instance</param>
    /// <returns>A disposable scope that ends the indentation when disposed</returns>
    public static IDisposable Indent(this ImGui gui)
    {
        return new IndentScope(gui);
    }

    /// <summary>
    /// Begins a list layout scope with the specified rectangle
    /// </summary>
    /// <param name="gui">The ImGui instance</param>
    /// <param name="rect">The rectangle defining the list bounds</param>
    /// <returns>A disposable scope that ends the list layout when disposed</returns>
    public static IDisposable List(this ImGui gui, ImRect rect)
    {
        return new ListScope(gui, rect);
    }

    /// <summary>
    /// Begins a list layout scope with the specified size
    /// </summary>
    /// <param name="gui">The ImGui instance</param>
    /// <param name="size">The size of the list (default for auto)</param>
    /// <returns>A disposable scope that ends the list layout when disposed</returns>
    public static IDisposable List(this ImGui gui, ImSize size = default)
    {
        return new ListScope(gui, size);
    }

    /// <summary>
    /// Begins a scrollable layout scope
    /// </summary>
    /// <param name="gui">The ImGui instance</param>
    /// <returns>A disposable scope that ends the scrollable layout when disposed</returns>
    public static IDisposable Scrollable(this ImGui gui)
    {
        return new ScrollableScope(gui);
    }
}