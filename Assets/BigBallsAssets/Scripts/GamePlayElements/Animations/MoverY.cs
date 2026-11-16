using System.Collections;
using BigBalls.GameplayObjects;
using DG.Tweening;
using UnityEngine;

public class MoverY : MonoBehaviour
{
    [Header("Movement")]

    [SerializeField] private float _upPoint = -4f;
    [SerializeField] private float _distance = 4f;
    [SerializeField] private float _moveDuration = 0.5f;
    [SerializeField] private float _interval = 3f;
    [SerializeField] private Ease _ease = Ease.Linear;
    [SerializeField] private ParticleObject _particle;

    [Header("Startup")]
    [Tooltip("— какой фазы начинать: true Ч сразу подниматьс€, false Ч сначала пауза снизу")]
    [SerializeField] private bool _startGoingUp = true;

    private Enemy _enemy;
    private Stat _health;
    private Tween _tween;
    private Coroutine _loopRoutine;
    private WaitForSeconds _wait;

    private float _baseY;
    private bool _goingUp;

    private void Awake ()
    {
        _baseY = transform.position.y;
        _enemy = GetComponent<Enemy>();
    }

    private void OnEnable ()
    {
        _wait = new WaitForSeconds(_interval);

        if (_enemy.StatHolder != null && _enemy.StatHolder.TryGetStat(out Stat health, StatType.Health))
        {
            _health = health;
        }
        _goingUp = _startGoingUp;
        _baseY = transform.position.y;
        _loopRoutine = StartCoroutine(MoveRoutine());
    }

    private void OnDisable ()
    {
        if (_loopRoutine != null)
        {
            StopCoroutine(_loopRoutine);
            _loopRoutine = null;
        }

        _tween?.Kill();
        _tween = null;
    }

    private IEnumerator MoveRoutine ()
    {
        while (_upPoint < transform.position.z || _goingUp == false)
        {
            yield return _wait;

            float targetY = _goingUp ? _baseY - _distance : _baseY;
            bool done = false;
            _health?.SetImmune(_goingUp == false);

            _particle?.Play(_moveDuration);

            _tween = transform
                .DOMoveY(targetY, _moveDuration)
                .SetEase(_ease)
                .SetUpdate(UpdateType.Normal)
                .OnComplete(() => done = true);

            yield return new WaitUntil(() => done);

            _goingUp = !_goingUp;
        }
    }
}