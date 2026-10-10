using System;
using System.Collections;
using BigBalls.GameplayObjects;
using BigBalls.Services;
using DG.Tweening;
using UnityEngine;
using VContainer;
using Random = UnityEngine.Random;

[RequireComponent(typeof(EnemyLocator))]
public class NetherButcher : Enemy
{
    [SerializeField] private Hook _hook;
    [SerializeField] private Transform _trowPoint;
    [SerializeField] private float _pullDistance = 1f;
    [SerializeField] private float _pullDuration = 0.5f;
    [SerializeField] private float _hookMaxDistance = 10f;
    [SerializeField] private float _hookSpeed = 15f;
    [SerializeField] private float _radius = 10f;
    [SerializeField] private float _healEffectTime = 1f;

    private IPlayerProvider _playerProvider;
    private ICoroutineRunner _coroutineRunner;
    private IDamageService _damageService;
    private Stat _attackSpeed;
    private Coroutine _coroutine;
    private bool _canThrow = true;
    private EnemyLocator _enemyLocator;

    [Inject]
    public void Construct (IPlayerProvider playerProvider, ICoroutineRunner coroutineRunner, IDamageService damageService)
    {
        _playerProvider = playerProvider;
        _coroutineRunner = coroutineRunner;
        _damageService = damageService;
    }

    protected override void ChildInit ()
    {
        _attackSpeed = StatHolder[StatType.AttackSpeed];
        _enemyLocator = GetComponent<EnemyLocator>();

        _hook.ÑatchedEnemy += OnEnemyCached;
        _hook.ÑatchedPlayer += OnPlayerCached;

        _coroutine = _coroutineRunner.StartCoroutine(HookRoutine());
    }

    private void OnDisable ()
    {
        _hook.ÑatchedEnemy -= OnEnemyCached;
        _hook.ÑatchedPlayer -= OnPlayerCached;

        if (_coroutine != null)
            _coroutineRunner.StopCoroutine(_coroutine);
    }

    private IEnumerator HookRoutine ()
    {
        while (_canThrow)
        {
            if (Random.Range(0, 3) == 0)
            {
                Transform enemy = _enemyLocator.GetNearestEnemy(_radius);

                if (enemy != null)
                {
                    Rotator.SetTarget(enemy);
                }
                else
                {
                    Rotator.SetTarget(_playerProvider.Player.Transform);
                }
            }
            else
            {
                Rotator.SetTarget(_playerProvider.Player.Transform);
            }

            float elapsed = 0;
            while (elapsed < 1f / _attackSpeed.CurrentValue)
            {
                yield return null;
                elapsed += Time.deltaTime;
            }

            if (_playerProvider.Player != null)
            {
                ThrowHook();
            }
        }
    }

    private void ThrowHook ()
    {
        _hook.Collider.enabled = true;
        _hook.transform.SetParent(null);
        _coroutineRunner.StartCoroutine(HookLaunchSequence());
    }

    private void OnEnemyCached (Collision collision)
    {
        Transform target = collision.transform;
        Vector3 directionToHook = (transform.position - target.position).normalized;
        Vector3 targetPos = transform.position - (directionToHook * _pullDistance);
        targetPos.y = 0.5f;

        _hook.transform.DOKill();
        target.DOKill();
        target.DOMove(targetPos, _pullDuration).SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                target.TryGetComponent(out Enemy enemy);
                enemy.DeathHandler.Kill();
                _damageService.ApplyHeal(this, enemy.StatHolder[StatType.Health].CurrentValue, Color.green);
                EffectViewer.EnableEffect(EffectType.Heal, _healEffectTime);
            });
    }

    private void OnPlayerCached (Collision collision)
    {

        Transform target = collision.transform;
        Vector3 directionToHook = (transform.position - target.position).normalized;

        directionToHook.y = 0;
        directionToHook.Normalize();

        Vector3 targetPos = transform.position - (directionToHook * _pullDistance);
        targetPos.y = 0;
        _hook.transform.DOKill();
        target.DOKill();
        target.DOMove(targetPos, _pullDuration).SetEase(Ease.OutQuad);
    }

    private IEnumerator HookLaunchSequence ()
    {
        _hook.transform.position = RotationPart.position;
        _hook.transform.rotation = RotationPart.rotation;
        _hook.gameObject.SetActive(true);

        Vector3 startPosition = _hook.transform.position;
        Vector3 direction = RotationPart.forward;

        direction.y = 0;
        direction.Normalize();

        Vector3 targetDestination = startPosition + (direction * _hookMaxDistance);

        Action<Collision> hitHandler = null;
        hitHandler = (collision) =>
        {
            _hook.ÑatchedPlayer -= OnPlayerCached;
        };

        Tween flyTween = _hook.transform.DOMove(targetDestination, _hookMaxDistance / _hookSpeed)
            .SetEase(Ease.Linear);

        while (flyTween.IsActive())
        {
            yield return null;
        }

        flyTween.Kill();

        _hook.Collider.enabled = false;

        yield return _hook.transform.DOMove(_trowPoint.position, _pullDuration)
            .SetEase(Ease.InQuad)
            .WaitForCompletion();

        _hook.transform.SetParent(_trowPoint);
        _hook.gameObject.SetActive(false);
    }
}
