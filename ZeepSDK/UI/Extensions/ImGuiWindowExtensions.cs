using System;
using Imui.Core;
using JetBrains.Annotations;

namespace Imui.Controls;

public static class ImGuiWindowExtensions
{
    /// <summary>
    /// Begins a window with the specified title, position, and size, and invokes callbacks when the window is opened or closed.
    /// </summary>
    /// <param name="gui">The ImGui instance to use.</param>
    /// <param name="title">The title of the window.</param>
    /// <param name="open">Reference to a boolean that controls whether the window is open.</param>
    /// <param name="rect">The position and size of the window.</param>
    /// <param name="windowOpened">Optional callback invoked when the window is opened.</param>
    /// <param name="windowClosed">Optional callback invoked when the window is closed.</param>
    /// <param name="flags">Window flags to apply to the window.</param>
    /// <returns>True if the window is open and should be rendered, false otherwise.</returns>
    public static bool BeginWindow(this ImGui gui,
        string title,
        ref bool open,
        ImRect rect,
        [CanBeNull] Action windowOpened,
        [CanBeNull] Action windowClosed,
        ImWindowFlag flags = ImWindowFlag.None)
    {
        bool previousOpen = GetWasWindowOpen(gui, title);
        bool currentOpen = gui.BeginWindow(title, ref open, rect, flags);

        if (currentOpen != previousOpen)
        {
            if (currentOpen)
            {
                windowOpened?.Invoke();
            }
            else
            {
                windowClosed?.Invoke();
            }
        }

        return currentOpen;
    }

    /// <summary>
    /// Begins a window with the specified title and size, and invokes callbacks when the window is opened or closed.
    /// </summary>
    /// <param name="gui">The ImGui instance to use.</param>
    /// <param name="title">The title of the window.</param>
    /// <param name="open">Reference to a boolean that controls whether the window is open.</param>
    /// <param name="windowOpened">Optional callback invoked when the window is opened.</param>
    /// <param name="windowClosed">Optional callback invoked when the window is closed.</param>
    /// <param name="size">The size of the window. If default, uses auto-sizing.</param>
    /// <param name="flags">Window flags to apply to the window.</param>
    /// <returns>True if the window is open and should be rendered, false otherwise.</returns>
    public static bool BeginWindow(this ImGui gui,
        string title,
        ref bool open,
        [CanBeNull] Action windowOpened,
        [CanBeNull] Action windowClosed,
        ImSize size = default(ImSize),
        ImWindowFlag flags = ImWindowFlag.None)
    {
        bool previousOpen = GetWasWindowOpen(gui, title);
        bool currentOpen = gui.BeginWindow(title, ref open, size, flags);

        if (currentOpen != previousOpen)
        {
            if (currentOpen)
            {
                windowOpened?.Invoke();
            }
            else
            {
                windowClosed?.Invoke();
            }
        }

        return currentOpen;
    }

    /// <summary>
    /// Begins a window with the specified title, position, and size, and tracks whether the mouse is within the window bounds.
    /// </summary>
    /// <param name="gui">The ImGui instance to use.</param>
    /// <param name="title">The title of the window.</param>
    /// <param name="open">Reference to a boolean that controls whether the window is open.</param>
    /// <param name="mouse">Reference to a boolean that indicates whether the mouse is within the window bounds.</param>
    /// <param name="rect">The position and size of the window.</param>
    /// <param name="flags">Window flags to apply to the window.</param>
    /// <returns>True if the window is open and should be rendered, false otherwise.</returns>
    public static bool BeginWindow(this ImGui gui,
        string title,
        ref bool open,
        ref bool mouse,
        ImRect rect,
        ImWindowFlag flags = ImWindowFlag.None)
    {
        bool currentOpen = gui.BeginWindow(title, ref open, rect, flags);

        if (currentOpen)
        {
            ImRect fullRect = gui.WindowManager.GetWindowState(gui.PeekId()).Rect;
            mouse = fullRect.Contains(gui.Input.MousePosition);
        }
        else if (mouse)
        {
            mouse = false;
        }

        return currentOpen;
    }

