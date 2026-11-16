using System.Collections;
using BigBalls.GameplayObjects;
using BigBalls.Services;
using UnityEngine;
using VContainer;

public class IceWizardColdNovaState : BossStateBase
{
    private readonly IceWizardBoss _boss;
    private ICoroutineRunner _coroutineRunner;
    private DebuffService _debuffService;
    private Coroutine _routine;

    public IceWizardColdNovaState (IceWizardBoss boss, EnemyStateMachine sm, float duration)
        : base(sm, boss, duration)
    {
        _boss = boss;
    }

    [Inject]
    public void Construct (ICoroutineRunner runner, DebuffService debuffService)
    {
        _coroutineRunner = runner;
        _debuffService = debuffService;
    }

    protected override void OnEnter ()
    {
        _boss.EnemyAnimator.PlayCast(Nova);
    }

    protected override void OnTick (float dt) { }

    protected override void OnComplete ()
    {
        StateMachine.ChangeState<IceWizardIdleState>();
    }

    protected override void OnExit ()
    {
        if (_routine != null)
        {
            _coroutineRunner.StopCoroutine(_routine);
            _routine = null;
        }
    }

    private void Nova ()
    {
        var cfg = _boss.BossConfig;
        _boss.NovaParticle?.Play();

        Vector3 center = _boss.transform.position;
        var hits = Physics.OverlapSphere(center, cfg.ColdNovaRadius, cfg.ColdNovaMask);

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].TryGetComponent(out Player player))
            {
                _debuffService.Apply(EffectType.Stun, player, cfg.ColdNovaStunDuration);
            }
            else if (hits[i].TryGetComponent(out Enemy enemy))
            {
                Stat shield = enemy.StatHolder[StatType.Shield];
                shield.AddCurrentValue(cfg.ColdNovaShieldAmount);
            }
        }
    }
}
