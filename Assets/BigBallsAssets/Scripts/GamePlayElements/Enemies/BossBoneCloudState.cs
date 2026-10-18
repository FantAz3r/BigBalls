using System.Collections;
using BigBalls.Services;
using UnityEngine;
using VContainer;

public class BossBoneCloudState : BossStateBase
{
    private readonly System.Random _rng = new();
    private SkeletonKing _skeletonKing;
    private WaitForSeconds _shootDelay;
    private WaitForSeconds _volleyDelay;
    private ICoroutineRunner _coroutineRunner;
    private IPlayerProvider _playerProvider;
    private Coroutine _coroutine;

    public BossBoneCloudState (SkeletonKing skeleton, EnemyStateMachine stateMachine, float duration) : base(stateMachine, skeleton, duration)
    {
        _skeletonKing = skeleton;
        _shootDelay = new WaitForSeconds(skeleton.BossConfig.BoneCloudShotDelay);
        _volleyDelay = new WaitForSeconds(skeleton.BossConfig.VolleyDelay);
    }

    [Inject]
    public void Construct (ICoroutineRunner coroutineRunner, IPlayerProvider playerProvider)
    {
        _coroutineRunner = coroutineRunner;
        _playerProvider = playerProvider;
    }

    protected override void OnEnter ()
    {
        _coroutine = _coroutineRunner.StartCoroutine(ShootRoutine());
        // опционально: проиграть анимацию/звук
        //_skeletonKing.EnemyAnimator?.PlayAttack();
        //_skeletonKing.EffectViewer?.Play(_skeletonKing.BossConfig.BoneCloudEffect);
    }

    protected override void OnTick (float dt) { }

    protected override void OnComplete ()
    {
        StateMachine.ChangeState<BossIdleState>();
    }

    private IEnumerator ShootRoutine ()
    {
        var config = _skeletonKing.BossConfig;

        for (int j = 0; j < config.VolleyCount; j++)
        {
            int count = config.MinBoneCount;

            Vector3 toPlayer = _playerProvider.Player.Transform.position - _skeletonKing.Transform.position;
            toPlayer.y = 0f;

            if (toPlayer.sqrMagnitude < 0.0001f)
                toPlayer = _skeletonKing.Transform.forward;
            toPlayer.Normalize();

            float halfSpreadRad = config.BoneCloudSpreadAngle * 0.5f * Mathf.Deg2Rad;
            float baseAngle = Mathf.Atan2(toPlayer.x, toPlayer.z);

            for (int i = 0; i < count; i++)
            {
                float angleOffset = ((float) _rng.NextDouble() * 2f - 1f) * halfSpreadRad;
                float angle = baseAngle + angleOffset;

                Vector3 direction = new Vector3(Mathf.Sin(angle), 0f, Mathf.Cos(angle));

                _skeletonKing.SpawnBoneProjectile(_skeletonKing.BoneSpawnPoint, direction);

                yield return _shootDelay;
            }

            yield return _volleyDelay;
        }

        OnComplete();
    }
}
