using BigBalls.GameplayObjects;
using DG.Tweening;
using UnityEngine;

public class TiltOnShot : MonoBehaviour
{
    [SerializeField] private bool _enabled;
    [SerializeField] private float _kickAngleX = -12f; 
    [SerializeField] private float _returnDuration = 0.15f;
    [SerializeField] private Transform _self;

    private Vector3 _baseRotation;
    private Shooter _shooter;
    private Tween _currentTween;

    private void Awake()
    {
        _baseRotation = _self.localEulerAngles;
    }

    public void Init(Shooter shooter)
    {
        _shooter = shooter;
        _shooter.Shooted += OnShooted;
    }

    private void OnEnable()
    {
        if (_shooter != null)
            _shooter.Shooted += OnShooted;
    }

    private void OnDisable()
    {
        if (_shooter != null)
            _shooter.Shooted -= OnShooted;
    }

    private void OnShooted()
    {
        if (!_enabled)
            return;

        if (_currentTween != null)
            _currentTween.Kill();

        Vector3 targetKickRotation = new Vector3(_baseRotation.x + _kickAngleX, _baseRotation.y, _baseRotation.z);
        _self.localRotation = Quaternion.Euler(targetKickRotation);
        _currentTween = _self.DOLocalRotate(_baseRotation, _returnDuration);
    }
}
