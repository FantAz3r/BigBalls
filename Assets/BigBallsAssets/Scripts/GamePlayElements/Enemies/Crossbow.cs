using BigBalls.Factories;
using BigBalls.GameplayObjects;
using BigBalls.Services;
using DG.Tweening;
using System.Collections;
using UnityEngine;
using VContainer;

public class Crossbow : Enemy
{
    [Header("References")]
    [SerializeField] private Transform _bowStringTransform;
    [SerializeField] private Transform _launcherPivot;

    private IPlayerProvider _playerProvider;
    private Stat _attackSpeedStat;
    private IIdentifierService _identifierService;
    private ICoroutineRunner _coroutineRunner;
    private IBallFactory _ballFactory;
    private Coroutine _coroutine;

    [Header("Settings")]
    [SerializeField] private float _damageRange = 2f;
    [SerializeField] private float _projectileSpeed = 20f;
    [SerializeField] private float _spreadAngle = 10f;
    [SerializeField] private int _arrowsCount = 5;
    [SerializeField] private Vector3 _stringRestPos;
    [SerializeField] private Vector3 _stringDrawnPos;

    private WaitForSeconds _reloadDelay;
    private bool _canShoot = true;
    private float _reloadTime;

    [Inject]
    public void Construct(IDamageService damageService, IPlayerProvider playerProvider, IBallFactory ballFactory, ICoroutineRunner coroutineRunner, IIdentifierService identifierService)
    {
        _coroutineRunner = coroutineRunner;
        _ballFactory = ballFactory;
        _playerProvider = playerProvider;
        _identifierService = identifierService;
    }

    protected override void ChildInit()
    {
        _attackSpeedStat = StatHolder[StatType.AttackSpeed];
        _reloadTime = _attackSpeedStat.CurrentValue;
        _coroutine = _coroutineRunner.StartCoroutine(AttackLoop());
        _reloadDelay = new WaitForSeconds(1f / _reloadTime);
    }

    private void OnDisable()
    {
        if (_coroutine != null)
            _coroutineRunner.StopCoroutine(_coroutine);
    }

    private IEnumerator AttackLoop()
    {
        while (_canShoot)
        {
            if (_bowStringTransform != null)
                AnimateReload(_reloadTime);

            yield return _reloadDelay;
            PerformVolley();
        }
    }

    private Tween AnimateReload(float duration)
    {
        return _bowStringTransform.DOLocalMove(_stringDrawnPos, duration)
            .SetEase(Ease.InOutSine);
    }

    private void PerformVolley()
    {
        if (_playerProvider == null || _playerProvider.Player == null)
            return;

        Vector3 targetPos = _playerProvider.Player.transform.position;
        Vector3 startPos = _launcherPivot.position;
        Vector3 directionToTarget = (targetPos - startPos).normalized;

        for (int i = 0; i < _arrowsCount; i++)
        {
            int centerIndex = _arrowsCount / 2;
            float angleOffset = (i - centerIndex) * _spreadAngle;
            Quaternion baseRotation = Quaternion.LookRotation(directionToTarget);
            Quaternion shootRotation = baseRotation * Quaternion.Euler(0, angleOffset, 0);

            SpawnArrow(startPos, shootRotation);
        }
    }

    private void SpawnArrow(Vector3 position, Quaternion rotation)
    {
        BallModel ballModel = new BallModel(_identifierService.ID, Config.BallConfig);
        Vector3 direction = (rotation * Vector3.forward).normalized;

        Ball ball = _ballFactory.Create(ballModel, EntityType.Enemy, transform, rotation);
        ball.Mover.SetDirection(new Vector2(direction.x, direction.z));
        ball.transform.position = position;
        ball.Subscribe();
        //_bowStringTransform.DOLocalMove(_stringRestPos, 0.1f).SetEase(Ease.OutQuad);
    }
}
