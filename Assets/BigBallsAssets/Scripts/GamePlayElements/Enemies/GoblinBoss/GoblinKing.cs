using System;
using System.Collections;
using System.Collections.Generic;
using BigBalls.GameplayObjects;
using BigBalls.Infrastructure.DI;
using BigBalls.Services;
using UnityEngine;
using VContainer;

public class GoblinKing : LevelBoss
{
    [field: SerializeField] public GoblinKingConfig BossConfig { get; private set; }
    [field: SerializeField] public Transform RightHandSpawn { get; private set; }
    [field: SerializeField] public Transform LeftHandSpawn { get; private set; }
    [field: SerializeField] public Transform AxeFanSpawnPoint { get; private set; }
    [field: SerializeField] public Collider BlockShield { get; private set; }
    [field: SerializeField] public BoomerangAxe BoomerangAxeLeft { get; private set; }
    [field: SerializeField] public BoomerangAxe BoomerangAxeRight { get; private set; }
    [field: SerializeField] public AxeProjectile AxeProjectilePrefab { get; private set; }

    private IPoolService _poolService;
    private IObjectResolverProvider _resolverProvider;
    private ICoroutineRunner _coroutineRunner;
    private bool _isSpawned;

    public event Action Spawned;
    public EnemyStateMachine StateMachine { get; private set; }
    public IEnemyState LastAttackState { get; private set; }
    public List<IEnemyState> AttackStates { get; private set; } = new();

    protected override void OnDisable ()
    {
        base.OnDisable();
        StateMachine?.Stop();
    }

    [Inject]
    public void Construct (IPoolService poolService, IObjectResolverProvider objectResolverProvider, ICoroutineRunner coroutineRunner)
    {
        _poolService = poolService;
        _resolverProvider = objectResolverProvider;
        _coroutineRunner = coroutineRunner;
    }

    protected override void ChildInit ()
    {
        StateMachine = new EnemyStateMachine(_resolverProvider);

        GoblinKingIdleState goblinKingIdleState = new GoblinKingIdleState(this, StateMachine, BossConfig.IdleMinDuration);
        StateMachine.Register(goblinKingIdleState);
        AttackStates.Add(goblinKingIdleState);

        GoblinBoomerangState goblinBoomerangState = new GoblinBoomerangState(this, StateMachine, BossConfig.BoomerangDuration);
        StateMachine.Register(goblinBoomerangState);
        AttackStates.Add(goblinBoomerangState);

        GoblinAxeFanState goblinAxeFanState = new GoblinAxeFanState(this, StateMachine, BossConfig.AxeFanDuration);
        StateMachine.Register(goblinAxeFanState);
        AttackStates.Add(goblinAxeFanState);

        GoblinRoarState goblinRoarState = new GoblinRoarState(this, StateMachine, BossConfig.RoarDuration);
        StateMachine.Register(goblinRoarState);
        AttackStates.Add(goblinRoarState);

        EnemyAnimator.BossEnter(OnSpawnEnded);
    }

    public void SetLastAttackState (IEnemyState state) => LastAttackState = state;

    public AxeProjectile SpawnFanAxe (Vector3 pos, Vector3 dir)
    {
        var axe = _poolService.Get<AxeProjectile>(AxeProjectilePrefab);
        axe.Launch(pos, dir, BossConfig.AxeFanProjectileSpeed, BossConfig.AxeFanDamage);
        return axe;
    }

    public void SetBlockStance (bool active)
    {
        BlockShield.gameObject.SetActive(active);
    }

    private void OnSpawnEnded ()
    {
        _isSpawned = true;
        Mover.Stop();
        Spawned?.Invoke();
        StateMachine.Start<GoblinKingIdleState>();
    }
}