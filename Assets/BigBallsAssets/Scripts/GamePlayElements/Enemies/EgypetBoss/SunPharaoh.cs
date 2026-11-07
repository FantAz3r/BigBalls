using System;
using System.Collections.Generic;
using BigBalls.GameplayObjects;
using BigBalls.Infrastructure.DI;
using UnityEngine;
using VContainer;

public class SunPharaoh : LevelBoss
{
    [field: SerializeField] public SunPharaohConfig BossConfig { get; private set; }
    [field: SerializeField] public Animator Animator { get; private set; }
    [field: SerializeField] public Transform EyeOriginLeft { get; private set; }
    [field: SerializeField] public Transform EyeOriginRight { get; private set; }
    [field: SerializeField] public Transform SunOrbSpawnPoint { get; private set; }
    [field: SerializeField] public Transform FieldOrigin { get; private set; }

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
    public void Construct (IObjectResolverProvider objectResolverProvider)
    {
        _resolverProvider = objectResolverProvider;
    }

    protected override void ChildInit ()
    {
        StateMachine = new EnemyStateMachine(_resolverProvider);

        StateMachine.Register(new BossIdleState(this, StateMachine, BossConfig.IdleMinDuration));

        var laser = new SunPharaohEyeLaserState(this, StateMachine, BossConfig.EyeLaserDuration);
        StateMachine.Register(laser);
        AttackStates.Add(laser);

        var orb = new SunPharaohSunOrbState(this, StateMachine, BossConfig.SunOrbDuration);
        StateMachine.Register(orb);
        AttackStates.Add(orb);

        var streams = new SunPharaohSolarStreamsState(this, StateMachine, BossConfig.SolarStreamsDuration);
        StateMachine.Register(streams);
        AttackStates.Add(streams);

        EnemyAnimator.BossEnter(OnSpawnEnded);
    }

    public void SetLastAttackState (IEnemyState state) => LastAttackState = state;

    private void OnSpawnEnded ()
    {
        Mover.Stop();
        Spawned?.Invoke();
        StateMachine.Start<SunPharaohIdleState>();
    }
}