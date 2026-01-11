using System;
using Imui.Core;
using JetBrains.Annotations;

namespace Imui.Controls;

public static class ImGuiWindowExtensions
{
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