    /// <summary>
    /// Begins a window with the specified title and size, and tracks whether the mouse is within the window bounds.
    /// </summary>
    /// <param name="gui">The ImGui instance to use.</param>
    /// <param name="title">The title of the window.</param>
    /// <param name="open">Reference to a boolean that controls whether the window is open.</param>
    /// <param name="mouse">Reference to a boolean that indicates whether the mouse is within the window bounds.</param>
    /// <param name="size">The size of the window. If default, uses auto-sizing.</param>
    /// <param name="flags">Window flags to apply to the window.</param>
    /// <returns>True if the window is open and should be rendered, false otherwise.</returns>
    public static bool BeginWindow(this ImGui gui,
        string title,
        ref bool open,
        ref bool mouse,
        ImSize size = default(ImSize),
        ImWindowFlag flags = ImWindowFlag.None)
    {
        bool currentOpen = gui.BeginWindow(title, ref open, size, flags);

        if (currentOpen)
        {
            ImRect fullRect = gui.WindowManager.GetWindowState(gui.PeekId()).Rect;
            mouse = fullRect.Contains(gui.Input.MousePosition);
        }
        else if (mouse)
        {
            mouse = false;
        }

        return currentOpen;
    }

    /// <summary>
    /// Begins a window with the specified title, position, and size, tracks mouse position, and invokes callbacks when the mouse enters or exits the window.
    /// </summary>
    /// <param name="gui">The ImGui instance to use.</param>
    /// <param name="title">The title of the window.</param>
    /// <param name="open">Reference to a boolean that controls whether the window is open.</param>
    /// <param name="mouse">Reference to a boolean that indicates whether the mouse is within the window bounds.</param>
    /// <param name="rect">The position and size of the window.</param>
    /// <param name="mouseEntered">Optional callback invoked when the mouse enters the window bounds.</param>
    /// <param name="mouseExited">Optional callback invoked when the mouse exits the window bounds.</param>
    /// <param name="flags">Window flags to apply to the window.</param>
    /// <returns>True if the window is open and should be rendered, false otherwise.</returns>
    public static bool BeginWindow(this ImGui gui,
        string title,
        ref bool open,
        ref bool mouse,
        ImRect rect,
        [CanBeNull] Action mouseEntered,
        [CanBeNull] Action mouseExited,
        ImWindowFlag flags = ImWindowFlag.None)
    {
        bool currentOpen = gui.BeginWindow(title, ref open, rect, flags);

        if (currentOpen)
        {
            ImRect fullRect = gui.WindowManager.GetWindowState(gui.PeekId()).Rect;
            bool previousMouse = mouse;
            bool currentMouse = fullRect.Contains(gui.Input.MousePosition);

            if (currentMouse != previousMouse)
            {
                if (currentMouse)
                {
                    mouseEntered?.Invoke();
                }
                else
                {
                    mouseExited?.Invoke();
                }
            }

            mouse = currentMouse;
        }
        else if (mouse)
        {
            mouseExited?.Invoke();
            mouse = false;
        }

        return currentOpen;
    }

    /// <summary>
    /// Begins a window with the specified title and size, tracks mouse position, and invokes callbacks when the mouse enters or exits the window.
    /// </summary>
    /// <param name="gui">The ImGui instance to use.</param>
    /// <param name="title">The title of the window.</param>
    /// <param name="open">Reference to a boolean that controls whether the window is open.</param>
    /// <param name="mouse">Reference to a boolean that indicates whether the mouse is within the window bounds.</param>
    /// <param name="mouseEntered">Optional callback invoked when the mouse enters the window bounds.</param>
    /// <param name="mouseExited">Optional callback invoked when the mouse exits the window bounds.</param>
    /// <param name="size">The size of the window. If default, uses auto-sizing.</param>
    /// <param name="flags">Window flags to apply to the window.</param>
    /// <returns>True if the window is open and should be rendered, false otherwise.</returns>
    public static bool BeginWindow(this ImGui gui,
        string title,
        ref bool open,
        ref bool mouse,
        [CanBeNull] Action mouseEntered,
        [CanBeNull] Action mouseExited,
        ImSize size = default(ImSize),
        ImWindowFlag flags = ImWindowFlag.None)
    {
        bool currentOpen = gui.BeginWindow(title, ref open, size, flags);

        if (currentOpen)
        {
            ImRect fullRect = gui.WindowManager.GetWindowState(gui.PeekId()).Rect;
            bool previousMouse = mouse;
            bool currentMouse = fullRect.Contains(gui.Input.MousePosition);

            if (currentMouse != previousMouse)
            {
                if (currentMouse)
                {
                    mouseEntered?.Invoke();
                }
                else
                {
                    mouseExited?.Invoke();
                }
            }

            mouse = currentMouse;
        }
        else if (mouse)
        {
            mouseExited?.Invoke();
            mouse = false;
        }

        return currentOpen;
    }

