using System;
using System.Collections;
using System.Collections.Generic;
using BigBalls.GameplayObjects;
using BigBalls.Infrastructure.DI;
using BigBalls.Services;
using UnityEngine;
using VContainer;

public class SkeletonKing : LevelBoss
{
    [field: SerializeField] public Ball BoneProjectilePrefab { get; private set; }
    [field: SerializeField] public SkeletonKingConfig BossConfig { get; private set; }
    [field: SerializeField] public Transform BoneSpawnPoint { get; private set; }
    [field: SerializeField] public Hand RightHand { get; private set; }
    [field: SerializeField] public Hand LeftHand { get; private set; }


    private IPoolService _poolService;
    private IObjectResolverProvider _resolverProvider;
    private ICoroutineRunner _coroutineRunner;
    private bool _isSpawned;

    public event Action Spawned;
    public EnemyStateMachine StateMachine { get; private set; }
    public IEnemyState LastAttackState { get; private set; }
    public List<IEnemyState> AttackStates { get; private set; } = new List<IEnemyState>();

    protected override void OnDisable ()
    {
        base.OnDisable ();
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

        StateMachine.Register(new BossIdleState(this, StateMachine, BossConfig.IdleMinDuration));
        BossBoneCloudState bossBoneCloudState = new BossBoneCloudState(this, StateMachine, BossConfig.BoneCloudDuration);
        StateMachine.Register(bossBoneCloudState);
        AttackStates.Add(bossBoneCloudState);

        BossHandAttackState bossHandAttackState = new BossHandAttackState(this, StateMachine, BossConfig.HandAttackDuration);
        AttackStates.Add(bossHandAttackState);
        StateMachine.Register(bossHandAttackState);

        BossBoneFanState bossBoneFanState = new BossBoneFanState(this, StateMachine, BossConfig.FanBoneDuration);
        AttackStates.Add(bossBoneFanState);
        StateMachine.Register(bossBoneFanState);

        _coroutineRunner.StartCoroutine(SpawnRoutine());
    }

    public void SpawnBoneProjectile (Transform position, Vector3 direction)
    {
        Shooter.Set(position);
        Shooter.Set(direction);
        Shooter.Shoot();
    }
    public void SetLastAttackState (IEnemyState stateType)
    {
        LastAttackState = stateType;
    }

    private IEnumerator SpawnRoutine ()
    {
        EnemyAnimator.BossEnter(OnSpawnEnded);

        while (_isSpawned == false)
        {
            yield return null;
        }

        Spawned?.Invoke();
        StateMachine.Start<BossIdleState>();
    }

    private void OnSpawnEnded ()
    {
        _isSpawned = true;
        Mover.Stop();
    }
}