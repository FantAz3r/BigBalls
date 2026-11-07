using BigBalls.Infrastructure.DI;
using BigBalls.Services;
using UnityEngine;
using VContainer;

public class SunPharaohSunOrbState : BossStateBase
{
    private readonly SunPharaoh _boss;
    private IObjectResolverProvider _resolverProvider;
    private SunOrb _spawned;

    public SunPharaohSunOrbState (SunPharaoh boss, EnemyStateMachine stateMachine, float duration)
        : base(stateMachine, boss, duration)
    {
        _boss = boss;
    }

    [Inject]
    public void Construct (IObjectResolverProvider resolverProvider)
    {
        _resolverProvider = resolverProvider;
    }

    protected override void OnEnter ()
    {
        var cfg = _boss.BossConfig;
        var go = Object.Instantiate(cfg.SunOrbPrefab, _boss.SunOrbSpawnPoint.position, Quaternion.identity);
        _spawned = go.GetComponent<SunOrb>();
        if (_spawned != null)
        {
            _resolverProvider.CurrentResolver.Inject(_spawned);
            _spawned.Init(_boss, cfg, _boss.SunOrbSpawnPoint.position);
        }
    }

    protected override void OnTick (float dt) { }

    protected override void OnComplete ()
    {
        StateMachine.ChangeState<SunPharaohIdleState>();
    }

    protected override void OnExit ()
    {
        if (_spawned != null)
        {
            Object.Destroy(_spawned.gameObject);
            _spawned = null;
        }
    }
}