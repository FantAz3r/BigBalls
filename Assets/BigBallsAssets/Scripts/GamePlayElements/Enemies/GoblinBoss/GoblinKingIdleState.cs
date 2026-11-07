using System.Collections.Generic;
using UnityEngine;

public class GoblinKingIdleState : BossIdleState<GoblinKing>
{
    private readonly System.Random _rng = new();

    public GoblinKingIdleState (GoblinKing boss, EnemyStateMachine sm, float duration)
        : base(boss, sm, duration) { }

    protected override IReadOnlyList<IEnemyState> GetAttacks () => Boss.AttackStates;
    protected override IEnemyState GetLastAttack () => Boss.LastAttackState;
    protected override void SetLastAttack (IEnemyState state) => Boss.SetLastAttackState(state);

    protected override void OnEnter ()
    {
        Boss.SetBlockStance(true);
        Boss.EnemyAnimator.EnterIdle();
        Duration = Mathf.Lerp(
            Boss.BossConfig.IdleMinDuration,
            Boss.BossConfig.IdleMaxDuration,
            (float) _rng.NextDouble());
    }

    protected override void OnTick (float dt) { }

    protected override void OnExit ()
    {
        Boss.SetBlockStance(false);
    }
}