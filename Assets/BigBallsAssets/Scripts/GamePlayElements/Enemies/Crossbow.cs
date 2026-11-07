using System.Collections;
using System.Collections.Generic;
using BigBalls.Factories;
using BigBalls.GameplayObjects;
using BigBalls.Infrastructure.DI;
using BigBalls.Services;
using DG.Tweening;
using UnityEngine;
using VContainer;

public class Crossbow : Enemy
{
    [Header("References")]
    [SerializeField] private Transform _bowStringTransform;
    [SerializeField] private Transform _launcherPivot;

    [Header("Settings")]
    [SerializeField] private float _spreadAngle = 10f;
    [SerializeField] private int _arrowsCount = 5;
    [SerializeField] private Vector3 _stringRestPos;
    [SerializeField] private Vector3 _stringDrawnPos;

    [SerializeField] private SoundStruct _sootSound;
    [SerializeField] private SoundStruct _reloadSound;

    private IPlayerProvider _playerProvider;
    private IIdentifierService _identifierService;
    private ICoroutineRunner _coroutineRunner;
    private IBallFactory _ballFactory;
    private IObjectResolverProvider _objectResolverProvider;
    private Stat _attackSpeedStat;
    private Coroutine _coroutine;
    private SoundSpawner _soundSpawner;
    private WaitForSeconds _reloadDelay;
    private bool _canShoot = true;
    private float _reloadTime;

    [Inject]
    public void Construct (
        IDamageService damageService,
        IPlayerProvider playerProvider,
        IBallFactory ballFactory,
        ICoroutineRunner coroutineRunner,
        IIdentifierService identifierService,
        SoundSpawner soundSpawner,
        IObjectResolverProvider objectResolverProvider)
    {
        _soundSpawner = soundSpawner;
        _coroutineRunner = coroutineRunner;
        _ballFactory = ballFactory;
        _playerProvider = playerProvider;
        _identifierService = identifierService;
        _objectResolverProvider = objectResolverProvider;
    }

    protected override void ChildInit ()
    {
        _attackSpeedStat = StatHolder[StatType.AttackSpeed];
        _reloadTime = _attackSpeedStat.CurrentValue;
        _coroutine = _coroutineRunner.StartCoroutine(AttackLoop());
    }

    private void OnDisable ()
    {
        if (_coroutine != null)
            _coroutineRunner.StopCoroutine(_coroutine);
    }

    private IEnumerator AttackLoop ()
    {
        while (_canShoot)
        {
            float elapsed = 0;
            SoundObject reloadSound = _soundSpawner.Spawn(_reloadSound);

            if (_bowStringTransform != null)
                AnimateReload(_reloadTime);

            while (elapsed < 1 / _reloadTime)
            {
                yield return null;
                elapsed += Time.deltaTime;
            }

            reloadSound.RemoveSound();
            Shoot();
            _soundSpawner.Spawn(_sootSound);
        }
    }

    private Tween AnimateReload (float duration)
    {
        return _bowStringTransform.DOLocalMove(_stringDrawnPos, duration)
            .SetEase(Ease.InOutSine);
    }

    private void Shoot ()
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

    private void SpawnArrow (Vector3 position, Quaternion rotation)
    {
        BallModel ballModel = new BallModel(_identifierService.ID, Config.BallConfig);
        Vector3 direction = (rotation * Vector3.forward).normalized;

        Ball ball = _ballFactory.Create(ballModel, EntityType.Enemy, transform, CreateCollisionStrategiesToEnemy(), rotation);
        ball.Mover.SetDirection(direction);
        ball.transform.position = position;
        ball.Subscribe();
        //_bowStringTransform.DOLocalMove(_stringRestPos, 0.1f).SetEase(Ease.OutQuad);
    }

    private List<ICollisionStrategy> CreateCollisionStrategiesToEnemy ()
    {
        List<ICollisionStrategy> collisionStrategies = new List<ICollisionStrategy>
        {
            new BallCollisionStrategy(),
            new HitPlayerStrategy(),
            new WallColllisionStrategy()
        };

        foreach (ICollisionStrategy strategy in collisionStrategies)
            _objectResolverProvider.CurrentResolver.Inject(strategy);

        return collisionStrategies;
    }
}
