using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UIElements;
using ZeepSDK.UI.Elements;
using FloatField = ZeepSDK.UI.Elements.FloatField;

namespace ZeepSDK.UI
{
    /// <summary>
    /// A class that provides all kinds of gui elements
    /// </summary>
    [PublicAPI]
    public partial class ZeepGUI
    {
        /// <summary>
        /// The style to apply to the control
        /// </summary>
        public enum StyleMode
        {
            Regular,
            Primary,
            Secondary,
            Accent,
            Success,
            Warning,
            Danger,
            Info
        }

        private readonly VisualElement _root;
        private readonly Stack<VisualElement> _stack;

        public ZeepGUI(VisualElement root)
        {
            _root = root;

            _stack = new Stack<VisualElement>();
            _stack.Push(_root);
        }

        /// <summary>
        /// The root visual element to add items to
        /// </summary>
        public VisualElement Root => _root;

        /// <summary>
        /// Returns the last element from the stack
        /// </summary>
        /// <returns></returns>
        public VisualElement GetLastElement()
        {
            return _stack.Peek();
        }

        /// <summary>
        /// Creates a scope that enforces horizontal positioning if its children
        /// </summary>
        /// <param name="onConfigureElement"></param>
        /// <param name="onConfigureStyle"></param>
        /// <returns></returns>
        public IZeepScope Horizontal(ConfigureElementHandler onConfigureElement = null,
            ConfigureStyleHandler onConfigureStyle = null)
        {
            return new HorizontalScope(this, onConfigureElement, onConfigureStyle);
        }

        /// <summary>
        /// Creates a scope that enforces vertical positioning if its children
        /// </summary>
        /// <param name="onConfigureElement"></param>
        /// <param name="onConfigureStyle"></param>
        /// <returns></returns>
        public IZeepScope Vertical(ConfigureElementHandler onConfigureElement = null,
            ConfigureStyleHandler onConfigureStyle = null)
        {
            return new VerticalScope(this, onConfigureElement, onConfigureStyle);
        }

        /// <summary>
        /// Creates a scope that wraps its children in a (vertically) scrollable view
        /// </summary>
        /// <param name="onScrollValueUpdated"></param>
        /// <param name="onConfigureElement"></param>
        /// <param name="onConfigureStyle"></param>
        /// <returns></returns>
        public IZeepScope Scroll(ScrollValueUpdatedHandler onScrollValueUpdated = null,
            ConfigureElementHandler onConfigureElement = null, ConfigureStyleHandler onConfigureStyle = null)
        {
            return Scroll(Vector2.zero, onScrollValueUpdated, onConfigureElement, onConfigureStyle);
        }

        /// <summary>
        /// Creates a scope that wraps its children in a (vertically) scrollable view
        /// </summary>
        /// <param name="scrollOffset"></param>
        /// <param name="onScrollValueUpdated"></param>
        /// <param name="onConfigureElement"></param>
        /// <param name="onConfigureStyle"></param>
        /// <returns></returns>
        public IZeepScope Scroll(Vector2 scrollOffset, ScrollValueUpdatedHandler onScrollValueUpdated = null,
            ConfigureElementHandler onConfigureElement = null, ConfigureStyleHandler onConfigureStyle = null)
        {
            return new ScrollScope(this, scrollOffset, onScrollValueUpdated, onConfigureElement, onConfigureStyle);
        }

        /// <summary>
        /// Creates a scope that wraps its children
        /// </summary>
        /// <param name="onConfigureElement"></param>
        /// <param name="onConfigureStyle"></param>
        /// <returns></returns>
        public IZeepScope Container(ConfigureElementHandler onConfigureElement = null,
            ConfigureStyleHandler onConfigureStyle = null)
        {
            return new ContainerScope(this, onConfigureElement, onConfigureStyle);
        }

