using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class WaveFlag : MonoBehaviour
{
    [SerializeField] private Image _backgroundImage;
    [SerializeField] private float _pulseDuration = 0.6f;
    [SerializeField] private Color _activeColor = Color.yellow;
    [SerializeField] private Color _inactiveColor = Color.white;
    [SerializeField] private Vector2 _shakeStrength = new Vector2(10f, 0f); 
    [SerializeField] private int _shakeVibrato = 10;
    [SerializeField] private float _shakeElasticity = 0.5f;

    private bool _isActivated;
    private Tween _colorTween;
    private Tween _shakeTween;

    private void Awake()
    {
        _isActivated = false;

        if (_colorTween != null && _colorTween.IsActive()) 
            _colorTween.Kill();

        if (_shakeTween != null && _shakeTween.IsActive())
            _shakeTween.Kill();

        if (_backgroundImage != null)
        {
            _backgroundImage.color = _inactiveColor;
        }

        var rt = GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.anchoredPosition = Vector2.zero;
        }
    }

    public void ActivateEffect()
    {
        if (_isActivated)
            return;

        _isActivated = true;

        if (_colorTween != null && _colorTween.IsActive()) 
            _colorTween.Kill();

        if (_shakeTween != null && _shakeTween.IsActive()) 
            _shakeTween.Kill();

        var rectTransform = GetComponent<RectTransform>();

       //if (rectTransform != null)
       //{
       //    _shakeTween = rectTransform.DOPunchAnchorPos(new Vector2(_shakeStrength.x, _shakeStrength.y), _pulseDuration, _shakeVibrato, _shakeElasticity)
       //        .SetEase(Ease.OutQuad)
       //        .OnComplete(() =>
       //        {
       //            rectTransform.anchoredPosition = Vector2.zero;
       //        });
       //}

        if (_backgroundImage != null)
        {
            _colorTween = _backgroundImage.DOColor(_activeColor, _pulseDuration).SetEase(Ease.OutCubic);
        }
    }
}