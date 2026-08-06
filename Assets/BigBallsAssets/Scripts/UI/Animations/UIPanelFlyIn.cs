using DG.Tweening;
using UnityEngine;

public class UIPanelFlyIn : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private Vector2 _startPosition;
    [SerializeField] private float _duration = 1;
    [SerializeField] private Ease _easeType = Ease.OutBack;

    [Header("Options")]
    [SerializeField] private bool _playOnEnable = true;

    private Vector2 _targetPosition;
    private RectTransform _rectTransform;

    private void Awake ()
    {
        _rectTransform = GetComponent<RectTransform>();
        _targetPosition = _rectTransform.anchoredPosition;
    }

    private void OnEnable ()
    {
        if (_playOnEnable)
        {
            PlayAnimation();
        }
    }

    public void PlayAnimation ()
    {
        transform.DOKill();
        _rectTransform.anchoredPosition = _startPosition;
        _rectTransform.DOAnchorPos(_targetPosition, _duration)
            .SetEase(_easeType);
    }
}

