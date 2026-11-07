using BigBalls.Services;
using UnityEngine;
using VContainer;

public class GoblinBoomerangState : BossStateBase
{
    private IPlayerProvider _playerProvider;
    private GoblinKing _goblinKing;

    public GoblinBoomerangState (GoblinKing boss, EnemyStateMachine sm, float duration)
        : base(sm, boss, duration)
    {
        _goblinKing = boss;
    }

    [Inject]
    public void Construct (IPlayerProvider playerProvider) => _playerProvider = playerProvider;

    protected override void OnEnter ()
    {
        _goblinKing.EnemyAnimator.Attack(OnShootFrame);
    }

    private void OnShootFrame ()
    {
        Vector3 target = _playerProvider.Player.Transform.position;
        target.y = 0f;

        var right = SpawnBoomerang(_goblinKing.LeftHandSpawn, _goblinKing.BoomerangAxeLeft);
        var left = SpawnBoomerang(_goblinKing.RightHandSpawn, _goblinKing.BoomerangAxeRight);

        right.Launch(target, clockwise: true);
        left.Launch(target, clockwise: false);
    }

    private BoomerangAxe SpawnBoomerang (Transform hand, BoomerangAxe axe)
    {
        axe.Setup(
            hand,
            _goblinKing.BossConfig.BoomerangFlightTime,
            _goblinKing.BossConfig.BoomerangReturnTime,
            _goblinKing.BossConfig.BoomerangDamage,
            _goblinKing.BossConfig.BoomerangRadius);
        return axe;
    }

    protected override void OnTick (float dt) { }

    protected override void OnComplete () => StateMachine.ChangeState<GoblinKingIdleState>();

    protected override void OnExit ()
    {
        _goblinKing.EnemyAnimator.EnterIdle();
    }
}
