using DG.Tweening;
using UnityEngine;

public class EnemyIdleBob : MonoBehaviour
{
    [SerializeField] private float _scaleMultiplier = 1.1f;
    [SerializeField] private float _duration = 0.4f;
    [SerializeField] private Ease _ease = Ease.InOutSine;

    private Vector3 _initialScale;
    private Tween _tween;

    private void Awake ()
    {
        _initialScale = transform.localScale;
    }

    private void OnEnable ()
    {
        _tween = transform.DOScaleY(_initialScale.y * _scaleMultiplier, _duration)
            .SetEase(_ease)
            .SetLoops(-1, LoopType.Yoyo)
            .SetUpdate(true)
            .Play();
    }

    private void OnDisable ()
    {
        _tween?.Kill();
        transform.localScale = _initialScale;
    }
}