using BigBalls.GameplayObjects;
using BigBalls.Services;
using DG.Tweening;
using System.Collections;
using UnityEngine;
using VContainer;

public class Catapult : Enemy
{
    [Header("References")]
    [SerializeField] private Transform _launcherPivot;
    [SerializeField] private Transform _leverTransform;
    [SerializeField] private CatapultProjectile _projectilePrefab;

    private IPlayerProvider _playerProvider;
    private Stat _damageStat;
    private Stat _attackSpeedStat;
    private ICoroutineRunner _coroutineRunner;
    private IPoolService _poolService;
    private Coroutine _coroutine;

    [Header("Settings")]

    [SerializeField] private float _damageRange = 2f;
    [SerializeField] private float _launchForce = 15f;
    [SerializeField] private float _minLaunchAngle = 45f;
    [SerializeField] private float _leverStartAngle = 0f;
    [SerializeField] private float _leverEndAngle = 70f;
    [SerializeField] private float _shotTime = 0.4f;
    [SerializeField] private float _arcHeight = 5f;
    [SerializeField] private float _projectileSpeed = 5;

    private WaitForSeconds _reloadDelay;
    private WaitForSeconds _shootDelay;
    private bool _canShoot = true;
    private float _reloadTime;

    [Inject]
    public void Construct(IPlayerProvider playerProvider, IPoolService poolService, ICoroutineRunner coroutineRunner)
    {
        _coroutineRunner = coroutineRunner;
        _poolService = poolService;
        _playerProvider = playerProvider;
    }

    protected override void ChildInit()
    {
        _damageStat = StatHolder[StatType.Damage];
        _attackSpeedStat = StatHolder[StatType.AttackSpeed];
        _coroutine = _coroutineRunner.StartCoroutine(AttackLoop());
        _attackSpeedStat.ValueChanged += OnAttackSpeedChanged;
        OnAttackSpeedChanged(_attackSpeedStat);
    }

    private void OnDisable()
    {
        if(_attackSpeedStat != null)
            _attackSpeedStat.ValueChanged -= OnAttackSpeedChanged;

        if (_coroutine != null)
            _coroutineRunner.StopCoroutine(_coroutine);
    }

    private IEnumerator AttackLoop()
    {
        while (_canShoot)
        {
            AnimateReload(_reloadTime);
            yield return _reloadDelay;
            PerformShot();
            AnimateShot();
            yield return _shootDelay;
        }
    }
    private Tween AnimateReload(float duration)
    {
        _leverTransform.localRotation = Quaternion.Euler(_leverStartAngle, 0, 0);
        return _leverTransform.DOLocalRotate(new Vector3(_leverEndAngle, 0, 0), duration)
            .SetEase(Ease.Linear);
    }
    private Tween AnimateShot()
    {
        return _leverTransform.DOLocalRotate(new Vector3(_leverStartAngle, 0, 0), _shotTime)
            .SetEase(Ease.Linear);
    }

    private void PerformShot()
    {
        if (_playerProvider == null)
            return;

        CatapultProjectile projectile = _poolService.GetObject(_projectilePrefab, _launcherPivot.position, Quaternion.identity);
        projectile.OnProjectileReturned += OnProjectileReturn;
        projectile.Initialize(_damageStat.CurrentValue, _damageRange);

        Vector3 startPos = _launcherPivot.position;
        Vector3 targetPos = _playerProvider.Player.transform.position;
        Vector3[] path = CalculateParabolaPath(startPos, targetPos, _arcHeight);

        float distance = Vector3.Distance(startPos, targetPos);
        float duration = distance / _projectileSpeed;

        DOVirtual.DelayedCall(duration * 0.5f, () =>
        {
            projectile.Collider.enabled = true;
        });

        projectile.transform.DOPath(path, duration, PathType.CatmullRom)
           .SetEase(Ease.Linear)
           .OnUpdate(() =>
           {

           })
           .OnComplete(() =>
           {
               //OnProjectileReturn(projectile);
           });
    }

    private Vector3[] CalculateParabolaPath(Vector3 start, Vector3 end, float height)
    {
        Vector3 midPoint = (start + end) / 2f;
        Vector3 apex = midPoint + Vector3.up * height;
        return new Vector3[] { start, apex, end };
    }

    private void OnProjectileReturn(CatapultProjectile projectile)
    {
        _poolService.ReleaseObject(projectile);
        projectile.OnProjectileReturned -= OnProjectileReturn;
    }

    private void OnAttackSpeedChanged(IReadonlyStat stat)
    {
        _reloadTime = 1f / stat.CurrentValue;
        _shootDelay = new WaitForSeconds(_shotTime);
        _reloadDelay = new WaitForSeconds(_reloadTime);
    }
}