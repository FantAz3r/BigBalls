using System.Collections.Generic;
using UnityEngine;

public class BossIdleState : BossStateBase
{
    private readonly System.Random _rng;
    private readonly SkeletonKing _skeletonKing;
    private readonly List<IEnemyState> _candidates = new();

    public BossIdleState (SkeletonKing skeleton, EnemyStateMachine stateMachine, float duration)
        : base(stateMachine, skeleton, duration)
    {
        _skeletonKing = skeleton;
        _rng = new System.Random();
    }

    protected override void OnEnter () { }
    protected override void OnTick (float dt) { }

    protected override void OnComplete ()
    {
        StateMachine.ChangeState(PickNextAttackState());
    }

    protected override void OnExit ()
    {
        base.OnExit();
    }

    private IEnemyState PickNextAttackState ()
    {
        var attacks = _skeletonKing.AttackStates;
        var last = _skeletonKing.LastAttackState;

        _candidates.Clear();
        for (int i = 0; i < attacks.Count; i++)
        {
            if (attacks[i] != last)
                _candidates.Add(attacks[i]);
        }

        IEnemyState nextState = _candidates[_rng.Next(_candidates.Count)];
        _skeletonKing.SetLastAttackState(nextState);
        Debug.Log(nextState);
        return nextState;
    }
}