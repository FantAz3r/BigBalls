using BigBalls.GameplayObjects;
using DG.Tweening;
using UnityEngine;

public class SquashOnShot : MonoBehaviour
{
    [SerializeField] private bool _enabled;
    [SerializeField] private Vector3 _squashScale = new Vector3(1.3f, 0.7f, 1.3f);
    [SerializeField] private float returnDuration = 0.12f;
    [SerializeField] private Transform _self;

    private Vector3 _startScale;
    private Shooter _shooter;
    private Tween _currentTween;

    public void Init(Shooter shooter)
    {
        _startScale = _self.localScale;
        _shooter = shooter;
        _shooter.Shooted += OnShooted;
    }

    private void OnEnable()
    {
        if(_shooter != null)
            _shooter.Shooted += OnShooted;
    }

    private void OnDisable()
    {
        if (_shooter != null)
            _shooter.Shooted -= OnShooted;
    }

    private void OnShooted()
    {
        if (enabled == false)
            return;

        if (_currentTween != null)
            _currentTween.Kill();

        _self.localScale = _squashScale;
        _currentTween = _self.DOScale(_startScale, returnDuration);
    }
}
