using System.Collections.Generic;
using UnityEngine;

public class GoblinKingIdleState : BossStateBase
{
    private readonly System.Random _rng = new();
    private readonly List<IEnemyState> _candidates = new();
    private GoblinKing _goblinKing;

    public GoblinKingIdleState (GoblinKing boss, EnemyStateMachine sm, float duration)
        : base(sm, boss, duration)
    {
        _goblinKing = boss;
    }

    protected override void OnEnter ()
    {
        _goblinKing.SetBlockStance(true);
        _goblinKing.EnemyAnimator.EnterIdle();
        Duration = Mathf.Lerp(_goblinKing.BossConfig.IdleMinDuration, _goblinKing.BossConfig.IdleMaxDuration, (float) _rng.NextDouble());
    }

    protected override void OnTick (float dt) { }

    protected override void OnComplete ()
    {
        StateMachine.ChangeState(PickNextAttackState());
    }

    protected override void OnExit ()
    {
        _goblinKing.SetBlockStance(false);
    }

    private IEnemyState PickNextAttackState ()
    {
        var attacks = _goblinKing.AttackStates;
        var last = _goblinKing.LastAttackState;
        _candidates.Clear();

        for (int i = 0; i < attacks.Count; i++)
            if (attacks[i] != last) _candidates.Add(attacks[i]);

        var next = _candidates[_rng.Next(_candidates.Count)];
        _goblinKing.SetLastAttackState(next);
        return next;
    }
}
