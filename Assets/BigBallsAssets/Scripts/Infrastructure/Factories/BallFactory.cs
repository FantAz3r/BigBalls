using BigBalls.GameplayObjects;
using BigBalls.Infrastructure.DI;
using BigBalls.Services;
using BigBalls.StaticData;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BigBalls.Factories
{
    public class BallFactory : IBallFactory
    {
        private readonly IObjectResolverProvider _objectResolverProvider;
        private readonly IBallEffectFactory _ballEffectFactory;
        private readonly IEntityRepository _entityRepository;
        private readonly IIdentifierService _identifierService;
        private readonly IUpdateService _updateService;
        private readonly IPlayerProvider _playerProvider;
        private readonly IPoolService _poolService;

        public BallFactory(
            IObjectResolverProvider objectResolverProvider,
            IBallEffectFactory ballBehaivorFactory,
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

        public event Action<Ball> BallReturned;

        public Ball Create(BallConfig ballConfig, Transform parent = null)
        {
            int ballId = _identifierService.ID;
            Ball ball = _poolService.GetObject<Ball>(ballConfig.Prefab.name);
            ball.Returned += OnReturn;

            StatHolder statHolder = new StatHolder(ballId, ballConfig);
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

        private void OnReturn(Ball ball)
        {
            _entityRepository.Remove(ball);
            ball.UnsubscribeEffects();
            ball.DeathHandler.Unsubscribe();
            ball.DeathHandler.Died -= OnReturn;
            ball.Returned -= OnReturn;

            _poolService.ReleaseObject(ball);
            BallReturned?.Invoke(ball);
        }
    }
}