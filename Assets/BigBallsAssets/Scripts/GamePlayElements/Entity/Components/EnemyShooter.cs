using System;
using System.Collections;
using System.Collections.Generic;
using BigBalls.Configs;
using BigBalls.Factories;
using BigBalls.GameplayObjects;
using BigBalls.Services;
using UnityEngine;
using VContainer;

public class EnemyShooter : ISubscribable
{
    private readonly Enemy _enemy;
    private readonly EnemyConfig _config;
    private readonly Stat _attackSpeed;

    private List<ICollisionStrategy> _collisionStrategies = new List<ICollisionStrategy>();
    private ICoroutineRunner _coroutineRunner;
    private IIdentifierService _identifierService;
    private IBallFactory _ballFactory;
    private SoundSpawner _soundSpawner;
    private WaitForSeconds _attackDelay;
    private Coroutine _shootRoutine;
    private Transform _target;
    private Vector3 _direction;
    private Transform _firePoint;
    private bool _canShoot = true;

    public EnemyShooter (Stat attackSpeed,
            Enemy firePoint,
            EnemyConfig config,
            Transform target)
    {
        _attackSpeed = attackSpeed;
        _enemy = firePoint;
        _config = config;
        _target = target;
        OnStatChanged(_attackSpeed);
    }

    public event Action Shooted;

    [Inject]
    public void Construct (ICoroutineRunner coroutineRunner, IIdentifierService identifierService, IBallFactory ballFactory, SoundSpawner soundSpawner)
    {
        _coroutineRunner = coroutineRunner;
        _identifierService = identifierService;
        _ballFactory = ballFactory;
        _soundSpawner = soundSpawner;
        CreateCollisionStrategiesToEnemy();

        _firePoint = _enemy.transform;
    }

    public void Subscribe ()
    {
        _attackSpeed.ValueChanged += OnStatChanged;
        _canShoot = true;
        _shootRoutine = _coroutineRunner?.StartCoroutine(ShootRoutine());
    }

    public void Unsubscribe ()
    {
        _canShoot = false;
        _attackSpeed.ValueChanged -= OnStatChanged;

        if (_shootRoutine != null)
        {
            _coroutineRunner.StopCoroutine(_shootRoutine);
            _shootRoutine = null;
        }
    }

    public void Shoot ()
    {
        BallModel ballModel = new BallModel(_identifierService.ID, _config.BallConfig);

        if (_target != null)
        {
            _direction = _target.position - _firePoint.transform.position;
        }

        _direction.y = 0;
        _direction.Normalize();

        Quaternion rotation = Quaternion.LookRotation(_direction, Vector3.up);

        Ball ball = _ballFactory.Create(ballModel, EntityType.Enemy, _enemy.Transform, _collisionStrategies, rotation);
        ball.transform.position = _firePoint.position;
        ball.Mover.SetDirection(new Vector2(_direction.x, _direction.z));
        ball.Subscribe();

        if (_config.RangeAttackSound.Clip != null)
        {
            _soundSpawner.Spawn(_config.RangeAttackSound);
        }

        Shooted?.Invoke();
    }

    public void Set (List<ICollisionStrategy> collisionStrategies) => _collisionStrategies = collisionStrategies;
    public void Set (Transform firePoint) => _firePoint = firePoint;
    public void SetTarget (Transform target) => _target = target;
    public void Set (Vector3 direction)
    {
        _target = null;
        _direction = direction;
    }

    private void OnStatChanged (IReadonlyStat stat)
    {
        _attackDelay = new WaitForSeconds((1 / stat.CurrentValue) * UnityEngine.Random.Range(0.6f, 1.4f));
    }

    private IEnumerator ShootRoutine ()
    {
        while (_canShoot)
        {
            yield return _attackDelay;

            if (_target == null)
                yield break;

            if (_enemy.EnemyAnimator == null)
            {
                Shoot();
            }
            else
            {
                _enemy.EnemyAnimator.Shoot(Shoot);
            }
        }
    }

    private void CreateCollisionStrategiesToEnemy ()
    {
        _collisionStrategies = new List<ICollisionStrategy>
        {
            new BallCollisionStrategy(),
            new HitPlayerStrategy(),
            new WallColllisionStrategy()
        };
    }
}
