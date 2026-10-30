using UnityEngine;
using UnityEngine.UI;

public class ScrollParallax : MonoBehaviour
{
    [SerializeField] private ScrollRect _scrollRect;

    [Tooltip("—корость движени€ относительно Content. 1 Ч как Content, 0.5 Ч медленнее, 1.5 Ч быстрее.")]
    [SerializeField, Min(0f)] private float _parallaxFactor = 0.5f;

    [Header("–азмер относительно Content")]
    [SerializeField, Min(0f)] private float _widthFactor = 1f;
    [SerializeField, Min(0f)] private float _heightFactor = 1f;

    private RectTransform _rectTransform;
    private RectTransform _content;
    private Vector2 _startPosition;
    private Vector2 _startContentPosition;

    private void Awake ()
    {
        _rectTransform = GetComponent<RectTransform>();

        if (_scrollRect == null)
            _scrollRect = GetComponentInParent<ScrollRect>();

        if (_scrollRect == null || _scrollRect.content == null)
        {
            enabled = false;
            return;
        }

        _content = _scrollRect.content;
        _startPosition = _rectTransform.anchoredPosition;
        _startContentPosition = _scrollRect.content.anchoredPosition;

        UpdateSize();
    }

    private void LateUpdate ()
    {
        if (_scrollRect == null || _scrollRect.content == null)
            return;

        Vector2 contentDelta =
            _scrollRect.content.anchoredPosition - _startContentPosition;

        _rectTransform.anchoredPosition =
            _startPosition + contentDelta * _parallaxFactor;

        UpdateSize();
    }

    private void UpdateSize ()
    {
        _rectTransform.SetSizeWithCurrentAnchors(
            RectTransform.Axis.Horizontal,
            _content.rect.width * _widthFactor
        );

        _rectTransform.SetSizeWithCurrentAnchors(
            RectTransform.Axis.Vertical,
            _content.rect.height * _heightFactor
        );
    }
}
