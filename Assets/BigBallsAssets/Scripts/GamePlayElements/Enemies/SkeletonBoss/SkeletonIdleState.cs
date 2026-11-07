using System.Collections.Generic;

public class SkeletonIdleState : BossIdleState<SkeletonKing>
{
    public SkeletonIdleState (SkeletonKing skeleton, EnemyStateMachine stateMachine, float duration)
        : base(skeleton, stateMachine, duration) { }

    protected override IReadOnlyList<IEnemyState> GetAttacks () => Boss.AttackStates;
    protected override IEnemyState GetLastAttack () => Boss.LastAttackState;
    protected override void SetLastAttack (IEnemyState state) => Boss.SetLastAttackState(state);

    protected override void OnEnter () { }
    protected override void OnTick (float dt) { }
    protected override void OnExit () { }
}