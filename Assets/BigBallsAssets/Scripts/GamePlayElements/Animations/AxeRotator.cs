using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class AxeRotator : MonoBehaviour
{
    [SerializeField] private float _degreesPerSecond = 180f;
    [SerializeField] private bool _useUnscaledTime = false;
    [SerializeField] private bool _rotateLocal = true;
    [SerializeField] private int _direction = 1;

    private Tween _rotationTween;

    private void OnEnable ()
    {
        StartRotation();
    }

    private void OnDisable ()
    {
        StopRotation();
    }

    private void StartRotation ()
    {
        if (_direction == 0)
            _direction = 1;

        if (_rotationTween != null && _rotationTween.IsActive())
        {
            _rotationTween.Kill();
            _rotationTween = null;
        }

        float step = _degreesPerSecond * Mathf.Sign(_direction);

        if (_rotateLocal)
        {
            _rotationTween = transform.DOLocalRotate(new Vector3(0f, step, 0f), 1f, RotateMode.LocalAxisAdd)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Incremental);
        }
        else
        {
            _rotationTween = transform.DORotate(new Vector3(0f, step, 0f), 1f, RotateMode.WorldAxisAdd)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Incremental);
        }

        if (_useUnscaledTime)
            _rotationTween.SetUpdate(true);
    }

    private void StopRotation ()
    {
        if (_rotationTween != null)
        {
            if (_rotationTween.IsActive())
            {
                _rotationTween.Kill();
            }

            _rotationTween = null;
        }
    }
}
