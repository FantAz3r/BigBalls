using DG.Tweening;
using UnityEngine;

public class BackgroundLooper : MonoBehaviour
{
    [Header("Настройки движения")]
    [SerializeField] private float _startZ = 30f;
    [SerializeField] private float _endZ = -10f;
    [SerializeField] private float _duration = 5f;
    [SerializeField] private bool _useCurrentPosition = true;
    [SerializeField] private int _loopCount = -1;
    [SerializeField] private LoopType _loopType = LoopType.Restart;
    [SerializeField] private Ease _ease = Ease.Linear;

    private Tween _currentTween;
    private Vector3 _startPosition;

    private void OnDestroy ()
    {
        StopMove();
    }

    public void StartMove ()
    {
        AnimateBackground();
    }

    public void StopMove ()
    {
        _currentTween?.Kill();
        _currentTween = null;
    }

    private void AnimateBackground ()
    {
        if (_useCurrentPosition)
        {
            _startPosition = transform.position;
        }
        else
        {
            _startPosition = new Vector3(transform.position.x, transform.position.y, _startZ);
        }

        if (!_useCurrentPosition)
        {
            transform.position = _startPosition;
        }

        _currentTween = transform.DOMoveZ(_endZ, _duration)
            .SetEase(_ease)
            .SetLoops(_loopCount, _loopType)
            .OnComplete(() =>
            {
                if (_loopCount != -1)
                {
                    transform.position = _startPosition;
                }
            });
    }
}