using UnityEngine;
using UnityEngine.UIElements;

namespace ZeepSDK.UI;

internal class TooltipManipulator : Manipulator
{
    public override void RegisterCallbacksOnTarget()
    {
        target.RegisterCallback<MouseOverEvent>(OnMouseOver);
        target.RegisterCallback<MouseMoveEvent>(OnMouseMove);
        target.RegisterCallback<MouseOutEvent>(OnMouseOut);
    }

    public override void UnregisterCallbacksFromTarget()
    {
        target.UnregisterCallback<MouseOverEvent>(OnMouseOver);
        target.UnregisterCallback<MouseMoveEvent>(OnMouseMove);
        target.UnregisterCallback<MouseOutEvent>(OnMouseOut);
    }

    private void OnMouseOver(MouseOverEvent evt)
    {
        ZeepTooltipper.StartTooltip(target, evt.mousePosition);
    }

    private void OnMouseMove(MouseMoveEvent evt)
    {
        ZeepTooltipper.UpdateTooltip(target, evt.mousePosition);
    }

    private void OnMouseOut(MouseOutEvent evt)
    {
        ZeepTooltipper.StopTooltip(target);
    }
}
