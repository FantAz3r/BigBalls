using UnityEngine;
using VContainer;

public class BossBoneFanState : BossStateBase
{
    private int _currentIndex;
    private float _nextShotTime;
    private SkeletonKing _skeletonKing;

    public BossBoneFanState (SkeletonKing skeleton, EnemyStateMachine stateMachine, float duration) : base(stateMachine, skeleton, duration)
    {
        _skeletonKing = skeleton;
    }

    [Inject]
    public void Construct ()
    {
    }

    protected override void OnEnter ()
    {
        int count = _skeletonKing.BossConfig.FanBoneCount;
        float interval = _skeletonKing.BossConfig.FanShotInterval;

        Duration = count * interval;
        _currentIndex = 0;
        _nextShotTime = 0f;
    }

    protected override void OnTick (float dt)
    {
        if (_currentIndex >= _skeletonKing.BossConfig.FanBoneCount)
            return;

        if (Elapsed < _nextShotTime)
            return;

        FireBone(_currentIndex);

        _currentIndex++;
        _nextShotTime = _currentIndex * _skeletonKing.BossConfig.FanShotInterval;
    }

    private void FireBone (int index)
    {
        int total = _skeletonKing.BossConfig.FanBoneCount;
        float spread = _skeletonKing.BossConfig.FanSpreadAngle;
        float t = total == 1 ? 0.5f : index / (float) (total - 1);
        float angle = Mathf.Lerp(-spread * 0.5f, spread * 0.5f, t);

        Vector3 baseDir = Vector3.back;
        baseDir.y = 0f;
        if (baseDir.sqrMagnitude < 0.001f) baseDir = Vector3.back;
        baseDir.Normalize();

        Vector3 dir = Quaternion.Euler(0f, angle, 0f) * baseDir;

        Vector3 spawnPos = _skeletonKing.Transform.position + dir * _skeletonKing.BossConfig.FanSpawnOffset;

        _skeletonKing.SpawnBoneProjectile(_skeletonKing.BoneSpawnPoint, dir);
        //_skeletonKing.EnemyAnimator?.PlayAttack();
    }

    protected override void OnComplete ()
    {
        _skeletonKing.StateMachine.ChangeState<BossIdleState>();
    }
}
