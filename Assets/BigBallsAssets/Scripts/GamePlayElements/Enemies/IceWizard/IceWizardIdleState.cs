using System.Collections.Generic;
using UnityEngine;

public class IceWizardIdleState : BossIdleState<IceWizardBoss>
{
    private readonly System.Random _rng = new();

    public IceWizardIdleState (IceWizardBoss boss, EnemyStateMachine sm, float duration)
        : base(boss, sm, duration) { }

    protected override IReadOnlyList<IEnemyState> GetAttacks () => Boss.AttackStates;
    protected override IEnemyState GetLastAttack () => Boss.LastAttackState;
    protected override void SetLastAttack (IEnemyState state) => Boss.SetLastAttackState(state);

    protected override void OnEnter ()
    {
        Duration = Mathf.Lerp(Boss.BossConfig.IdleMinDuration, Boss.BossConfig.IdleMaxDuration, (float) _rng.NextDouble());
    }

    protected override void OnTick (float dt) { }
    protected override void OnExit () { }
}