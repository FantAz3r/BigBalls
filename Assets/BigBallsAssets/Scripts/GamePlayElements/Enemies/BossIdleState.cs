using System.Collections.Generic;
using BigBalls.GameplayObjects;
using UnityEngine;

public abstract class BossIdleState<TBoss> : BossStateBase
    where TBoss : LevelBoss
{
    private readonly System.Random _rng = new();
    private readonly List<IEnemyState> _candidates = new();

    protected TBoss Boss { get; }

    protected BossIdleState (TBoss boss, EnemyStateMachine stateMachine, float duration)
        : base(stateMachine, boss, duration)
    {
        Boss = boss;
    }

    protected abstract IReadOnlyList<IEnemyState> GetAttacks ();

    protected abstract IEnemyState GetLastAttack ();

    protected abstract void SetLastAttack (IEnemyState state);

    protected override void OnComplete ()
    {
        StateMachine.ChangeState(PickNextAttackState());
    }

    private IEnemyState PickNextAttackState ()
    {
        var attacks = GetAttacks();
        var last = GetLastAttack();

        _candidates.Clear();
        for (int i = 0; i < attacks.Count; i++)
        {
            if (attacks[i] != last)
                _candidates.Add(attacks[i]);
        }

        if (_candidates.Count == 0 && attacks.Count > 0)
            _candidates.Add(attacks[0]);

        var next = _candidates[_rng.Next(_candidates.Count)];
        SetLastAttack(next);
        return next;
    }
}