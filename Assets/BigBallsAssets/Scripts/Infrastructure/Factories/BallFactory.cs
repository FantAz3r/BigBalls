using System;
using System.Collections.Generic;
using BigBalls.Configs;
using BigBalls.GameplayObjects;
using BigBalls.Infrastructure.DI;
using BigBalls.Services;

namespace BigBalls.Factories
{
    public class BallFactory : IBallFactory
    {
        private readonly IObjectResolverProvider _objectResolverProvider;
        private readonly IEffectFactory _ballEffectFactory;
        private readonly IEntityRepository _entityRepository;
        private readonly IIdentifierService _identifierService;
        private readonly IUpdateService _updateService;
        private readonly IPlayerProvider _playerProvider;
        private readonly IPoolService _poolService;

        public BallFactory(
            IObjectResolverProvider objectResolverProvider,
            IEffectFactory ballBehaivorFactory,
            IEntityRepository entityRepository,
            IIdentifierService identifierService,
            IUpdateService updateService,
            IPlayerProvider playerProvider,
            IPoolService poolService)
        {
            _objectResolverProvider = objectResolverProvider;
            _ballEffectFactory = ballBehaivorFactory;
            _entityRepository = entityRepository;
            _identifierService = identifierService;
            _updateService = updateService;
            _playerProvider = playerProvider;
            _poolService = poolService;
        }

        public event Action BallReturned;

        public Ball Create(BallConfig ballConfig, int level = 0)
        {
            int ballId = _identifierService.ID;
            Ball ball = _poolService.GetObject<Ball>(ballConfig.Prefab.name);
            ball.EventHandler.Returned += OnReturn;

            StatHolder statHolder = new StatHolder(ballId, ballConfig.Type, ballConfig);
            Mover mover = new Mover(statHolder[StatType.MoveSpeed], ball.transform, _updateService);

            List<ISubscribable> subscribables = new List<ISubscribable>()
            {
                mover
            };

            DeathHandler<Ball> ballDeathHandler = new DeathHandler<Ball>(statHolder[StatType.Health], subscribables, ball);
            ballDeathHandler.Died += OnReturn;
            ballDeathHandler.Subscribe();

            ball.Construct(ballId, mover, CreateCollisionStrategies(), ballDeathHandler, _ballEffectFactory);
            _entityRepository.Add(ball, statHolder);
            return ball;
        }

        private List<ICollisionStrategy> CreateCollisionStrategies()
        {
            List<ICollisionStrategy> collisionStrategies = new List<ICollisionStrategy>
            {
                new PlayerCollisionStrategy(),
                new BackWallCollisionStrategy(_playerProvider.Player),
                new EnemyCollisionStrategy(),
                new ReflectCollisionStrategy()
            };

            return collisionStrategies;
        }

        private void OnReturn(IEntity entity)
        {
            if (entity is not Ball ball)
                return;

            _entityRepository.Remove(ball);
            ball.UnsubscribeEffects();
            ball.DeathHandler.Unsubscribe();
            ball.DeathHandler.Died -= OnReturn;
            ball.EventHandler.Returned -= OnReturn;

            _poolService.ReleaseObject(ball);
            BallReturned?.Invoke();
        }
    }
}