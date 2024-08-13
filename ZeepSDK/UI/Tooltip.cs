using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ZeepSDK.UI;

internal class Tooltip : MonoBehaviour
{
    private const float FadeDuration = 0.1f;
    private const float Opacity = 0.75f;

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
        image.color = new Color(0, 0, 0, 0.75f);

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

        if (!success) return;

        localPoint.x += 24;
        localPoint.y -= 24;
        _transform.anchoredPosition = localPoint;
    }

    public void Show(string text)
    {
        _text.text = text;
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
            elapsedTime += Time.deltaTime;
            _canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            yield return null;
        }

        _canvasGroup.alpha = endAlpha;
    }
}
