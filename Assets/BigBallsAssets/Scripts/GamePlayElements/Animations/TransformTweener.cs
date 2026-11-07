using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class TransformTweener : MonoBehaviour
{
    [Header("Target")]
    [Tooltip("Целевой Transform. Если пусто — используется свой Transform.")]
    [SerializeField] private Transform _target;

    [Header("Position")]
    [SerializeField] private bool _animatePosition = true;
    [Tooltip("Стартовая позиция (применяется мгновенно при Play, если useStartPosition = true)")]
    [SerializeField] private Vector3 _startPosition;
    [SerializeField] private Vector3 _targetPosition;
    [SerializeField] private AxisMask _positionAxes = AxisMask.All;
    [SerializeField] private bool _useLocalPosition = false;
    [Tooltip("Применять стартовую позицию перед анимацией")]
    [SerializeField] private bool _useStartPosition = true;

    [Header("Rotation (Euler)")]
    [SerializeField] private bool _animateRotation = true;
    [Tooltip("Стартовые эйлеровы углы")]
    [SerializeField] private Vector3 _startEulerAngles;
    [SerializeField] private Vector3 _targetEulerAngles;
    [SerializeField] private AxisMask _rotationAxes = AxisMask.All;
    [SerializeField] private bool _useLocalRotation = true;
    [Tooltip("Применять стартовый поворот перед анимацией")]
    [SerializeField] private bool _useStartRotation = true;

    [Header("Scale")]
    [SerializeField] private bool _animateScale = false;
    [Tooltip("Стартовый масштаб")]
    [SerializeField] private Vector3 _startScale = Vector3.one;
    [SerializeField] private Vector3 _targetScale = Vector3.one;
    [SerializeField] private AxisMask _scaleAxes = AxisMask.All;
    [Tooltip("Применять стартовый масштаб перед анимацией")]
    [SerializeField] private bool _useStartScale = true;

    [Header("Tween Settings")]
    [SerializeField] private float _duration = 1f;
    [SerializeField] private Ease _ease = Ease.OutQuad;
    [SerializeField] private bool _playOnStart = true;
    [SerializeField] private bool _autoKill = true;
    [SerializeField] private bool _useUnscaledTime = false;

    [Header("Events")]
    [SerializeField] private UnityEvent _onComplete;

    private Sequence _sequence;

    public float Duration
    {
        get => _duration;
        set => _duration = value;
    }

    public event Action Completed;

    private Transform Target => _target != null ? _target : transform;

    private void Start ()
    {
        if (_playOnStart) Play();
    }

    private void OnDisable ()
    {
        Kill();
    }

    /// <summary>
    /// Запустить анимацию.
    /// </summary>
    public void Play ()
    {
        Kill();

        Transform tr = Target;
        _sequence = DOTween.Sequence();

        if (_animatePosition)
        {
            Vector3 current = _useLocalPosition ? tr.localPosition : tr.position;
            Vector3 start = _useStartPosition
                ? ApplyMask(current, _startPosition, _positionAxes)
                : current;
            Vector3 end = ApplyMask(start, _targetPosition, _positionAxes);

            if (_useStartPosition)
            {
                if (_useLocalPosition) tr.localPosition = start;
                else tr.position = start;
            }

            Tween posTween = _useLocalPosition
                ? tr.DOLocalMove(end, _duration)
                : tr.DOMove(end, _duration);

            posTween.SetEase(_ease);
            _sequence.Join(posTween);
        }

        if (_animateRotation)
        {
            Vector3 current = _useLocalRotation ? tr.localEulerAngles : tr.eulerAngles;
            Vector3 start = _useStartRotation
                ? ApplyMask(current, _startEulerAngles, _rotationAxes)
                : current;
            Vector3 end = ApplyMask(start, _targetEulerAngles, _rotationAxes);

            if (_useStartRotation)
            {
                if (_useLocalRotation) tr.localEulerAngles = start;
                else tr.eulerAngles = start;
            }

            Tween rotTween = _useLocalRotation
                ? tr.DOLocalRotate(end, _duration)
                : tr.DORotate(end, _duration);

            rotTween.SetEase(_ease);
            _sequence.Join(rotTween);
        }

        if (_animateScale)
        {
            Vector3 current = tr.localScale;
            Vector3 start = _useStartScale
                ? ApplyMask(current, _startScale, _scaleAxes)
                : current;
            Vector3 end = ApplyMask(start, _targetScale, _scaleAxes);

            if (_useStartScale)
            {
                tr.localScale = start;
            }

            Tween scaleTween = tr.DOScale(end, _duration).SetEase(_ease);
            _sequence.Join(scaleTween);
        }

        _sequence
            .SetAutoKill(_autoKill)
            .SetUpdate(_useUnscaledTime)
            .OnComplete(() =>
            {
                Completed?.Invoke();
                _onComplete?.Invoke();
            });

        _sequence.Play();
    }

    public void Kill ()
    {
        if (_sequence != null && _sequence.IsActive())
        {
            _sequence.Kill();
        }
        _sequence = null;
    }

    public void SnapToStart ()
    {
        Kill();
        Transform tr = Target;

        if (_animatePosition && _useStartPosition)
        {
            Vector3 cur = _useLocalPosition ? tr.localPosition : tr.position;
            Vector3 start = ApplyMask(cur, _startPosition, _positionAxes);
            if (_useLocalPosition) tr.localPosition = start;
            else tr.position = start;
        }

        if (_animateRotation && _useStartRotation)
        {
            Vector3 cur = _useLocalRotation ? tr.localEulerAngles : tr.eulerAngles;
            Vector3 start = ApplyMask(cur, _startEulerAngles, _rotationAxes);
            if (_useLocalRotation) tr.localEulerAngles = start;
            else tr.eulerAngles = start;
        }

        if (_animateScale && _useStartScale)
        {
            Vector3 cur = tr.localScale;
            tr.localScale = ApplyMask(cur, _startScale, _scaleAxes);
        }
    }

    public void SnapToTarget ()
    {
        Kill();
        Transform tr = Target;

        if (_animatePosition)
        {
            Vector3 cur = _useLocalPosition ? tr.localPosition : tr.position;
            Vector3 end = ApplyMask(cur, _targetPosition, _positionAxes);
            if (_useLocalPosition) tr.localPosition = end;
            else tr.position = end;
        }

        if (_animateRotation)
        {
            Vector3 cur = _useLocalRotation ? tr.localEulerAngles : tr.eulerAngles;
            Vector3 end = ApplyMask(cur, _targetEulerAngles, _rotationAxes);
            if (_useLocalRotation) tr.localEulerAngles = end;
            else tr.eulerAngles = end;
        }

        if (_animateScale)
        {
            Vector3 cur = tr.localScale;
            tr.localScale = ApplyMask(cur, _targetScale, _scaleAxes);
        }
    }

    public void PlayTo (Vector3? position = null, Vector3? euler = null, Vector3? scale = null, float? newDuration = null)
    {
        if (position.HasValue) _targetPosition = position.Value;
        if (euler.HasValue) _targetEulerAngles = euler.Value;
        if (scale.HasValue) { _targetScale = scale.Value; _animateScale = true; }
        if (newDuration.HasValue) _duration = newDuration.Value;
        Play();
    }

    public void PlayFrom (Vector3? position = null, Vector3? euler = null, Vector3? scale = null)
    {
        if (position.HasValue) { _startPosition = position.Value; _useStartPosition = true; }
        if (euler.HasValue) { _startEulerAngles = euler.Value; _useStartRotation = true; }
        if (scale.HasValue) { _startScale = scale.Value; _useStartScale = true; _animateScale = true; }
        Play();
    }

    [ContextMenu("Capture Current As Start")]
    public void CaptureCurrentAsStart ()
    {
        Transform tr = Target;
        if (_animatePosition)
        {
            _startPosition = _useLocalPosition ? tr.localPosition : tr.position;
            _useStartPosition = true;
        }
        if (_animateRotation)
        {
            _startEulerAngles = _useLocalRotation ? tr.localEulerAngles : tr.eulerAngles;
            _useStartRotation = true;
        }
        if (_animateScale)
        {
            _startScale = tr.localScale;
            _useStartScale = true;
        }
    }

    [ContextMenu("Capture Current As Target")]
    public void CaptureCurrentAsTarget ()
    {
        Transform tr = Target;

        if (_animatePosition)
            _targetPosition = _useLocalPosition ? tr.localPosition : tr.position;

        if (_animateRotation)
            _targetEulerAngles = _useLocalRotation ? tr.localEulerAngles : tr.eulerAngles;

        if (_animateScale)
            _targetScale = tr.localScale;
    }

    private static Vector3 ApplyMask (Vector3 start, Vector3 end, AxisMask mask)
    {
        return new Vector3(
            (mask & AxisMask.X) != 0 ? end.x : start.x,
            (mask & AxisMask.Y) != 0 ? end.y : start.y,
            (mask & AxisMask.Z) != 0 ? end.z : start.z
        );
    }

    [Flags]
    public enum AxisMask
    {
        None = 0,
        X = 1 << 0,
        Y = 1 << 1,
        Z = 1 << 2,
        All = X | Y | Z
    }
}