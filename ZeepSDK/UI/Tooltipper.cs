﻿using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ZeepSDK.UI;

internal class Tooltipper : MonoBehaviour, IPointerEnterHandler, IPointerMoveHandler, IPointerExitHandler
{
    private const float TooltipDelay = 0.05f;

    private string _text;
    private bool _isOver;
    private bool _shownTooltip;
    private float _timeUntilTooltip = TooltipDelay;

    public void Initialize(string text)
    {
        _text = text;
    }

    private void OnEnable()
    {
        _isOver = false;
        _shownTooltip = false;
        _timeUntilTooltip = TooltipDelay;
    }

    private void OnDisable()
    {
        if (_shownTooltip)
        {
            UIApi.HideTooltip();
        }
        
        _isOver = false;
        _shownTooltip = false;
        _timeUntilTooltip = TooltipDelay;
    }

    private void Update()
    {
        if (!_isOver || _shownTooltip)
            return;

        _timeUntilTooltip -= Time.unscaledDeltaTime;
        if (_timeUntilTooltip <= 0)
        {
            _shownTooltip = true;
            UIApi.ShowTooltip(_text);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _isOver = true;
        _shownTooltip = false;
        _timeUntilTooltip = TooltipDelay;
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        _timeUntilTooltip = TooltipDelay;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _isOver = false;
        if (_shownTooltip)
        {
            UIApi.HideTooltip();
        }
    }
}
