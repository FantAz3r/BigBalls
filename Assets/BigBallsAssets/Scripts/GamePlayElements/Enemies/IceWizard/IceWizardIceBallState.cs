using System.Collections.Generic;
using BigBalls.Factories;
using BigBalls.GameplayObjects;
using BigBalls.Infrastructure.DI;
using BigBalls.Services;
using BigBalls.StaticData;
using UnityEngine;
using VContainer;

public class IceWizardIceBallState : BossStateBase
{
    private readonly IceWizardBoss _boss;
    private IObjectResolverProvider _resolverProvider;
    private IPlayerProvider _playerProvider;
    private IBallFactory _ballFactory;
    private BallRepository _ballRepository;

    public IceWizardIceBallState (IceWizardBoss boss, EnemyStateMachine sm, float duration)
        : base(sm, boss, duration)
    {
        _boss = boss;
    }

    [Inject]
    public void Construct (IObjectResolverProvider resolverProvider, IPlayerProvider playerProvider, IBallFactory ballFactory, BallRepository ballRepository)
    {
        _resolverProvider = resolverProvider;
        _playerProvider = playerProvider;
        _ballFactory = ballFactory;
        _ballRepository = ballRepository;
    }

    protected override void OnEnter ()
    {
        _boss.EnemyAnimator.Shoot(OnAnimationEvent);
    }

    protected override void OnTick (float dt) { }

    protected override void OnComplete ()
    {
        StateMachine.ChangeState<IceWizardIdleState>();
    }

    protected override void OnExit () { }

    private void OnAnimationEvent()
    {
        var cfg = _boss.BossConfig;

        Vector3 target = _playerProvider.Player.Transform.position;
        target.y = 0f;
        Vector3 origin = _boss.FirePoint.position;
        Vector3 direction = (target - origin);
        direction.y = 0f;

        if (direction.sqrMagnitude < 1e-4f)
            direction = _boss.transform.forward;

        BallModel ballmodel = _ballRepository.AllModels[BallType.Snow];
        Ball ball = _ballFactory.Create(ballmodel, EntityType.Enemy, _boss.FirePoint, GetStrategies(), Quaternion.identity);
        ball.Mover.SetDirection(direction);
        ball.Subscribe();
        ball.EventHandler.Spawn(ball);

    }

    private List<ICollisionStrategy> GetStrategies ()
    {
        List<ICollisionStrategy> collisionStrategies = new List<ICollisionStrategy>()
        {
            new HitPlayerStrategy(),
            new BallCollisionStrategy(),
            new HitTeammateStrategy(),
            new ReflectCollisionStrategy(),
        };

        foreach (var strategy in collisionStrategies)
            _resolverProvider.CurrentResolver.Inject(strategy);

        return collisionStrategies;
    }
}