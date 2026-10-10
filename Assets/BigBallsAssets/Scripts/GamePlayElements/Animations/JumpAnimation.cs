using DG.Tweening;
using UnityEngine;

public class JumpAnimation : MonoBehaviour
{
    [Header("Настройки прыжка")]
    [SerializeField] private float _jumpPower = 1f;
    [SerializeField] private float _duration = 0.5f;
    [SerializeField] private int _jumpsCount = -1;
    [SerializeField] private int _jumpsCountPerCicle = 1;

    [Header("Настройки движения")]
    [SerializeField] private bool _useLocalPosition = true;

    private Vector3 _startPosition;
    private Tween _tween;

    private void OnEnable ()
    {
        _startPosition = _useLocalPosition ? transform.localPosition : transform.position;
        StartJumping();
    }

    private void OnDisable ()
    {
        _tween?.Kill();
    }

    private void StartJumping ()
    {
        _tween = transform.DOJump(
            _startPosition,
            _jumpPower,
            _jumpsCountPerCicle,
            _duration
        )
        .SetLoops(_jumpsCount, LoopType.Restart)
        .SetEase(Ease.OutQuad);
    }
}
