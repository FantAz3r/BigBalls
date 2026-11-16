using System.Collections.Generic;
using BigBalls.GameplayObjects;
using BigBalls.Infrastructure.DI;
using BigBalls.Services;
using UnityEngine;
using VContainer;

public class SunPharaohSunOrbState : BossStateBase
{
    private readonly SunPharaoh _boss;
    private IPoolService _poolService;
    private IObjectResolverProvider _objectResolverProvider;
    private IEntityRepository _entityRepository;
    private SunOrb _spawned;

    public SunPharaohSunOrbState (SunPharaoh boss, EnemyStateMachine stateMachine, float duration)
        : base(stateMachine, boss, duration)
    {
        _boss = boss;
    }

    [Inject]
    public void Construct (IPoolService poolService, IObjectResolverProvider objectResolverProvider, IEntityRepository entityRepository)
    {
        _poolService = poolService;
        _objectResolverProvider = objectResolverProvider;
        _entityRepository = entityRepository;
    }

    protected override void OnEnter ()
    {
        _boss.EnemyAnimator.Shoot(SpawnOrb);
    }

    private void SpawnOrb ()
    {
        SunOrb sunOrb = _poolService.Get(_boss.BossConfig.SunOrbConfig.Prefab, _boss.SunOrbSpawnPoint.position, Quaternion.identity);
        StatHolder statHolder = new StatHolder(sunOrb, EntityType.None, _boss.BossConfig.SunOrbConfig);
        List<ISubscribable> subscribables = CreateComponents(statHolder, sunOrb);
        statHolder.InitStats();

        statHolder.TryGetStat(out Stat health, StatType.Health);
        HealthHandler<SunOrb> healthHandler = new HealthHandler<SunOrb>(health, CreateComponents(statHolder, sunOrb), sunOrb);
        healthHandler.Subscribe();

        ComponentContainer componentContainer = new();

        foreach (var subscribable in subscribables)
            componentContainer.Add(subscribable);

        _entityRepository.Add(sunOrb, statHolder, componentContainer);
        sunOrb.Init( statHolder, _boss.BossConfig, _boss.SunOrbSpawnPoint.position, healthHandler);
        sunOrb.EventHandler.Died += OnDied;

    }

    private void OnDied (IEntity obj)
    {
        if (obj is not SunOrb sunOrb)
            return;

        sunOrb.HealthHandler.Unsubscribe();
        sunOrb.EventHandler.Died -= OnDied;
        _entityRepository.Remove(sunOrb);
        _poolService.Release(sunOrb);
    }

    protected override void OnTick (float dt) { }

    protected override void OnComplete ()
    {
        StateMachine.ChangeState<SunPharaohIdleState>();
    }

    protected override void OnExit ()
    {
    }

    private List<ISubscribable> CreateComponents (StatHolder statHolder, SunOrb sunOrb)
    {
        statHolder.TryGetStat(out Stat rotationSpeed, StatType.RotationSpeed);
        EntityRotator rotator = new EntityRotator(rotationSpeed, sunOrb.transform);
        _objectResolverProvider.CurrentResolver.Inject(rotator);

        List<ISubscribable> subscribables = new List<ISubscribable>()
        {
            rotator
        };

        return subscribables;
    }



}