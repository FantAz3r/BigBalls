using System.Collections;
using BigBalls.GameplayObjects;
using BigBalls.Services;
using DG.Tweening;
using UnityEngine;
using Utils;
using VContainer;

public class Catapult : Enemy
{
    [Header("References")]
    [SerializeField] private Transform _launcherPivot;
    [SerializeField] private Transform _leverTransform;
    [SerializeField] private CatapultProjectile _projectilePrefab;

    [Header("Settings")]
    [SerializeField] private float _damageRange = 2f;
    [SerializeField] private float _launchForce = 15f;
    [SerializeField] private float _minLaunchAngle = 45f;
    [SerializeField] private float _leverStartAngle = 0f;
    [SerializeField] private float _leverEndAngle = 70f;
    [SerializeField] private float _shotTime = 0.4f;
    [SerializeField] private float _arcHeight = 5f;
    [SerializeField] private float _projectileSpeed = 5;

    private IPlayerProvider _playerProvider;
    private Stat _damageStat;
    private Stat _attackSpeedStat;
    private ICoroutineRunner _coroutineRunner;
    private IPoolService _poolService;
    private Coroutine _coroutine;
    private bool _canShoot = true;

    [Inject]
    public void Construct (IPlayerProvider playerProvider, IPoolService poolService, ICoroutineRunner coroutineRunner)
    {
        _coroutineRunner = coroutineRunner;
        _poolService = poolService;
        _playerProvider = playerProvider;
    }

    protected override void ChildInit ()
    {
        _damageStat = StatHolder[StatType.Damage];
        _attackSpeedStat = StatHolder[StatType.AttackSpeed];
        _coroutine = _coroutineRunner.StartCoroutine(AttackLoop());
    }

    private void OnDisable ()
    {
        if (_coroutineRunner != null && _coroutine != null)
            _coroutineRunner.StopCoroutine(_coroutine);
    }

    private IEnumerator AttackLoop ()
    {
        while (_canShoot)
        {
            float reloadTime = 1f / _attackSpeedStat.CurrentValue;
            AnimateReload(reloadTime);
            float elapsed = 0;

            while (elapsed < reloadTime)
            {
                yield return null;
                elapsed += Time.deltaTime;
            }

            PerformShot();
            AnimateShot();

            yield return new WaitForSeconds(_shotTime);
        }
    }
    private Tween AnimateReload (float duration)
    {
        _leverTransform.localRotation = Quaternion.Euler(_leverStartAngle, 0, 0);
        return _leverTransform.DOLocalRotate(new Vector3(_leverEndAngle, 0, 0), duration)
            .SetEase(Ease.Linear);
    }

    private Tween AnimateShot ()
    {
        return _leverTransform.DOLocalRotate(new Vector3(_leverStartAngle, 0, 0), _shotTime)
            .SetEase(Ease.Linear);
    }

    private void PerformShot ()
    {
        if (_playerProvider == null)
            return;

        CatapultProjectile projectile = _poolService.Get(_projectilePrefab, _launcherPivot.position, Quaternion.identity);
        projectile.OnProjectileReturned += OnProjectileReturn;
        projectile.Initialize(_damageStat.CurrentValue, _damageRange);

        Vector3 startPos = _launcherPivot.position;
        Vector3 targetPos = _playerProvider.Player.transform.position;
        Vector3[] path = Math.CalculateParabolaPath(startPos, targetPos, _arcHeight);

        float distance = Vector3.Distance(startPos, targetPos);
        float duration = distance / _projectileSpeed;

        DOVirtual.DelayedCall(duration * 0.5f, () =>
        {
            projectile.Collider.enabled = true;
        });

        projectile.transform.DOPath(path, duration, PathType.CatmullRom)
           .SetEase(Ease.Linear);
    }

    private void OnProjectileReturn (CatapultProjectile projectile)
    {
        _poolService.Release(projectile);
        projectile.OnProjectileReturned -= OnProjectileReturn;
    }
}