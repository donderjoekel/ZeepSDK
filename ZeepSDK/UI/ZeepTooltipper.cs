using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace ZeepSDK.UI;

internal class ZeepTooltipper : ZeepGUIBehaviour
{
    private static ZeepTooltipper _instance;

    public static void StartTooltip(VisualElement visualElement, Vector2 mousePosition)
    {
        _instance.SetTooltip(visualElement, mousePosition);
    }

    public static void UpdateTooltip(VisualElement visualElement, Vector2 mousePosition)
    {
        _instance.SetTooltip(visualElement, mousePosition);
    }

    public static void StopTooltip(VisualElement visualElement)
    {
        if (_instance._activeTarget != null && _instance._activeTarget != visualElement)
        {
            return;
        }

        _instance._activeTarget = null;
        _instance._container.style.display = DisplayStyle.None;
    }

    protected override void OnAwake()
    {
        base.OnAwake();
        _instance = this;
        UIDocument.sortingOrder = int.MaxValue;
    }

    protected override bool BlocksInput()
    {
        return false;
    }

    private VisualElement _activeTarget;
    private VisualElement _container;
    private Label _label;

    protected override void BuildUi()
    {
        using (ZeepGUI.Container())
        {
            _container = ZeepGUI.GetLastElement();
            _container.AddToClassList("tooltip");
            _label = ZeepGUI.Label(string.Empty);
        }
    }

    private void SetTooltip(VisualElement visualElement, Vector2 mousePosition)
    {
        _activeTarget = visualElement;
        _container.style.top = mousePosition.y + 20;
        _container.style.left = mousePosition.x + 20;
        _label.text = visualElement.tooltip;
    }

    private float _timer;

    private void Update()
    {
        if (_activeTarget == null)
        {
            _timer = 0.2f;
        }
        else if (_timer > 0)
        {
            _timer -= Time.deltaTime;
        }
        else if (_container.style.display != DisplayStyle.Flex)
        {
            _container.style.display = DisplayStyle.Flex;
        }
    }
}