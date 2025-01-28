using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ZeepSDK.UI;

internal class Tooltip : MonoBehaviour
{
    private const float FadeDuration = 0.05f;
    private const float Opacity = 0.9f;

    private readonly Vector3[] _canvasCorners = new Vector3[4];
    private readonly Vector3[] _tooltipCorners = new Vector3[4];

    private bool _first = true;
    private RectTransform _canvasTransform;
    private RectTransform _transform;
    private TextMeshProUGUI _text;
    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        _transform = GetComponent<RectTransform>();
        _transform.anchorMin = new Vector2(0.5f, 0.5f);
        _transform.anchorMax = new Vector2(0.5f, 0.5f);
        _transform.pivot = new Vector2(0, 1);
        _transform.sizeDelta = new Vector2(200, 80);

        Image image = GetComponent<Image>();
        image.color = new Color(0, 0, 0, Opacity);

        _canvasGroup = GetComponent<CanvasGroup>();
        _canvasGroup.alpha = 0;
        _canvasGroup.blocksRaycasts = false;

        VerticalLayoutGroup verticalLayoutGroup = GetComponent<VerticalLayoutGroup>();
        verticalLayoutGroup.padding = new RectOffset(8, 8, 4, 4);
        verticalLayoutGroup.childForceExpandWidth = true;
        verticalLayoutGroup.childForceExpandHeight = true;
        verticalLayoutGroup.childScaleHeight = false;
        verticalLayoutGroup.childScaleWidth = false;
        verticalLayoutGroup.childControlHeight = true;
        verticalLayoutGroup.childControlWidth = true;

        ContentSizeFitter contentSizeFitter = GetComponent<ContentSizeFitter>();
        contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        GameObject textHolder = new("TooltipText", typeof(RectTransform), typeof(TextMeshProUGUI));
        textHolder.transform.SetParent(transform);
        RectTransform textTransform = textHolder.GetComponent<RectTransform>();
        textTransform.anchorMin = Vector2.zero;
        textTransform.anchorMax = Vector2.one;
        textTransform.anchoredPosition = Vector2.zero;
        textTransform.sizeDelta = new Vector2(-32, -16);

        _text = textHolder.GetComponent<TextMeshProUGUI>();
        _text.horizontalAlignment = HorizontalAlignmentOptions.Center;
        _text.verticalAlignment = VerticalAlignmentOptions.Middle;
        _text.fontSize = 28;
        _text.text = "Hello Tooltip!";
    }

    private void Start()
    {
        _canvasTransform = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        UpdatePosition();
    }

    private void Update()
    {
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        if (_canvasTransform == null)
            return;

        bool success = RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _canvasTransform,
            Input.mousePosition,
            null,
            out Vector2 localPoint);

        _canvasTransform.GetWorldCorners(_canvasCorners);
        _transform.GetWorldCorners(_tooltipCorners);

        if (!success) return;

        Rect canvasRect = new Rect(
            _canvasCorners[0].x,
            _canvasCorners[0].y,
            _canvasCorners[2].x,
            _canvasCorners[2].y);

        localPoint.x += 24 * (1 - _transform.pivot.x);
        localPoint.y -= 24;
        _transform.anchoredPosition = localPoint;

        if (_first)
        {
            _first = false;
            return;
        }

        if (!canvasRect.Contains(_tooltipCorners[0]) || !canvasRect.Contains(_tooltipCorners[2]))
        {
            _transform.pivot = new Vector2(1 - _transform.pivot.x, 1);
        }
    }

    public void Show(string text)
    {
        _first = true;
        _text.text = text;
        _transform.pivot = new Vector2(0, 1);
        StopAllCoroutines();
        StartCoroutine(FadeCanvasGroup(0, Opacity, FadeDuration));
    }

    public void Hide()
    {
        StopAllCoroutines();
        StartCoroutine(FadeCanvasGroup(Opacity, 0, FadeDuration));
    }

    private IEnumerator FadeCanvasGroup(float startAlpha, float endAlpha, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            _canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            yield return null;
        }

        _canvasGroup.alpha = endAlpha;
    }
}
