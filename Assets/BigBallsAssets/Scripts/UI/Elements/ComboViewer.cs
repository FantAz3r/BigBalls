using DG.Tweening;
using TMPro;
using UnityEngine;
using VContainer;

public class ComboViewer : MonoBehaviour
{
    [SerializeField] private TMP_Text _comboText;
    [SerializeField] private RectTransform _panel;

    [SerializeField] private Gradient _comboGradient;
    [SerializeField] private int _maxComboForGradient = 25;
    [SerializeField] private bool _startGradientFromOne = true;

    [Header("Animation")]
    [SerializeField] private float _popScale = 1.15f;
    [SerializeField] private float _popDuration = 0.15f;

    [Header("Visibility")]
    [Tooltip("Скрывать текст, когда комбо = 0")]
    [SerializeField] private bool _hideWhenZero = true;

    private ScoreCounter _scoreCounter;
    private Vector3 _baseScale;
    private Tween _popTween;

    [Inject]
    public void Construct (ScoreCounter scoreCounter)
    {
        _scoreCounter = scoreCounter;
    }

    private void Start ()
    {
        _scoreCounter.ComboChanged += OnComboChanged;
        _scoreCounter.ComboReseted += OnComboReset;

        if (_comboText != null)
            _baseScale = _comboText.rectTransform.localScale;

        Refresh(_scoreCounter.Combo);
    }

    private void OnDestroy ()
    {
        if (_scoreCounter != null)
        {
            _scoreCounter.ComboChanged -= OnComboChanged;
            _scoreCounter.ComboReseted -= OnComboReset;
        }

        _popTween?.Kill();
    }

    private void OnComboChanged (int combo) => Refresh(combo);
    private void OnComboReset () => Refresh(0);

    private void Refresh (int combo)
    {
        if (_comboText == null)
            return;

        if (_hideWhenZero)
            _panel.SetActive(combo > 0);

        if (combo <= 0)
        {
            _comboText.text = string.Empty;
            return;
        }

        _comboText.text = $"x{combo}";
        _comboText.color = EvaluateColor(combo);

        PlayPop();
    }

    private Color EvaluateColor (int combo)
    {
        int min = _startGradientFromOne ? 1 : 0;
        int max = Mathf.Max(min + 1, _maxComboForGradient);

        float t = Mathf.Clamp01((combo - min) / (float) (max - min));
        return _comboGradient.Evaluate(t);
    }

    private void PlayPop ()
    {
        if (_comboText == null)
            return;

        _popTween?.Kill();

        var tr = _comboText.rectTransform;
        tr.localScale = _baseScale;

        _popTween = tr
            .DOScale(_baseScale * _popScale, _popDuration * 0.5f)
            .SetEase(Ease.OutQuad)
            .SetUpdate(true)
            .OnComplete(() =>
            {
                _popTween = tr.DOScale(_baseScale, _popDuration * 0.5f)
                    .SetEase(Ease.InQuad)
                    .SetUpdate(true);
            });
    }
}