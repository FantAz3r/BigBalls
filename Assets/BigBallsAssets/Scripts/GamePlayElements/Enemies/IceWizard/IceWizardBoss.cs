using System;
using System.Collections.Generic;
using BigBalls.GameplayObjects;
using BigBalls.Infrastructure.DI;
using UnityEngine;
using VContainer;

public class IceWizardBoss : LevelBoss
{
    [field: SerializeField] public IceWizardBossConfig BossConfig { get; private set; }
    [field: SerializeField] public Transform BeamOrigin { get; private set; }
    [field: SerializeField] public ParticleObject NovaParticle { get; private set; }
    [field: SerializeField] public ParticleObject IceBeam { get; private set; }

    private IObjectResolverProvider _resolverProvider;

    public event Action Spawned;
    public EnemyStateMachine StateMachine { get; private set; }
    public IEnemyState LastAttackState { get; private set; }
    public List<IEnemyState> AttackStates { get; } = new List<IEnemyState>();

    protected override void OnDisable ()
    {
        base.OnDisable();
        StateMachine?.Stop();
    }

    [Inject]
    public void Construct (IObjectResolverProvider resolverProvider)
    {
        _resolverProvider = resolverProvider;
    }

    protected override void ChildInit ()
    {
        StateMachine = new EnemyStateMachine(_resolverProvider);

        StateMachine.Register(new IceWizardIdleState(this, StateMachine, BossConfig.IdleMinDuration));

        var beam = new IceWizardFreezeBeamState(this, StateMachine, BossConfig.FreezeBeamDuration);
        StateMachine.Register(beam);
        AttackStates.Add(beam);

        var nova = new IceWizardColdNovaState(this, StateMachine, BossConfig.ColdNovaDuration);
        StateMachine.Register(nova);
        AttackStates.Add(nova);

        var ball = new IceWizardIceBallState(this, StateMachine, BossConfig.IceBallDuration);
        StateMachine.Register(ball);
        AttackStates.Add(ball);

        var rain = new IceWizardIcicleRainState(this, StateMachine, BossConfig.IcicleRainDuration);
        StateMachine.Register(rain);
        AttackStates.Add(rain);

        EnemyAnimator.BossEnter(OnSpawnEnded);
    }

    public void SetLastAttackState (IEnemyState state) => LastAttackState = state;

    private void OnSpawnEnded ()
    {
        Mover.Stop();
        Spawned?.Invoke();
        StateMachine.Start<IceWizardIdleState>();
    }
}
