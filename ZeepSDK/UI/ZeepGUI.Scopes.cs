using System;
using UnityEngine;
using UnityEngine.UIElements;
using ZeepSDK.UI.Elements;

namespace ZeepSDK.UI
{
    public partial class ZeepGUI
    {
        /// <summary>
        /// A scope that can be used to contain controls
        /// </summary>
        public interface IZeepScope : IDisposable
        {
        }

        internal readonly struct VerticalScope : IZeepScope
        {
            private readonly ZeepGUI _zeepGUI;

            public VerticalScope(ZeepGUI zeepGUI, ConfigureElementHandler onConfigureElement,
                ConfigureStyleHandler onConfigureStyle)
            {
                _zeepGUI = zeepGUI;

                VisualElement element = new()
                {
                    name = "Vertical"
                };
                element.AddToClassList("vertical");
                onConfigureStyle?.Invoke(element.style);
                onConfigureElement?.Invoke(element);
                zeepGUI._stack.Peek().Add(element);
                _zeepGUI.Push(element);
            }

            void IDisposable.Dispose()
            {
                _zeepGUI.Pop();
            }
        }

        internal readonly struct HorizontalScope : IZeepScope
        {
            private readonly ZeepGUI _zeepGUI;

            public HorizontalScope(ZeepGUI zeepGUI, ConfigureElementHandler onConfigureElement,
                ConfigureStyleHandler onConfigureStyle)
            {
                _zeepGUI = zeepGUI;

                VisualElement element = new()
                {
                    name = "Horizontal"
                };
                element.AddToClassList("horizontal");
                onConfigureStyle?.Invoke(element.style);
                onConfigureElement?.Invoke(element);
                zeepGUI._stack.Peek().Add(element);
                _zeepGUI.Push(element);
            }

            void IDisposable.Dispose()
            {
                _zeepGUI.Pop();
            }
        }

        internal readonly struct ScrollScope : IZeepScope
        {
            private readonly ZeepGUI _zeepGUI;

            public ScrollScope(ZeepGUI zeepGUI,
                Vector2 scrollOffset,
                ScrollValueUpdatedHandler onScrollValueUpdated,
                ConfigureElementHandler onConfigureElement,
                ConfigureStyleHandler onConfigureStyle)
            {
                _zeepGUI = zeepGUI;

                ScrollView element = new();
                element.scrollOffset = scrollOffset;
                element.horizontalScroller.valueChanged += h =>
                {
                    onScrollValueUpdated?.Invoke(new Vector2(h, element.scrollOffset.y));
                };
                element.verticalScroller.valueChanged += v =>
                {
                    onScrollValueUpdated?.Invoke(new Vector2(element.scrollOffset.x, v));
                };
                element.AddToClassList("scroll");
                onConfigureStyle?.Invoke(element.style);
                onConfigureElement?.Invoke(element);
                _zeepGUI._stack.Peek().Add(element);
                _zeepGUI.Push(element);
            }

            public void Dispose()
            {
                _zeepGUI.Pop();
            }
        }

        internal readonly struct WindowScope : IZeepScope
        {
            private readonly ZeepGUI _zeepGUI;

            public WindowScope(ZeepGUI zeepGUI,
                string title,
                Rect initialPosition,
                WindowPositionUpdatedHandler onPositionUpdated,
                WindowClosedHandler onClose,
                ConfigureElementHandler onConfigureElement,
                ConfigureStyleHandler onConfigureStyle)
            {
                _zeepGUI = zeepGUI;
                Window window = new(title,
                    initialPosition,
                    onPositionUpdated,
                    onClose,
                    onConfigureElement,
                    onConfigureStyle);
                _zeepGUI._stack.Peek().Add(window);
                _zeepGUI.Push(window);
            }

            public void Dispose()
            {
                _zeepGUI.Pop();
            }
        }

        internal readonly struct ContainerScope : IZeepScope
        {
            private readonly ZeepGUI _zeepGUI;

            public ContainerScope(ZeepGUI zeepGUI,
                ConfigureElementHandler onConfigureElement,
                ConfigureStyleHandler onConfigureStyle)
            {
                _zeepGUI = zeepGUI;

                VisualElement element = new();
                onConfigureStyle?.Invoke(element.style);
                onConfigureElement?.Invoke(element);
                _zeepGUI._stack.Peek().Add(element);
                _zeepGUI.Push(element);
            }

            public void Dispose()
            {
                _zeepGUI.Pop();
            }
        }

        public readonly struct ToolbarScope : IZeepScope
        {
            private readonly ZeepGUI _zeepGUI;

            public ToolbarScope(ZeepGUI zeepGUI)
            {
                _zeepGUI = zeepGUI;

                VisualElement toolbar;

                using (_zeepGUI.Container())
                {
                    using (_zeepGUI.Horizontal(e => e.AddToClassList("toolbar")))
                    {
                        toolbar = _zeepGUI.GetLastElement();
                    }
                }

                _zeepGUI.Push(toolbar);
            }

            public void Dispose()
            {
                _zeepGUI.Pop();
            }
        }
    }
}
