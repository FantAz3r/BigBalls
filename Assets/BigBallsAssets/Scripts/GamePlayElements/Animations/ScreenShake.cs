using BigBalls.GameplayObjects;
using DG.Tweening;
using UnityEngine;
using VContainer;

public class ScreenShake : MonoBehaviour
{
    [Header("Shot Settings")]
    [SerializeField] private bool _onShot = true;
    [SerializeField] private float _shotIntensity = 0.2f;
    [SerializeField] private float _shotDuration = 0.15f;
    [SerializeField] private int _shotFrequency = 25;

    [Header("Hit Settings")]
    [SerializeField] private bool _onHit = true;
    [SerializeField] private float _hitIntensity = 0.4f;
    [SerializeField] private float _hitDuration = 0.25f;
    [SerializeField] private int _hitFrequency = 30;

    [Header("Kill Settings")]
    [SerializeField] private bool _killShot = true;
    [SerializeField] private float _killIntensity = 0.8f;
    [SerializeField] private float _killDuration = 0.5f;
    [SerializeField] private int _killFrequency = 40;

    [SerializeField] private Transform _camera;
    private int _randomnes;
    private Shooter _shooter;
    private Stat _health;
    private Tween _shakeTween;
    private ICameraProvider _cameraProvider;

    public void Init(Shooter shooter, Stat health)
    {
        _shooter = shooter;
        _health = health;

        _shooter.Shooted += OnShot;
        _health.ValueChanged += OnDamaged;
        _health.ResetedToMiValue += OnDied;
    }

    private void OnEnable()
    {
        if (_shooter != null)
            _shooter.Shooted += OnShot;

        if (_health != null)
        {
            _health.ValueChanged += OnDamaged;
            _health.ResetedToMiValue += OnDied;
        }
    }

    private void OnDisable()
    {
        if (_shooter != null)
            _shooter.Shooted -= OnShot;

        if (_health != null)
        {
            _health.ValueChanged -= OnDamaged;
            _health.ResetedToMiValue -= OnDied;
        }
    }

    private void OnShot()
    {
        if (_onShot)
            Shake(_shotIntensity, _shotDuration, _shotFrequency);
    }

    private void OnDamaged(IReadonlyStat useles)
    {
        if (_onHit)
            Shake(_hitIntensity, _hitDuration, _hitFrequency);
    }

    private void OnDied(IReadonlyStat useles)
    {
        if (_killShot)
            Shake(_killIntensity, _killDuration, _killFrequency);
    }

    private void Shake(float intensity, float duration, int frequency)
    {
        Debug.Log(1234);
        _shakeTween?.Kill();
        _shakeTween = _camera.DOShakePosition(duration, intensity, frequency, _randomnes)
                                      .SetUpdate(true);
    }
}
