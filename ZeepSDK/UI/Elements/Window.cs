using UnityEngine;
using UnityEngine.UIElements;

namespace ZeepSDK.UI.Elements
{
    internal class Window : VisualElement
    {
        private readonly WindowDragger _windowDragger;
        private readonly VisualElement _content;

        public override VisualElement contentContainer => _content ?? this;

        public Window(string title,
            Rect initialPosition,
            WindowPositionUpdatedHandler onPositionUpdated,
            WindowClosedHandler onClose,
            ConfigureElementHandler onConfigureElement,
            ConfigureStyleHandler onConfigureStyle)
        {
            style.position = Position.Absolute;
            style.left = initialPosition.x;
            style.top = initialPosition.y;
            style.width = initialPosition.width;
            style.height = initialPosition.height;
            
            AddToClassList("window");
            VisualElement bar = new()
            {
                name = "Bar"
            };
            bar.AddToClassList("bar");

            Label label = new(title)
            {
                name = "Title"
            };
            label.AddToClassList("title");
            bar.Add(label);

            Button closeButton = new(() =>
            {
                parent.Remove(this);
                onClose?.Invoke();
            })
            {
                name = "CloseButton",
                text = "X",
            };
            closeButton.AddToClassList("close-button");
            bar.Add(closeButton);
            
            _windowDragger = new WindowDragger(bar);
            Add(bar);

            VisualElement content = new()
            {
                name = "Content"
            };
            content.AddToClassList("content");
            Add(content);
            _content = content;

            RegisterCallback<GeometryChangedEvent, WindowPositionUpdatedHandler>(
                (evt, arg) => arg?.Invoke(evt.newRect),
                onPositionUpdated);

            onConfigureStyle?.Invoke(style);
            onConfigureElement?.Invoke(this);
        }

        private class WindowDragger : DragEventsProcessor
        {
            private float _offsetX;
            private float _offsetY;
            
            public WindowDragger(VisualElement target) : base(target)
            {
            }

            public override bool CanStartDrag(Vector3 pointerPosition)
            {
                return !m_Target.Q<Button>().worldBound.Contains(pointerPosition);
            }

            public override StartDragArgs StartDrag(Vector3 pointerPosition)
            {
                Vector2 offset = (Vector2)pointerPosition - m_Target.worldBound.position;
                _offsetX = offset.x;
                _offsetY = offset.y;
                return new StartDragArgs();
            }

            public override DragVisualMode UpdateDrag(Vector3 pointerPosition)
            {
                m_Target.parent.style.left = pointerPosition.x - _offsetX;
                m_Target.parent.style.top = pointerPosition.y - _offsetY;
                return DragVisualMode.Move;
            }

            public override void OnDrop(Vector3 pointerPosition)
            {
            }

            public override void ClearDragAndDropUI()
            {
            }
        }
    }
}
