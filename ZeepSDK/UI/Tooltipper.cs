using UnityEngine;
using UnityEngine.EventSystems;

namespace ZeepSDK.UI;

internal class Tooltipper : MonoBehaviour, IPointerEnterHandler, IPointerMoveHandler, IPointerExitHandler
{
    private const float TooltipDelay = 0.5f;

    private string _text;
    private bool _isOver;
    private bool _shownTooltip;
    private float _timeUntilTooltip = 0.5f;

    public void Initialize(string text)
    {
        _text = text;
    }

    private void OnDisable()
    {
        if (_shownTooltip)
        {
            UIApi.HideTooltip();
        }
    }

    private void Update()
    {
        if (!_isOver || _shownTooltip)
            return;

        _timeUntilTooltip -= Time.deltaTime;
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
