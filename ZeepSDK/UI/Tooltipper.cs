using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ZeepSDK.UI;

internal class Tooltipper : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IZeepTooltip
{
    public bool IsOver { get; private set; }
    public string Content { get; private set; }

    public void Initialize(string text)
    {
        Content = text;
    }

    private void OnDestroy()
    {
        UIApi.RemoveTooltip(gameObject);
    }

    private void OnEnable()
    {
        IsOver = false;
    }

    private void OnDisable()
    {
        IsOver = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Tooltipper.OnPointerEnter");
        IsOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Tooltipper.OnPointerExit");
        IsOver = false;
    }
}