        /// <summary>
        /// Creates a scope that wraps its children in the style of a window. The recommended approach is to make a class that inherits from <see cref="ZeepGUIWindow"/>
        /// </summary>
        /// <param name="title"></param>
        /// <param name="initialPosition"></param>
        /// <param name="onPositionUpdated"></param>
        /// <param name="onClose"></param>
        /// <param name="onConfigureElement"></param>
        /// <param name="onConfigureStyle"></param>
        /// <returns></returns>
        public IZeepScope Window(string title,
            Rect initialPosition,
            WindowPositionUpdatedHandler onPositionUpdated,
            WindowClosedHandler onClose,
            ConfigureElementHandler onConfigureElement = null,
            ConfigureStyleHandler onConfigureStyle = null)
        {
            return new WindowScope(this,
                title,
                initialPosition,
                onPositionUpdated,
                onClose,
                onConfigureElement,
                onConfigureStyle);
        }

        /// <summary>
        /// Creates a button control
        /// </summary>
        /// <param name="text"></param>
        /// <param name="onClick"></param>
        /// <param name="tooltip"></param>
        /// <param name="styleMode"></param>
        /// <param name="onConfigureElement"></param>
        /// <param name="onConfigureStyle"></param>
        /// <returns></returns>
        public Button Button(string text,
            ButtonClickHandler onClick,
            string tooltip = "",
            StyleMode styleMode = StyleMode.Regular,
            ConfigureElementHandler onConfigureElement = null,
            ConfigureStyleHandler onConfigureStyle = null)
        {
            Button element = new(() => onClick())
            {
                text = text
            };

            if (!string.IsNullOrEmpty(tooltip))
            {
                element.tooltip = tooltip;
                element.AddManipulator(new TooltipManipulator());
            }

            HandleStyle(element, "button", styleMode, onConfigureStyle);
            onConfigureElement?.Invoke(element);
            _stack.Peek().Add(element);
            return element;
        }

        /// <summary>
        /// Creates a label control
        /// </summary>
        /// <param name="label"></param>
        /// <param name="tooltip"></param>
        /// <param name="styleMode"></param>
        /// <param name="onConfigureElement"></param>
        /// <param name="onConfigureStyle"></param>
        /// <returns></returns>
        public Label Label(string label,
            string tooltip = "",
            StyleMode styleMode = StyleMode.Regular,
            ConfigureElementHandler onConfigureElement = null,
            ConfigureStyleHandler onConfigureStyle = null)
        {
            Label element = new(label);

            if (!string.IsNullOrEmpty(tooltip))
            {
                element.tooltip = tooltip;
                element.AddManipulator(new TooltipManipulator());
            }

            HandleStyle(element, "label", styleMode, onConfigureStyle);
            onConfigureElement?.Invoke(element);
            _stack.Peek().Add(element);
            return element;
        }

        /// <summary>
        /// Creates a toggle control
        /// </summary>
        /// <param name="label"></param>
        /// <param name="value"></param>
        /// <param name="onValueUpdated"></param>
        /// <param name="tooltip"></param>
        /// <param name="styleMode"></param>
        /// <param name="onConfigureElement"></param>
        /// <param name="onConfigureStyle"></param>
        /// <returns></returns>
        public Toggle Toggle(string label,
            bool value,
            ToggleValueUpdatedHandler onValueUpdated,
            string tooltip = "",
            StyleMode styleMode = StyleMode.Regular,
            ConfigureElementHandler onConfigureElement = null,
            ConfigureStyleHandler onConfigureStyle = null)
        {
            Toggle element = new(label)
            {
                value = value
            };

            if (!string.IsNullOrEmpty(tooltip))
            {
                element.tooltip = tooltip;
                element.AddManipulator(new TooltipManipulator());
            }

            element.RegisterValueChangedCallback(evt => { onValueUpdated(evt.previousValue, evt.newValue); });
            HandleStyle(element, "toggle", styleMode, onConfigureStyle);
            onConfigureElement?.Invoke(element);
            _stack.Peek().Add(element);
            return element;
        }

