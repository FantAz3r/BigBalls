using System;
using System.Collections.Generic;
using BigBalls.GameplayObjects;
using BigBalls.Infrastructure.DI;
using BigBalls.Services;
using log4net.Util;
using UnityEngine;

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
        private readonly BallRepository _ballRepository;

        public BallFactory(
            IObjectResolverProvider objectResolverProvider,
            IEffectFactory ballBehaivorFactory,
            IEntityRepository entityRepository,
            IIdentifierService identifierService,
            IUpdateService updateService,
            IPlayerProvider playerProvider,
            IPoolService poolService,
            BallRepository ballRepository)
        {
            _objectResolverProvider = objectResolverProvider;
            _ballEffectFactory = ballBehaivorFactory;
            _entityRepository = entityRepository;
            _identifierService = identifierService;
            _updateService = updateService;
            _playerProvider = playerProvider;
            _poolService = poolService;
            _ballRepository = ballRepository;
        }

        public event Action<BallModel> BallReturned;

        public Ball Create(BallModel ballModel)
        {
            int ballId = _identifierService.ID;
            Ball ball = _poolService.GetObject<Ball>(ballModel.BallConfig.Prefab.name, _playerProvider.Player.transform.position + new Vector3(0, 0.5f, 0));
            Rigidbody rigidbody = ball.GetComponent<Rigidbody>();
            ball.EventHandler.Returned += OnReturn;

            StatHolder statHolder = new StatHolder(ball, ballModel.BallConfig.EntityType, ballModel.BallConfig);
            statHolder.InitStats();
            MoverPhythics mover = new MoverPhythics(statHolder[StatType.MoveSpeed], ball.transform, _updateService, rigidbody);

            List<ISubscribable> subscribables = new List<ISubscribable>()
            {
                mover
            };

            DeathHandler<Ball> ballDeathHandler = new DeathHandler<Ball>(statHolder[StatType.Health], subscribables, ball);
            ball.EventHandler.Died += OnReturn;
            ballDeathHandler.Subscribe();

            ball.Construct(ballId, ballModel.Level, mover, CreateCollisionStrategies(), ballDeathHandler, _ballEffectFactory);
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
            ball.EventHandler.Died -= OnReturn;
            ball.EventHandler.Returned -= OnReturn;

            BallModel model = _ballRepository.AllModels[ball.Config.BallType];
            model.AddItemEXP(ball.AppliedDamage);

            _poolService.ReleaseObject(ball);
            BallReturned?.Invoke(model);
        }
    }
}