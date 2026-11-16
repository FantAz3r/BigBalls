using BigBalls.GameplayObjects;
using DG.Tweening;
using UnityEngine;

public class ShieldView : MonoBehaviour
{
    [SerializeField] private MeshRenderer _renderer;
    [SerializeField] private float _popScale = 1.15f;
    [SerializeField] private float _popDuration = 0.15f;

    [Header("Damage Flash")]
    [SerializeField] private Color _flashColor = Color.red;
    [SerializeField] private float _flashDuration = 0.15f;
    [Tooltip("Имя свойства цвета в шейдере (Fresnel Color, _BaseColor и т.д.)")]
    [SerializeField] private string _shaderColorProperty = "_Color";

    private Stat _shield;
    private Vector3 _startScale;
    private Tween _scaleTween;
    private Tween _flashTween;
    private MaterialPropertyBlock _propertyBlock;

    private float _lastValue;

    public void Init (Stat shield)
    {
        _shield = shield;
        _shield.ValueChanged += View;

        _startScale = transform.localScale;
        transform.localScale = Vector3.zero;
        _renderer.enabled = false;

        // Инициализируем PropertyBlock
        _propertyBlock = new MaterialPropertyBlock();
        _lastValue = _shield.CurrentValue;

        View(_shield);
    }

    private void OnDisable ()
    {
        if (_shield != null)
        {
            _shield.ValueChanged -= View;
        }

        _scaleTween?.Kill();
        _flashTween?.Kill();

        // Сбрасываем цвет при выключении
        _renderer.SetPropertyBlock(null);
    }

    private void View (IReadonlyStat stat)
    {
        // ★ Проверяем, было ли получено урон (значение уменьшилось)
        if (stat.CurrentValue < _lastValue && stat.CurrentValue > 0)
        {
            FlashRed();
        }
        _lastValue = stat.CurrentValue;

        if (stat.CurrentValue > 0)
        {
            if (_renderer.enabled && transform.localScale != Vector3.zero)
                PopShield();
            else
                ShowShield();
        }
        else
        {
            _scaleTween?.Kill();
            HideShield();
        }
    }

    private void FlashRed ()
    {
        _flashTween?.Kill();

        _renderer.GetPropertyBlock(_propertyBlock);

        _propertyBlock.SetColor(_shaderColorProperty, _flashColor);
        _renderer.SetPropertyBlock(_propertyBlock);

        _flashTween = DOVirtual.DelayedCall(_flashDuration, () =>
        {
            if (_renderer != null)
                _renderer.SetPropertyBlock(null);
        });
    }

    private void ShowShield ()
    {
        _scaleTween?.Kill();
        _renderer.enabled = true;

        _scaleTween = transform.DOScale(_startScale, 0.3f)
            .SetEase(Ease.OutBack);
    }

    private void PopShield ()
    {
        _scaleTween?.Kill();

        Vector3 target = _startScale;
        Vector3 popTarget = _startScale * _popScale;

        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOScale(popTarget, _popDuration).SetEase(Ease.OutQuad));
        seq.Append(transform.DOScale(target, _popDuration).SetEase(Ease.InQuad));
        _scaleTween = seq;
    }

    private void HideShield ()
    {
        _renderer.enabled = true;

        _scaleTween = transform.DOScale(Vector3.zero, 0.2f)
            .SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                _renderer.enabled = false;
            });
    }
}