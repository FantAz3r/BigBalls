using System.Collections.Generic;
using BigBalls.Factories;
using BigBalls.Infrastructure;
using BigBalls.Infrastructure.DI;
using BigBalls.Providers;
using BigBalls.Services;
using UnityEngine;
using VContainer;

public class IceWizardIcicleRainState : BossStateBase
{
    private const int StartHeght = 20;
    private readonly IceWizardBoss _boss;
    private IObjectResolverProvider _resolverProvider;
    private ISceneContainerProvider _sceneContainerProvider;
    private IEnemyFactory _enemyFactory;
    private IPoolService _poolService;
    private CreateLevelState _levelState;

    public IceWizardIcicleRainState (IceWizardBoss boss, EnemyStateMachine sm, float duration)
        : base(sm, boss, duration)
    {
        _boss = boss;
    }

    [Inject]
    public void Construct (IObjectResolverProvider resolverProvider, IEnemyFactory enemyFactory, CreateLevelState levelState, IPoolService poolService, ISceneContainerProvider sceneContainerProvider)
    {
        _resolverProvider = resolverProvider;
        _enemyFactory = enemyFactory;
        _levelState = levelState;
        _poolService = poolService;
        _sceneContainerProvider = sceneContainerProvider;
    }

    protected override void OnEnter ()
    {
        _boss.EnemyAnimator.Spell(OnAnimationEvent);
    }

    private void OnAnimationEvent ()
    {
        IceWizardBossConfig cfg = _boss.BossConfig;
        List<FieldCell> cells = _sceneContainerProvider.BattleField.FreeCells;
        int count = Random.Range(cfg.IcicleMin, cfg.IcicleMax);

        for (int i = 0; i < count; i++)
        {
            int id = Random.Range(0, cells.Count);
            FieldCell cell = cells[id];
            Vector3 target = cell.transform.position;
            Icicle icicle = _poolService.Get(cfg.IciclePrefab, target + Vector3.up * StartHeght, Quaternion.identity);

            if (icicle != null)
            {
                _resolverProvider.CurrentResolver.Inject(icicle);
                icicle.Init(target, fallHeight: StartHeght,
                            telegraphTime: cfg.IcicleTelegraphTime,
                            fallTime: cfg.IcicleFallTime,
                            damage: cfg.IcicleDamage,
                            mask: cfg.IcicleDamageMask,
                            SpawnRandomEnemyAt);
            }

            void SpawnRandomEnemyAt ()
            {
                var available = _levelState.LevelConfig.ConfigsForPool;

                if (available == null)
                    return;

                var cfg = available[Random.Range(0, available.Count)];
                var enemy = _enemyFactory.Create(cfg, target);
            }
        }
    }

    protected override void OnTick (float dt) { }

    protected override void OnComplete ()
    {
        StateMachine.ChangeState<IceWizardIdleState>();
    }

    protected override void OnExit ()
    {
    }
}