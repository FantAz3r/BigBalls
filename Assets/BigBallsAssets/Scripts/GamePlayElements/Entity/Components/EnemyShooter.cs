using BigBalls.Configs;
using BigBalls.Factories;
using BigBalls.GameplayObjects;
using BigBalls.Services;
using System;
using System.Collections;
using UnityEngine;
using VContainer;

public class EnemyShooter : ISubscribable
{
    private readonly Enemy _enemy;
    private readonly EnemyConfig _config;
    private readonly WaitForSeconds _attackDelay;

    private Coroutine _shootRoutine;
    private ICoroutineRunner _coroutineRunner;
    private bool _canShoot = true;
    private Transform _target;
    private IIdentifierService _identifierService;
    private IBallFactory _ballFactory;

    public EnemyShooter(Stat attackSpeed,
            Enemy firePoint,
            EnemyConfig config,
            Transform target)
    {
        _attackDelay = new WaitForSeconds((1 / attackSpeed.CurrentValue) * UnityEngine.Random.Range(0.6f, 1.4f));
        _enemy = firePoint;
        _config = config;
        _target = target;
    }

    public event Action Shooted;

    [Inject]
    public void Construct(ICoroutineRunner coroutineRunner, IIdentifierService identifierService, IBallFactory ballFactory)
    {
        _coroutineRunner = coroutineRunner;
        _identifierService = identifierService;
        _ballFactory = ballFactory;
    }

    public void Subscribe()
    {
        _canShoot = true;
        _shootRoutine = _coroutineRunner?.StartCoroutine(ShootRoutine());
    }

    public void Unsubscribe()
    {
        _canShoot = false;

        if (_shootRoutine != null)
        {
            _coroutineRunner.StopCoroutine(_shootRoutine);
            _shootRoutine = null;
        }
    }

    public void Shoot()
    {
        BallModel ballModel = new BallModel(_identifierService.ID, _config.BallConfig);
        Vector3 direction = _target.position - _enemy.transform.position;
        direction.y = 0;
        direction.Normalize();
        Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);

        Ball ball = _ballFactory.Create(ballModel, EntityType.Enemy, _enemy.Transform, rotation);
        ball.Mover.SetDirection(new Vector2(direction.x, direction.z));
        ball.transform.position = _enemy.transform.position;
        Shooted?.Invoke();
    }

    private IEnumerator ShootRoutine()
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
}
