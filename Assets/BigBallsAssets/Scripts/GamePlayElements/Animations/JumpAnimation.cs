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
        float singleJumpDuration = _duration / Mathf.Max(1, _jumpsCountPerCicle);

        if (_useLocalPosition)
            transform.localPosition = _startPosition;
        else
            transform.position = _startPosition;

        _tween = DOTween.To
        (
            () => 0f,
            t =>
            {
                float offsetY = _jumpPower * Mathf.Sin(t * Mathf.PI);
                Vector3 pos = _startPosition;
                pos.y += offsetY;

                if (_useLocalPosition)
                    transform.localPosition = pos;
                else
                    transform.position = pos;
            },
            1f,
            singleJumpDuration
        )
        .SetEase(Ease.Linear)
        .SetLoops(_jumpsCount * _jumpsCountPerCicle, LoopType.Restart);
    }
}