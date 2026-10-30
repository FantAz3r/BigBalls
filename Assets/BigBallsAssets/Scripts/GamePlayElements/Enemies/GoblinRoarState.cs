using BigBalls.Services;
using UnityEngine;
using VContainer;

public class GoblinRoarState : BossStateBase
{
    private IPlayerProvider _playerProvider;
    private GoblinKing _goblinKing;
    private SoundSpawner _soundSpawner;
    private DebuffService _debuffService;
    public GoblinRoarState (GoblinKing boss, EnemyStateMachine sm, float duration)
        : base(sm, boss, duration)
    {
        _goblinKing = boss;
    }

    [Inject]
    public void Construct (IPlayerProvider playerProvider, SoundSpawner soundSpawner, DebuffService debuffService)
    {
        _playerProvider = playerProvider;
        _soundSpawner = soundSpawner;
        _debuffService = debuffService;
    }

    protected override void OnEnter ()
    {
        _goblinKing.EnemyAnimator.PlayRoar(TryStunPlayer);
    }

    private void TryStunPlayer ()
    {
        var player = _playerProvider.Player;

        if (player == null)
            return;

        float dist = Vector3.SqrMagnitude(player.Transform.position - _goblinKing.Transform.position);
        float range = _goblinKing.BossConfig.RoarRadius;

        if (dist > range * range)
            return;

        _debuffService.Apply(EffectType.Stun, _playerProvider.Player, _goblinKing.BossConfig.RoarStunDuration);
    }

    protected override void OnTick (float dt) { }

    protected override void OnComplete () => StateMachine.ChangeState<GoblinKingIdleState>();

    protected override void OnExit () => _goblinKing.EnemyAnimator.EnterIdle();
}
