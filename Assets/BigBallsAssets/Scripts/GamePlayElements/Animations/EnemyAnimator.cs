using System;
using UnityEngine;

public class EnemyAnimator : MonoBehaviour
{
    private static readonly int ShootTrigger = Animator.StringToHash("Shoot");
    private static readonly int AtackTrigger = Animator.StringToHash("Atack");
    [SerializeField] private Animator _animator;

    private Action _shootCallback;
    private Action _attackCallback;
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
}