        /// <summary>
        /// Creates a textfield control
        /// </summary>
        /// <param name="label"></param>
        /// <param name="text"></param>
        /// <param name="onValueUpdated"></param>
        /// <param name="tooltip"></param>
        /// <param name="styleMode"></param>
        /// <param name="onConfigureElement"></param>
        /// <param name="onConfigureStyle"></param>
        /// <returns></returns>
        public TextField TextField(string label,
            string text,
            TextFieldValueUpdatedHandler onValueUpdated,
            string tooltip = "",
            StyleMode styleMode = StyleMode.Regular,
            ConfigureElementHandler onConfigureElement = null,
            ConfigureStyleHandler onConfigureStyle = null)
        {
            TextField element = new(label)
            {
                multiline = false,
                value = text
            };

            if (!string.IsNullOrEmpty(tooltip))
            {
                element.tooltip = tooltip;
                element.AddManipulator(new TooltipManipulator());
            }

            element.RegisterValueChangedCallback(evt => { onValueUpdated(evt.previousValue, evt.newValue); });
            HandleStyle(element, "textfield", styleMode, onConfigureStyle);
            onConfigureElement?.Invoke(element);
            _stack.Peek().Add(element);
            return element;
        }

        /// <summary>
        /// Creates an intfield control
        /// </summary>
        /// <param name="label"></param>
        /// <param name="value"></param>
        /// <param name="onValueUpdated"></param>
        /// <param name="tooltip"></param>
        /// <param name="styleMode"></param>
        /// <param name="onConfigureElement"></param>
        /// <param name="onConfigureStyle"></param>
        /// <returns></returns>
        public IntField IntField(string label,
            int value,
            IntFieldValueUpdatedHandler onValueUpdated,
            string tooltip = "",
            StyleMode styleMode = StyleMode.Regular,
            ConfigureElementHandler onConfigureElement = null,
            ConfigureStyleHandler onConfigureStyle = null)
        {
            IntField element = new(label)
            {
                value = value
            };

            if (!string.IsNullOrEmpty(tooltip))
            {
                element.tooltip = tooltip;
                element.AddManipulator(new TooltipManipulator());
            }

            element.RegisterValueChangedCallback(evt => { onValueUpdated(evt.previousValue, evt.newValue); });
            HandleStyle(element, "textfield", styleMode, onConfigureStyle);
            onConfigureElement?.Invoke(element);
            _stack.Peek().Add(element);
            return element;
        }

        /// <summary>
        /// Creates a floatfield control
        /// </summary>
        /// <param name="label"></param>
        /// <param name="value"></param>
        /// <param name="onValueUpdated"></param>
        /// <param name="tooltip"></param>
        /// <param name="styleMode"></param>
        /// <param name="onConfigureElement"></param>
        /// <param name="onConfigureStyle"></param>
        /// <returns></returns>
        public FloatField FloatField(string label,
            float value,
            FloatFieldValueUpdatedHandler onValueUpdated,
            string tooltip = "",
            StyleMode styleMode = StyleMode.Regular,
            ConfigureElementHandler onConfigureElement = null,
            ConfigureStyleHandler onConfigureStyle = null)
        {
            FloatField element = new(label)
            {
                value = value
            };

            if (!string.IsNullOrEmpty(tooltip))
            {
                element.tooltip = tooltip;
                element.AddManipulator(new TooltipManipulator());
            }

            element.RegisterValueChangedCallback(evt => { onValueUpdated(evt.previousValue, evt.newValue); });
            HandleStyle(element, "textfield", styleMode, onConfigureStyle);
            onConfigureElement?.Invoke(element);
            _stack.Peek().Add(element);
            return element;
        }

        /// <summary>
        /// Creates a doublefield control
        /// </summary>
        /// <param name="label"></param>
        /// <param name="value"></param>
        /// <param name="onValueUpdated"></param>
        /// <param name="tooltip"></param>
        /// <param name="styleMode"></param>
        /// <param name="onConfigureElement"></param>
        /// <param name="onConfigureStyle"></param>
        /// <returns></returns>
        public DoubleField DoubleField(string label,
            double value,
            DoubleFieldValueUpdatedHandler onValueUpdated,
            string tooltip = "",
            StyleMode styleMode = StyleMode.Regular,
            ConfigureElementHandler onConfigureElement = null,
            ConfigureStyleHandler onConfigureStyle = null)
        {
            DoubleField element = new(label)
            {
                value = value
            };

            if (!string.IsNullOrEmpty(tooltip))
            {
                element.tooltip = tooltip;
                element.AddManipulator(new TooltipManipulator());
            }

            element.RegisterValueChangedCallback(evt => { onValueUpdated(evt.previousValue, evt.newValue); });
            HandleStyle(element, "textfield", styleMode, onConfigureStyle);
            onConfigureElement?.Invoke(element);
            _stack.Peek().Add(element);
            return element;
        }

