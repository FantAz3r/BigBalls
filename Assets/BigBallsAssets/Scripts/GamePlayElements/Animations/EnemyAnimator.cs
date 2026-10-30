using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimator : MonoBehaviour
{
    private static readonly int ShootTrigger = Animator.StringToHash("Shoot");
    private static readonly int AttackTrigger = Animator.StringToHash("Attack");
    private static readonly int RoarTrigger = Animator.StringToHash("Roar");
    private static readonly int IdleTrigger = Animator.StringToHash("Idle");
    private static readonly int SpawnTrigger = Animator.StringToHash("Spawn");

    [SerializeField] private Animator _animator;

    private readonly Dictionary<int, Action> _callbacks = new();

    public event Action RightSteped;
    public event Action LeftSteped;

    private void Awake ()
    {
        if (_animator == null)
            _animator = GetComponent<Animator>();
    }

    public void Shoot (Action callback = null) => PlayOnce(ShootTrigger, callback);
    public void Attack (Action callback = null) => PlayOnce(AttackTrigger, callback);
    public void PlayRoar (Action callback = null) => PlayOnce(RoarTrigger, callback);
    public void BossEnter (Action callback = null) => PlayOnce(SpawnTrigger, callback);
    public void EnterIdle () => _animator.SetTrigger(IdleTrigger);


    public void OnAnimationAction (string actionName)
        => InvokeCallback(Animator.StringToHash(actionName));

    // опционально Ч пр€мые методы, если удобнее вешать в эдиторе по имени метода
    public void ShootCallback () => InvokeCallback(ShootTrigger);
    public void AttackCallback () => InvokeCallback(AttackTrigger);
    public void RoarCallback () => InvokeCallback(RoarTrigger);
    public void EnterCallback () => InvokeCallback(SpawnTrigger);
    public void StepRightCallback () => RightSteped?.Invoke();
    public void StepLeftCallback () => LeftSteped?.Invoke();


    private void PlayOnce (int triggerHash, Action callback = null)
    {
        if (callback != null)
            _callbacks[triggerHash] = callback;

        _animator?.SetTrigger(triggerHash);
    }

    private void InvokeCallback (int key)
    {
        if (_callbacks.TryGetValue(key, out var cb))
        {
            _callbacks.Remove(key);
            cb?.Invoke();
        }
    }
}