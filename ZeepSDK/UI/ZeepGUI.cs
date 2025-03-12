using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UIElements;
using ZeepSDK.UI.Elements;
using FloatField = ZeepSDK.UI.Elements.FloatField;

namespace ZeepSDK.UI
{
    [PublicAPI]
    public partial class ZeepGUI
    {
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

        public VisualElement Root => _root;

        public VisualElement GetLastElement()
        {
            return _stack.Peek();
        }

        public INerdScope Horizontal(ConfigureElementHandler onConfigureElement = null,
            ConfigureStyleHandler onConfigureStyle = null)
        {
            return new HorizontalScope(this, onConfigureElement, onConfigureStyle);
        }

        public INerdScope Vertical(ConfigureElementHandler onConfigureElement = null,
            ConfigureStyleHandler onConfigureStyle = null)
        {
            return new VerticalScope(this, onConfigureElement, onConfigureStyle);
        }

        public INerdScope Scroll(ScrollValueUpdatedHandler onScrollValueUpdated = null,
            ConfigureElementHandler onConfigureElement = null, ConfigureStyleHandler onConfigureStyle = null)
        {
            return Scroll(Vector2.zero, onScrollValueUpdated, onConfigureElement, onConfigureStyle);
        }

        public INerdScope Scroll(Vector2 scrollOffset, ScrollValueUpdatedHandler onScrollValueUpdated = null,
            ConfigureElementHandler onConfigureElement = null, ConfigureStyleHandler onConfigureStyle = null)
        {
            return new ScrollScope(this, scrollOffset, onScrollValueUpdated, onConfigureElement, onConfigureStyle);
        }

        public INerdScope Container(ConfigureElementHandler onConfigureElement = null,
            ConfigureStyleHandler onConfigureStyle = null)
        {
            return new ContainerScope(this, onConfigureElement, onConfigureStyle);
        }

        public INerdScope Window(string title,
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
