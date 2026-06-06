using BigBalls.GameplayObjects;
using BigBalls.Infrastructure.DI;
using BigBalls.Services;
using BigBalls.StaticData;
using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer.Unity;

namespace BigBalls.Factories
{
    public class BallFactory : IBallFactory
    {
        private readonly IObjectResolverProvider _objectResolverProvider;
        private readonly IBallBehaivorFactory _ballBehaivorFactory;
        private readonly IEntityRepository _entityRepository;
        private readonly IIdentifierService _identifierService;
        private readonly IUpdateService _updateService;

        public BallFactory(
            IObjectResolverProvider objectResolverProvider,
            IBallBehaivorFactory ballBehaivorFactory,
            IEntityRepository entityRepository,
            IIdentifierService identifierService,
            IUpdateService updateService)
        {
            _objectResolverProvider = objectResolverProvider;
            _ballBehaivorFactory = ballBehaivorFactory;
            _entityRepository = entityRepository;
            _identifierService = identifierService;
            _updateService = updateService;
        }

        public event Action<Ball> BallReturned;

        public Ball Create(BallConfig ballConfig, Transform parent = null)
        {
            int ballId = _identifierService.ID;
            Ball ball = _objectResolverProvider.CurrentResolver.Instantiate(ballConfig.Prefab, parent);
            ball.Returned += OnReturn;

            StatHolder statHolder = new StatHolder(ballId, ballConfig);
            Mover mover = new Mover(statHolder[StatType.MoveSpeed], ball.transform, _updateService);

            List<ISubscribable> subscribables = new List<ISubscribable>() { mover };

            DeathHandler ballDeathHandler = new DeathHandler(statHolder[StatType.Health], subscribables, ball.transform);
            ballDeathHandler.Died += OnDied;
            ballDeathHandler.Subscribe();

            ball.Construct(_ballBehaivorFactory, ballId, mover, ballDeathHandler);
            return ball;
        }

        private void OnDied(DeathHandler deathHandler, Transform ballTransform)
        {
            deathHandler.Died -= OnDied;
            deathHandler.Unsubscribe();
            ballTransform.gameObject.SetActive(false);

            ballTransform.TryGetComponent(out Ball ball);
            BallReturned?.Invoke(ball);
        }

        private void OnReturn(Ball ball)
        {
            ball.Returned -= OnReturn;
            ball.gameObject.SetActive(false);
            BallReturned?.Invoke(ball);
        }
    }
}