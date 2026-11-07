using BigBalls.Services;
using UnityEngine;
using VContainer;

public class BossHandAttackState : BossStateBase
{
    private readonly SkeletonKing _skeletonKing;
    private IPlayerProvider _playerProvider;

    public BossHandAttackState (SkeletonKing skeleton, EnemyStateMachine stateMachine, float duration)
        : base(stateMachine, skeleton, duration)
    {
        _skeletonKing = skeleton;
    }

    [Inject]
    public void Construct (IPlayerProvider playerProvider)
    {
        _playerProvider = playerProvider;
    }

    protected override void OnEnter ()
    {
        _skeletonKing.EnemyAnimator.Shoot(OnShoot);
    }

    private void OnShoot ()
    {
        _skeletonKing.Animator.enabled = false;
        var config = _skeletonKing.BossConfig;
        Duration = config.HandAttackDuration;

        var hand = _skeletonKing.LeftHand;

        if (hand == null)
        {
            return;
        }

        Vector3 dir = _playerProvider.Player.Transform.position - _skeletonKing.Transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.0001f)
            dir = _skeletonKing.Transform.forward;

        hand.Punch(
            _skeletonKing.BossConfig,
            worldDirection: dir,
            distance: config.HandFlyDistance,
            travelTime: config.HandTravelTime,
            returnTime: config.HandReturnTime,
            onFinished: () => { });
    }

    protected override void OnTick (float dt) { }

    protected override void OnComplete ()
    {
        _skeletonKing.Animator.enabled = true;
        _skeletonKing.EnemyAnimator.EnterIdle();
        StateMachine.ChangeState<SkeletonIdleState>();
    }

    protected override void OnExit ()
    {
        base.OnExit();
        _skeletonKing.EnemyAnimator.EnterIdle();
        _skeletonKing.LeftHand?.ResetToHome();
    }
}