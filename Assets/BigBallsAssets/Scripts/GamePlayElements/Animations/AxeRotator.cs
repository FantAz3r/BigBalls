using DG.Tweening;
using UnityEngine;

public class AxeRotator : MonoBehaviour
{
    public enum RotationAxis
    {
        X,
        Y,
        Z
    }

    [SerializeField] private float _degreesPerSecond = 180f;
    [SerializeField] private bool _useUnscaledTime = false;
    [SerializeField] private bool _rotateLocal = true;
    [SerializeField] private int _direction = 1;
    [SerializeField] private RotationAxis _axis = RotationAxis.Y;

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
        Vector3 rotationDelta = GetAxisVector(_axis) * step;

        if (_rotateLocal)
        {
            _rotationTween = transform.DOLocalRotate(rotationDelta, 1f, RotateMode.LocalAxisAdd)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Incremental);
        }
        else
        {
            _rotationTween = transform.DORotate(rotationDelta, 1f, RotateMode.WorldAxisAdd)
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

    private static Vector3 GetAxisVector (RotationAxis axis)
    {
        return axis switch
        {
            RotationAxis.X => Vector3.right,
            RotationAxis.Y => Vector3.up,
            RotationAxis.Z => Vector3.forward,
            _ => Vector3.up
        };
    }
}