using System;
using UnityEngine;

public class EnemyAnimator : MonoBehaviour
{
    private static readonly int ShootTrigger = Animator.StringToHash("Shoot");
    [SerializeField] private Animator animator;

    private Action _shootCallback;
    private bool _isShooting = false;

    public void Shoot(Action callback)
    {
        _shootCallback = callback;

        if (!_isShooting)
        {
            _isShooting = true;
            animator?.SetTrigger(ShootTrigger);
        }
    }

    public void ShootCallback()
    {
        _shootCallback?.Invoke();
        _shootCallback = null;
        _isShooting = false;
    }
}
