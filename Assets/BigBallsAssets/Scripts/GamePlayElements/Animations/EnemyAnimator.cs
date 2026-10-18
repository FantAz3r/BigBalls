using System;
using BigBalls.GameplayObjects;
using UnityEngine;

public class EnemyAnimator : MonoBehaviour
{
    private static readonly int ShootTrigger = Animator.StringToHash("Shoot");
    private static readonly int AtackTrigger = Animator.StringToHash("Atack");
    private static readonly int Spawn = Animator.StringToHash("Spawn");
    private static readonly int Idle = Animator.StringToHash("Idle");

    [SerializeField] private Animator _animator;

    private Action _shootCallback;
    private Action _attackCallback;
    private Action _enterCallback;
    public event Action RightSteped;
    public event Action LeftSteped;
    private bool _isShooting = false;

    public void Shoot (Action callback)
    {
        _shootCallback = callback;

        if (!_isShooting)
        {
            _isShooting = true;
            _animator?.SetTrigger(ShootTrigger);
        }
    }

    public void Attack (Action callback)
    {
        _attackCallback = callback;
        _animator?.SetTrigger(AtackTrigger);
    }

    public void AttackCallback ()
    {
        _attackCallback?.Invoke();
        _attackCallback = null;
    }

    public void ShootCallback ()
    {
        _shootCallback?.Invoke();
        _shootCallback = null;
        _isShooting = false;
    }

    public void BossEnter(Action callback)
    {
        _enterCallback = callback;
        _animator?.SetBool(Spawn, true);
    }

    public void EnterCallback()
    {
        _animator?.SetBool(Spawn, false);
        _enterCallback?.Invoke();
    }

    public void StepRightCallback()
    {
        RightSteped?.Invoke();
    }

    public void StepLeftCallback ()
    {
        LeftSteped?.Invoke();
    }

    public void EnterIdle()
    {
        _animator?.SetTrigger(Idle);
    }
}
