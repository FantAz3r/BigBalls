using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ArrowPulseAnimation : MonoBehaviour
{
    [Header("Движение")]
    [Tooltip("Насколько далеко стрелка уезжает вперёд вдоль своего текущего направления.")]
    [SerializeField] private float _distance = 40f;

    [Tooltip("Направление 'вперёд' в локальных осях стрелки. " +
             "Для стрелки, смотрящей вправо — (1,0); вверх — (0,1).")]
    [SerializeField] private Vector2 _localForward = Vector2.right;

    [SerializeField] private float _forwardDuration = 0.35f;
    [SerializeField] private float _returnDuration = 0.55f;
    [SerializeField] private float _holdDuration = 0.15f;

    [Header("Кривые")]
    [SerializeField] private Ease _forwardEase = Ease.OutQuad;
    [SerializeField] private Ease _returnEase = Ease.InOutSine;

    [Header("Прочее")]
    [SerializeField] private bool _unscaledTime = true;

    private RectTransform _rect;
    private Vector2 _startPosition;
    private Sequence _sequence;

    private void Awake ()
    {
        _rect = (RectTransform) transform;
        _startPosition = _rect.anchoredPosition;
    }

    private void OnEnable () => Play();

    private void OnDisable () => Kill();

    public void Play ()
    {
        Kill();

        _rect.anchoredPosition = _startPosition;

        Vector2 offset = GetForwardOffset();

        _sequence = DOTween.Sequence();

        if (_unscaledTime)
            _sequence.SetUpdate(true);

        _sequence
            .Append(_rect.DOAnchorPos(_startPosition + offset, _forwardDuration)
                .SetEase(_forwardEase))
            .AppendInterval(_holdDuration)
            .Append(_rect.DOAnchorPos(_startPosition, _returnDuration)
                .SetEase(_returnEase))
            .AppendInterval(_holdDuration)
            .SetLoops(-1, LoopType.Restart);
    }

    public void Stop ()
    {
        Kill();
        _rect.anchoredPosition = _startPosition;
    }

    private void Kill ()
    {
        if (_sequence != null && _sequence.IsActive())
            _sequence.Kill();

        _sequence = null;
    }

    private Vector2 GetForwardOffset ()
    {
        float angleRad = _rect.localEulerAngles.z * Mathf.Deg2Rad;
        float cos = Mathf.Cos(angleRad);
        float sin = Mathf.Sin(angleRad);

        Vector2 dir = new Vector2(
            _localForward.x * cos - _localForward.y * sin,
            _localForward.x * sin + _localForward.y * cos
        );

        if (dir.sqrMagnitude < 0.0001f)
            return Vector2.zero;

        return dir.normalized * _distance;
    }
}