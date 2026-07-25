using DG.Tweening;
using UnityEngine;

public class CoinAnimation : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField] private float _rotationSpeed = 360f;
    [SerializeField] private RotateMode _rotateMode = RotateMode.LocalAxisAdd;

    [Header("Float Settings")]
    [SerializeField] private float _floatHeight = 0.3f;
    [SerializeField] private float _floatDuration = 1.5f;
    [SerializeField] private Ease _floatEase = Ease.InOutSine;

    [Header("Optional")]
    [SerializeField] private float _startDelay = 0f;

    private Tweener _rotationTweener;
    private Tweener _floatTweener;
    private Vector3 _startPosition;

    private void OnEnable ()
    {
        _startPosition = transform.localPosition;
        StartAnimation();
    }

    public void StartAnimation ()
    {
        StopAnimation();

        _rotationTweener = transform
            .DORotate(new Vector3(0, _rotationSpeed, 0), 1f, _rotateMode)
            .SetLoops(-1, LoopType.Incremental)
            .SetEase(Ease.Linear)
            .SetDelay(_startDelay);

        _floatTweener = transform
            .DOLocalMoveY(_startPosition.y + _floatHeight, _floatDuration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(_floatEase)
            .SetDelay(_startDelay);
    }

    public void StopAnimation ()
    {
        _rotationTweener?.Kill();
        _floatTweener?.Kill();
        _rotationTweener = null;
        _floatTweener = null;
    }

    private void OnDisable ()
    {
        StopAnimation();
    }
}