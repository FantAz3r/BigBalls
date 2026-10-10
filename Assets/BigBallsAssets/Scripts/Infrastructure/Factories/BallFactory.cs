using System;
using System.Collections.Generic;
using BigBalls.GameplayObjects;
using BigBalls.Infrastructure.DI;
using BigBalls.Services;
using UnityEngine;
using VContainer;

namespace BigBalls.Factories
{
    public class BallFactory : IBallFactory
    {
        private readonly IObjectResolver _objectResolver;
        private readonly IEffectFactory _effectFactory;
        private readonly IEntityRepository _entityRepository;
        private readonly IIdentifierService _identifierService;
        private readonly IPlayerProvider _playerProvider;
        private readonly IPoolService _poolService;
        private readonly BallRepository _ballRepository;

        public BallFactory (
            IObjectResolverProvider objectResolverProvider,
            IEffectFactory ballBehaivorFactory,
            IEntityRepository entityRepository,
            IIdentifierService identifierService,
            IPlayerProvider playerProvider,
            IPoolService poolService,
            BallRepository ballRepository)
        {
            _objectResolver = objectResolverProvider.CurrentResolver;
            _effectFactory = ballBehaivorFactory;
            _entityRepository = entityRepository;
            _identifierService = identifierService;
            _playerProvider = playerProvider;
            _poolService = poolService;
            _ballRepository = ballRepository;
        }

        public event Action<BallModel> BallReturned;

        public Ball Create (BallModel ballModel, EntityType type, Transform shooter, List<ICollisionStrategy> strategies, Quaternion rotation)
        {
            Ball ball = _poolService.Get(ballModel.BallConfig.Prefab, shooter.position + new Vector3(0, 0.5f, 0), rotation);
            Rigidbody rigidbody = ball.GetComponent<Rigidbody>();

            StatHolder statHolder = new StatHolder(ball, ballModel.BallConfig.EntityType, ballModel.BallConfig);
            statHolder.InitStats();
            MoverPhythics mover = new MoverPhythics(statHolder[StatType.MoveSpeed], ball.transform, rigidbody);
            _objectResolver.Inject(mover);

            List<ISubscribable> subscribables = new List<ISubscribable>()
            {
                mover
            };

            DeathHandler<Ball> ballDeathHandler = new DeathHandler<Ball>(statHolder[StatType.Health], subscribables, ball);
            ballDeathHandler.Subscribe();

            var collisions = new List<ICollisionStrategy>(strategies);

            if (type == EntityType.Player)
            {
                ball.EventHandler.Died += OnReturnToPlayer;
                ball.EventHandler.Returned += OnReturnToPlayer;
            }
            else
            {
                ball.EventHandler.Died += OnReturn;
                ball.EventHandler.Returned += OnReturn;
            }

            ball.Construct(_identifierService.ID, ballModel, mover, collisions, ballDeathHandler, _effectFactory);
            _entityRepository.Add(ball, statHolder, null);
            return ball;
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
            _poolService.Release(ball);

            ball.transform.position = new Vector3(1000, 1000, 1000);
        }
    }
}