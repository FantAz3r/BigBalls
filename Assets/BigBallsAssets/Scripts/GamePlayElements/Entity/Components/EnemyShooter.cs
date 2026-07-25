using System;
using System.Collections;
using BigBalls.Configs;
using BigBalls.Factories;
using BigBalls.GameplayObjects;
using BigBalls.Services;
using UnityEngine;
using VContainer;

public class EnemyShooter : ISubscribable
{
    private readonly Transform _firePoint;
    private readonly EnemyConfig _config;
    private readonly WaitForSeconds _attackDelay;

    private Coroutine _shootRoutine;
    private ICoroutineRunner _coroutineRunner;
    private bool _canShoot = true;
    private Transform _target;
    private IIdentifierService _identifierService;
    private IBallFactory _ballFactory;

    public EnemyShooter (Stat attackSpeed,
            Transform firePoint,
            EnemyConfig config,
            Transform target)
    {
        _attackDelay = new WaitForSeconds(1 / attackSpeed.CurrentValue);
        _firePoint = firePoint;
        _config = config;
        _target = target;
    }

    public event Action Shooted;

    [Inject]
    public void Construct (ICoroutineRunner coroutineRunner, IIdentifierService identifierService, IBallFactory ballFactory)
    {
        _coroutineRunner = coroutineRunner;
        _identifierService = identifierService;
        _ballFactory = ballFactory;
    }

    public void Subscribe ()
    {
        _canShoot = true;
        _shootRoutine = _coroutineRunner?.StartCoroutine(ShootRoutine());
    }

    public void Unsubscribe ()
    {
        _canShoot = false;

        if (_shootRoutine != null)
        {
            _coroutineRunner.StopCoroutine(_shootRoutine);
            _shootRoutine = null;
        }
    }

    public void Shoot (Ball ball)
    {
        ball.transform.position = _firePoint.position;
        Vector3 toTarget = _target.position - ball.transform.position;
        toTarget.y = 0f;
        ball.transform.rotation = Quaternion.LookRotation(toTarget.normalized, Vector3.up);
        Vector2 dir2D = new Vector2(toTarget.x, toTarget.z).normalized;
        ball.Mover.SetDirection(dir2D);

        Shooted?.Invoke();
    }

    private IEnumerator ShootRoutine ()
    {
        while (_canShoot)
        {
            BallModel ballModel = new BallModel(_identifierService.ID, _config.BallConfig);
            Ball ball = _ballFactory.Create(ballModel, EntityType.Enemy);
            Shoot(ball);
            yield return _attackDelay;
        }
    }
}
