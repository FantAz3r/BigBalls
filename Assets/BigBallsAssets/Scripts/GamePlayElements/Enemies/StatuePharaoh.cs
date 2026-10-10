using System;
using System.Collections;
using BigBalls.Factories;
using BigBalls.GameplayObjects;
using BigBalls.Services;
using DG.Tweening;
using UnityEngine;
using VContainer;

public class StatuePharaoh : Enemy
{
    [SerializeField] private int _cloneZoneWidth = 9;
    [SerializeField] private int _cloneZoneDepth = 5;
    [SerializeField] private int _cloneCount = 4;
    [SerializeField] private float _jumpDuration = 0.6f;
    [SerializeField] private float _jumpHeight = 2f;
    [SerializeField] private Renderer _renderer;
    [SerializeField] private Color _cloneHitColor;

    private WaitForSeconds _separationDelay = new WaitForSeconds(2f);
    private IEnemyFactory _enemyFactory;
    private ICoroutineRunner _coroutineRunner;
    private IDamageService _damageService;
    private ClonArmy _cloneArmy;

    public event Action<StatuePharaoh> StatueDied;
    public bool IsOriginal { get; private set; } = true;
    public Stat Health { get; private set; }
    public Stat MoveSpeed { get; private set; }

    [Inject]
    public void Construct (IEnemyFactory enemyFactory, ICoroutineRunner coroutineRunner, IDamageService damageService)
    {
        _enemyFactory = enemyFactory;
        _coroutineRunner = coroutineRunner;
        _damageService = damageService;
    }

    protected override void ChildInit ()
    {
        _coroutineRunner.StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine ()
    {
        yield return _separationDelay;

        Health = StatHolder[StatType.Health];
        MoveSpeed = StatHolder[StatType.MoveSpeed];
        DeathHandler.Died += OnDied;

        if (IsOriginal)
        {
            _renderer.material.color = Color.red;
            _cloneArmy = new ClonArmy(_coroutineRunner, _damageService, _enemyFactory);
            _cloneArmy.Init(_cloneZoneWidth, _cloneZoneDepth, _cloneCount, transform.position, MoveSpeed.CurrentValue);
            _cloneArmy.SpawnClones(this);
        }
    }

    private void Update ()
    {
        if (IsOriginal && _cloneArmy != null)
        {
            _cloneArmy.UpdateOrigin(Time.deltaTime);
        }
    }

    public void InitClone ()
    {
        IsOriginal = false;
        DeathHandler.Died += OnDied;
        ColorChanger.SetHitColor(_cloneHitColor);
    }

    private void OnDied ()
    {
        DeathHandler.Died -= OnDied;
        StatueDied?.Invoke(this);
    }

    public void JumpAnimation (Vector3 targetPosition)
    {
        transform.DOKill();

        Sequence jumpSequence = DOTween.Sequence();
        jumpSequence.Append(transform.DOJump(targetPosition, _jumpHeight, 1, _jumpDuration)
            .SetEase(Ease.OutQuad));

        jumpSequence.AppendCallback(() =>
        {
            transform.DOKill();
            transform.DOScale(new Vector3(1.2f, 0.8f, 1.2f), 0.1f).OnComplete(() =>
            {
                transform.DOScale(Vector3.one, 0.1f);
            });
        });
    }
}
