using System;
using System.Collections.Generic;
using BigBalls.GameplayObjects;
using BigBalls.Infrastructure.DI;
using BigBalls.Services;
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

        public BallFactory (
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

        public Ball Create (BallModel ballModel, EntityType type)
        {
            Ball ball = _poolService.GetObject<Ball>(ballModel.BallConfig.Prefab.name, _playerProvider.Player.transform.position + new Vector3(0, 0.5f, 0));
            Rigidbody rigidbody = ball.GetComponent<Rigidbody>();

            StatHolder statHolder = new StatHolder(ball, ballModel.BallConfig.EntityType, ballModel.BallConfig);
            statHolder.InitStats();
            MoverPhythics mover = new MoverPhythics(statHolder[StatType.MoveSpeed], ball.transform, _updateService, rigidbody);

            List<ISubscribable> subscribables = new List<ISubscribable>()
            {
                mover
            };

            DeathHandler<Ball> ballDeathHandler = new DeathHandler<Ball>(statHolder[StatType.Health], subscribables, ball);
            ballDeathHandler.Subscribe();

            var collisions = new List<ICollisionStrategy>();

            if (type == EntityType.Player)
            {
                ball.EventHandler.Died += OnReturnToPlayer;
                ball.EventHandler.Returned += OnReturnToPlayer;
                collisions = CreateCollisionStrategies();
            }
            else
            {
                ball.EventHandler.Died += OnReturn;
                ball.EventHandler.Returned += OnReturn;
                collisions = CreateCollisionStrategiesToEnemy();
            }

            ball.Construct(_identifierService.ID, ballModel, mover, collisions, ballDeathHandler, _ballEffectFactory);
            _entityRepository.Add(ball, statHolder, null);
            return ball;
        }

        private List<ICollisionStrategy> CreateCollisionStrategies ()
        {
            List<ICollisionStrategy> collisionStrategies = new List<ICollisionStrategy>
            {
                new BallCollisionStrategy(),
                new PlayerCollisionStrategy(),
                new BackWallCollisionStrategy(_playerProvider.Player),
                new EnemyCollisionStrategy(),
                new ReflectCollisionStrategy()
            };

            return collisionStrategies;
        }

        private List<ICollisionStrategy> CreateCollisionStrategiesToEnemy ()
        {
            List<ICollisionStrategy> collisionStrategies = new List<ICollisionStrategy>
            {
                new BallCollisionStrategy(),
                new HitPlayerStrategy(),
                new WallColllisionStrategy()
            };

            return collisionStrategies;
        }

        private void OnReturnToPlayer (IEntity entity)
        {
            if (entity is not Ball ball)
                return;

            OnReturn(ball);

            ball.EventHandler.Died -= OnReturnToPlayer;
            ball.EventHandler.Returned -= OnReturnToPlayer;
            BallModel model = _ballRepository.AllModels[ball.Config.BallType];
            model.AddItemEXP(ball.AppliedDamage);
            BallReturned?.Invoke(model);
        }

        private void OnReturn (IEntity entity)
        {
            if (entity is not Ball ball)
                return;

            _entityRepository.Remove(ball);
            ball.ClearCollisionStrategies();
            ball.UnsubscribeEffects();
            ball.DeathHandler.Unsubscribe();
            ball.EventHandler.Died -= OnReturn;
            ball.EventHandler.Returned -= OnReturn;
            _poolService.ReleaseObject(ball);
        }
    }
}