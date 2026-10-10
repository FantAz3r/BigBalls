using System.Collections;
using System.Collections.Generic;
using BigBalls.GameplayObjects;
using BigBalls.Services;
using DG.Tweening;
using UnityEngine;
using Utils;
using VContainer;

public class PirateCock : Enemy
{
    [SerializeField] private List<FoodProjectile> _foodPrefabs;
    [SerializeField] private float _damageRange = 1.5f;
    [SerializeField] private float _projectileSpeed = 3f;
    [SerializeField] private float _arcHeight = 2f;
    [SerializeField] private float _throwRange = 4f;
    [SerializeField] private float _healDuration = 1f;
    [SerializeField] private Transform _handTransform;

    private IPoolService _poolService;
    private ICoroutineRunner _coroutineRunner;
    private IDamageService _damageService;
    private Stat _healStat;
    private Stat _attackSpeed;
    private WaitForSeconds _attackDelay;
    private Coroutine _attackRoutine;
    private bool _canAttack = true;
    private FoodProjectile _currentProjectile;
    private Enemy _currentTarget;

    private void OnEnable ()
    {
        if (_attackSpeed != null)
            _attackSpeed.ValueChanged += OnAttackSpeedChange;
    }

    private void OnDisable ()
    {
        if (_attackRoutine != null)
            _coroutineRunner.StopCoroutine(_attackRoutine);

        if (_attackSpeed != null)
            _attackSpeed.ValueChanged -= OnAttackSpeedChange;
    }

    [Inject]
    public void Construct (IPoolService poolService, ICoroutineRunner coroutineRunner, IDamageService damageService)
    {
        _poolService = poolService;
        _coroutineRunner = coroutineRunner;
        _damageService = damageService; 
    }

    protected override void ChildInit ()
    {
        _healStat = StatHolder[StatType.Damage];
        _attackSpeed = StatHolder[StatType.AttackSpeed];
        _attackSpeed.ValueChanged += OnAttackSpeedChange;
        OnAttackSpeedChange(_attackSpeed);

        _attackRoutine = _coroutineRunner.StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine ()
    {
        while (_canAttack)
        {
            Enemy enemy = GetTargetPoint();

            if (enemy == null)
            {
                yield return null;
                continue;
            }

            float attackCooldown = 1f / _attackSpeed.CurrentValue;
            float elapsed = 0f;

            while (elapsed < attackCooldown)
            {
                if (enemy == null || !enemy.gameObject.activeInHierarchy)
                {
                    enemy = null;
                    break;
                }

                RotateToTarget(enemy.transform);

                elapsed += Time.deltaTime;
                yield return null;
            }

            if (enemy == null || !enemy.gameObject.activeInHierarchy)
                continue;

            Reload(enemy);
        }
    }


    private void RotateToTarget (Transform targetTransform)
    {
        if (targetTransform == transform)
        {
            Rotator.SetDirection(Vector2.down);
            return;
        }

        if (targetTransform != null)
            Rotator.SetTarget(targetTransform);
    }

    private void Reload(Enemy target)
    {
        FoodProjectile foodProjectile = _foodPrefabs[Random.Range(0, _foodPrefabs.Count - 1)];
        FoodProjectile projectile = _poolService.Get(foodProjectile, _handTransform.position, Quaternion.identity);

        projectile.transform.SetParent(_handTransform);
        _currentProjectile = projectile;
        _currentTarget = target;

        EnemyAnimator.Shoot(ThrowFood);
    }

    private void ThrowFood ()
    {
        if (_currentTarget == null)
            return;

        _currentProjectile.transform.SetParent(null);
        Vector3 startPos = transform.position;

        Vector3[] path = Math.CalculateParabolaPath(startPos, _currentTarget.Transform.position, _arcHeight);

        float distance = Vector3.Distance(startPos, _currentTarget.Transform.position);
        float duration = distance / _projectileSpeed;

        _currentProjectile.transform.DOPath(path, duration, PathType.CatmullRom)
           .SetEase(Ease.Linear)
           .OnComplete(() =>
           {
               _damageService.ApplyHeal(_currentTarget, _healStat.CurrentValue, Color.green);
               _currentTarget.EffectViewer.EnableEffect(EffectType.Heal, _healDuration);
               OnProjectileReturn(_currentProjectile);
           });
    }

    private void OnProjectileReturn (FoodProjectile projectile)
    {
        _poolService.Release(projectile);
    }

    private void OnAttackSpeedChange (IReadonlyStat stat)
    {
        _attackDelay = new WaitForSeconds(1 / stat.CurrentValue);
    }

    private Enemy GetTargetPoint ()
    {
        List<Enemy> enemies = new List<Enemy>();
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, _throwRange);

        foreach (var hit in hitColliders)
        {
            if (hit.TryGetComponent(out Enemy entity))
            {
                if (entity == this)
                    continue;

                enemies.Add(entity);
            }
        }

        if (enemies.Count == 0)
            return this;

        return enemies[Random.Range(0, enemies.Count - 1)];
    }
}
