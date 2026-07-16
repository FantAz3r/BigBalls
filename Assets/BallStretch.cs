using DG.Tweening;
using UnityEngine;

public class BallStretch : MonoBehaviour
{
    [SerializeField] private bool _enabled;
    [SerializeField] private Vector3 _scaleMultiplier = new Vector3(1f, 1f, 2f);
    [SerializeField] private float _returnDuration = 0.15f;
    [SerializeField] private Transform _self;

    private Vector3 _baseScale;
    private Tween _tween;

    private void Awake()
    {
        if (_self != null)
            _baseScale = _self.localScale;
    }

    private void OnEnable()
    {
        _tween?.Kill();

        if (_enabled == false)
            return;

        Vector3 spawnScale;
        spawnScale.x = _scaleMultiplier.x * _baseScale.x;
        spawnScale.y = _scaleMultiplier.y * _baseScale.y;
        spawnScale.z = _scaleMultiplier.z * _baseScale.z;

        _self.localScale = spawnScale;
        _tween = _self.DOScale(_baseScale, _returnDuration).SetEase(Ease.OutQuad);
    }
}