    /// <summary>
    /// Begins a window with the specified title, position, and size, tracks window open/close state and mouse position, and invokes callbacks for all state changes.
    /// </summary>
    /// <param name="gui">The ImGui instance to use.</param>
    /// <param name="title">The title of the window.</param>
    /// <param name="open">Reference to a boolean that controls whether the window is open.</param>
    /// <param name="mouse">Reference to a boolean that indicates whether the mouse is within the window bounds.</param>
    /// <param name="rect">The position and size of the window.</param>
    /// <param name="windowOpened">Optional callback invoked when the window is opened.</param>
    /// <param name="windowClosed">Optional callback invoked when the window is closed.</param>
    /// <param name="mouseEntered">Optional callback invoked when the mouse enters the window bounds.</param>
    /// <param name="mouseExited">Optional callback invoked when the mouse exits the window bounds.</param>
    /// <param name="flags">Window flags to apply to the window.</param>
    /// <returns>True if the window is open and should be rendered, false otherwise.</returns>
    public static bool BeginWindow(this ImGui gui,
        string title,
        ref bool open,
        ref bool mouse,
        ImRect rect,
        [CanBeNull] Action windowOpened,
        [CanBeNull] Action windowClosed,
        [CanBeNull] Action mouseEntered,
        [CanBeNull] Action mouseExited,
        ImWindowFlag flags = ImWindowFlag.None)
    {
        bool previousOpen = GetWasWindowOpen(gui, title);
        bool currentOpen = gui.BeginWindow(title, ref open, rect, flags);

        if (currentOpen != previousOpen)
        {
            if (currentOpen)
            {
                windowOpened?.Invoke();
            }
            else
            {
                windowClosed?.Invoke();
            }
        }

        if (currentOpen)
        {
            ImRect fullRect = gui.WindowManager.GetWindowState(gui.PeekId()).Rect;
            bool previousMouse = mouse;
            bool currentMouse = fullRect.Contains(gui.Input.MousePosition);

            if (currentMouse != previousMouse)
            {
                if (currentMouse)
                {
                    mouseEntered?.Invoke();
                }
                else
                {
                    mouseExited?.Invoke();
                }
            }

            mouse = currentMouse;
        }
        else if (mouse)
        {
            mouseExited?.Invoke();
            mouse = false;
        }

        return currentOpen;
    }

    /// <summary>
    /// Begins a window with the specified title and size, tracks window open/close state and mouse position, and invokes callbacks for all state changes.
    /// </summary>
    /// <param name="gui">The ImGui instance to use.</param>
    /// <param name="title">The title of the window.</param>
    /// <param name="open">Reference to a boolean that controls whether the window is open.</param>
    /// <param name="mouse">Reference to a boolean that indicates whether the mouse is within the window bounds.</param>
    /// <param name="windowOpened">Optional callback invoked when the window is opened.</param>
    /// <param name="windowClosed">Optional callback invoked when the window is closed.</param>
    /// <param name="mouseEntered">Optional callback invoked when the mouse enters the window bounds.</param>
    /// <param name="mouseExited">Optional callback invoked when the mouse exits the window bounds.</param>
    /// <param name="size">The size of the window. If default, uses auto-sizing.</param>
    /// <param name="flags">Window flags to apply to the window.</param>
    /// <returns>True if the window is open and should be rendered, false otherwise.</returns>
    public static bool BeginWindow(this ImGui gui,
        string title,
        ref bool open,
        ref bool mouse,
        [CanBeNull] Action windowOpened,
        [CanBeNull] Action windowClosed,
        [CanBeNull] Action mouseEntered,
        [CanBeNull] Action mouseExited,
        ImSize size = default(ImSize),
        ImWindowFlag flags = ImWindowFlag.None)
    {
        bool previousOpen = GetWasWindowOpen(gui, title);
        bool currentOpen = gui.BeginWindow(title, ref open, size, flags);

        if (currentOpen != previousOpen)
        {
            if (currentOpen)
            {
                windowOpened?.Invoke();
            }
            else
            {
                windowClosed?.Invoke();
            }
        }

        if (currentOpen)
        {
            ImRect fullRect = gui.WindowManager.GetWindowState(gui.PeekId()).Rect;
            bool previousMouse = mouse;
            bool currentMouse = fullRect.Contains(gui.Input.MousePosition);

            if (currentMouse != previousMouse)
            {
                if (currentMouse)
                {
                    mouseEntered?.Invoke();
                }
                else
                {
                    mouseExited?.Invoke();
                }
            }

            mouse = currentMouse;
        }
        else if (mouse)
        {
            mouseExited?.Invoke();
            mouse = false;
        }

        return currentOpen;
    }

    private static bool GetWasWindowOpen(ImGui gui, string title)
    {
        uint windowControlId = gui.GetControlId(title);
        int windowId = gui.WindowManager.TryFindWindow(windowControlId);
        if (windowId < 0) return false;
        ImWindowState windowState = gui.WindowManager.GetWindowState(windowControlId);
        return windowState.Visible;
    }
}