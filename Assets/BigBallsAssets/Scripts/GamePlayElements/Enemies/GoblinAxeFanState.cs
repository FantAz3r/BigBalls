using System.Collections;
using BigBalls.Services;
using UnityEngine;
using VContainer;

public class GoblinAxeFanState : BossStateBase
{
    private IPlayerProvider _playerProvider;
    private Coroutine _routine;
    private ICoroutineRunner _coroutineRunner;
    private GoblinKing _goblinKing;

    public GoblinAxeFanState (GoblinKing boss, EnemyStateMachine sm, float duration)
        : base(sm, boss, duration) 
    {
        _goblinKing = boss;
    }

    [Inject]
    public void Construct (IPlayerProvider playerProvider, ICoroutineRunner coroutineRunner)
    {
        _coroutineRunner = coroutineRunner;
        _playerProvider = playerProvider;
    }

    protected override void OnEnter ()
    {
        _goblinKing.EnemyAnimator.Shoot(OnShootFrame);
    }

    private void OnShootFrame ()
    {
        _routine = _coroutineRunner.StartCoroutine(ThrowFan());
    }

    private IEnumerator ThrowFan ()
    {
        var cfg = _goblinKing.BossConfig;
        int count = Mathf.Clamp(cfg.AxeFanCount, 5, 7);

        Vector3 toPlayer = _playerProvider.Player.Transform.position - _goblinKing.Transform.position;
        toPlayer.y = 0f;
        if (toPlayer.sqrMagnitude < 0.0001f) toPlayer = _goblinKing.Transform.forward;
        toPlayer.Normalize();

        float baseAngle = Mathf.Atan2(toPlayer.x, toPlayer.z);
        float halfSpread = cfg.AxeFanSpreadAngle * 0.5f * Mathf.Deg2Rad;

        int center = count / 2; 

        for (int i = 0; i < count; i++)
        {
            float t = (count == 1) ? 0f : (i - center) / (float) center;
            float angle = baseAngle + t * halfSpread;

            Vector3 dir = new Vector3(Mathf.Sin(angle), 0f, Mathf.Cos(angle));
            Vector3 position = new Vector3(_goblinKing.AxeFanSpawnPoint.position.x, 0.65f, _goblinKing.AxeFanSpawnPoint.position.z);
            _goblinKing.SpawnFanAxe(position, dir);

            yield return null;
        }

        yield return null;
        OnComplete();
    }

    protected override void OnTick (float dt) { }

    protected override void OnComplete () => StateMachine.ChangeState<GoblinKingIdleState>();

    protected override void OnExit ()
    {
        if (_routine != null)
            _coroutineRunner.StopCoroutine(_routine);

        _goblinKing.EnemyAnimator.EnterIdle();
    }
}
