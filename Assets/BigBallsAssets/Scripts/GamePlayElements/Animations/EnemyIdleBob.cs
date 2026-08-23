using DG.Tweening;
using UnityEngine;

public class EnemyIdleBob : MonoBehaviour
{
    public enum ScaleAxis { X, Y, Z }

    [SerializeField] private ScaleAxis _axis = ScaleAxis.Y;
    [SerializeField] private float _scaleMultiplier = 1.1f;
    [SerializeField] private float _duration = 0.4f;
    [SerializeField] private Ease _ease = Ease.InOutSine;

    private Vector3 _initialScale;
    private Tween _tween;

    private void Awake()
    {
        _initialScale = transform.localScale;
    }

    private void OnEnable()
    {
        Play();
    }

    private void OnDisable()
    {
        Stop();
        transform.localScale = _initialScale;
    }

    public void Stop()
    {
        _tween?.Kill();
    }

    public void Play()
    {
        Stop();
        float targetValue = 0;

        switch (_axis)
        {
            case ScaleAxis.X:
                targetValue = _initialScale.x * _scaleMultiplier;
                _tween = transform.DOScaleX(targetValue, _duration);
                break;
            case ScaleAxis.Y:
                targetValue = _initialScale.y * _scaleMultiplier;
                _tween = transform.DOScaleY(targetValue, _duration);
                break;
            case ScaleAxis.Z:
                targetValue = _initialScale.z * _scaleMultiplier;
                _tween = transform.DOScaleZ(targetValue, _duration);
                break;
        }

        _tween?.SetEase(_ease)
               .SetLoops(-1, LoopType.Yoyo)
               .SetUpdate(true)
               .Play();
    }
}