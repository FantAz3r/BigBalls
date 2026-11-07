using System.Collections.Generic;

public class SunPharaohIdleState : BossIdleState<SunPharaoh>
{
    public SunPharaohIdleState (SunPharaoh boss, EnemyStateMachine stateMachine, float duration)
        : base(boss, stateMachine, duration) { }

    protected override IReadOnlyList<IEnemyState> GetAttacks () => Boss.AttackStates;
    protected override IEnemyState GetLastAttack () => Boss.LastAttackState;
    protected override void SetLastAttack (IEnemyState state) => Boss.SetLastAttackState(state);

    protected override void OnEnter () { }
    protected override void OnTick (float dt) { }
    protected override void OnExit () { }
}