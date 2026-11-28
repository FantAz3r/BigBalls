using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class StarAppearAnimation : MonoBehaviour
{
    [Header("Flight")]
    [SerializeField] private float _flyDuration = 0.45f;
    [SerializeField] private float _startScale = 3.5f;
    [SerializeField] private float _overshootScale = 1.15f;
    [SerializeField] private Vector2 _startOffset = new Vector2(0f, -250f);
    [SerializeField] private Ease _flyEase = Ease.OutCubic;

    [Header("Bounce")]
    [SerializeField] private float _bounceDuration = 0.25f;
    [SerializeField] private float _bounceStrength = 0.18f;
    [SerializeField] private int _bounceVibrato = 6;

    [Header("Fade (optional)")]
    [SerializeField] private bool _fadeIn = true;
    [SerializeField] private float _fadeDuration = 0.2f;

    [Header("Glow / Punch (optional)")]
    [SerializeField] private bool _punchOnLand = true;

    private Image _image;
    private RectTransform _rt;
    private Vector2 _targetPos;
    private Vector3 _targetScale;
    private Sequence _sequence;

    private void Awake ()
    {
        _image = GetComponent<Image>();
        _rt = (RectTransform) transform;
        _targetPos = _rt.anchoredPosition;
        _targetScale = _rt.localScale;
    }

    private void OnEnable ()
    {
        Play();
    }

    public void Play ()
    {
        _sequence?.Kill();

        _rt.anchoredPosition = _targetPos + _startOffset;
        _rt.localScale = _targetScale * _startScale;

        if (_fadeIn)
        {
            var c = _image.color;
            c.a = 0f;
            _image.color = c;
        }

        _sequence = DOTween.Sequence();
        _sequence.Append(_rt.DOAnchorPos(_targetPos, _flyDuration).SetEase(_flyEase));
        _sequence.Join(_rt.DOScale(_targetScale * _overshootScale, _flyDuration).SetEase(_flyEase));

        if (_fadeIn)
            _sequence.Join(_image.DOFade(1f, _fadeDuration).SetEase(Ease.OutQuad));

        _sequence.Append(_rt.DOScale(_targetScale, _bounceDuration)
            .SetEase(Ease.OutBack));

        if (_punchOnLand)
            _sequence.Join(_rt.DOPunchScale(Vector3.one * _bounceStrength, _bounceDuration, _bounceVibrato, 0.5f));

        _sequence.SetUpdate(true);
        _sequence.Play();
    }

    private void OnDisable ()
    {
        _sequence?.Kill();
    }
}