        /// <summary>
        /// Creates a generic typed dropdown control
        /// </summary>
        /// <param name="label"></param>
        /// <param name="choices"></param>
        /// <param name="initialChoice"></param>
        /// <param name="onFormat"></param>
        /// <param name="onValueUpdated"></param>
        /// <param name="tooltip"></param>
        /// <param name="styleMode"></param>
        /// <param name="onConfigureElement"></param>
        /// <param name="onConfigureStyle"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public CustomDropdownField<T> Dropdown<T>(string label,
            IEnumerable<T> choices,
            T initialChoice,
            DropDownFormatHandler<T> onFormat,
            DropDownValueUpdatedHandler<T> onValueUpdated,
            string tooltip = "",
            StyleMode styleMode = StyleMode.Regular,
            ConfigureElementHandler onConfigureElement = null,
            ConfigureStyleHandler onConfigureStyle = null)
        {
            CustomDropdownField<T> element = new(label, choices, initialChoice, onFormat, styleMode);

            if (!string.IsNullOrEmpty(tooltip))
            {
                element.tooltip = tooltip;
                element.AddManipulator(new TooltipManipulator());
            }

            element.RegisterCallback<ChangeEvent<T>, DropDownValueUpdatedHandler<T>>(
                (evt, args) => args.Invoke(evt.previousValue, evt.newValue), onValueUpdated);

            HandleStyle(element, "dropdown", styleMode, onConfigureStyle);
            onConfigureElement?.Invoke(element);
            _stack.Peek().Add(element);
            return element;
        }

        /// <summary>
        /// Creates a button that when clicked will show a dropdown menu
        /// </summary>
        /// <param name="label"></param>
        /// <param name="onBuildDropdownMenu"></param>
        /// <param name="tooltip"></param>
        /// <param name="styleMode"></param>
        /// <param name="onConfigureElement"></param>
        /// <param name="onConfigureStyle"></param>
        /// <returns></returns>
        public Button DropdownButton(string label,
            BuildDropdownMenuHandler onBuildDropdownMenu,
            string tooltip = "",
            StyleMode styleMode = StyleMode.Regular,
            ConfigureElementHandler onConfigureElement = null,
            ConfigureStyleHandler onConfigureStyle = null)
        {
            Button element = new()
            {
                text = label
            };

            if (!string.IsNullOrEmpty(tooltip))
            {
                element.tooltip = tooltip;
                element.AddManipulator(new TooltipManipulator());
            }

            element.clicked += () =>
            {
                GenericDropdownMenu genericDropdownMenu = new();
                genericDropdownMenu.menuContainer.AddToClassList("dropdown-menu");
                genericDropdownMenu.menuContainer.AddToClassList(styleMode.ToString().ToLower());
                // DropdownMenuWrapper wrapper = new(genericDropdownMenu);
                onBuildDropdownMenu(genericDropdownMenu);
                genericDropdownMenu.DropDown(element.worldBound, element);
            };
            HandleStyle(element, "button", styleMode, onConfigureStyle);
            onConfigureElement?.Invoke(element);
            _stack.Peek().Add(element);
            return element;
        }

        private void HandleStyle(VisualElement element,
            string baseClass,
            StyleMode styleMode,
            ConfigureStyleHandler onConfigureStyle)
        {
            element.AddToClassList(baseClass);
            element.AddToClassList(styleMode.ToString().ToLower());
            onConfigureStyle?.Invoke(element.style);
        }

        private void Push(VisualElement element)
        {
            if (_stack.TryPeek(out VisualElement peekedElement) && peekedElement == element)
                throw new Exception("Cannot push the same element more than once in a row");

            _stack.Push(element);
        }

        private void Pop()
        {
            if (!_stack.TryPeek(out VisualElement element))
                throw new Exception("No more items");

            if (element == _root)
                throw new Exception("Cannot pop root");

            _stack.Pop();
        }
    }
}
