using System;
using System.Collections.Generic;
using BigBalls.Configs;
using BigBalls.Factories;
using BigBalls.Services;
using BigBalls.StaticData;
using UnityEngine;
using VContainer;

namespace BigBalls.GameplayObjects
{
    public class PlayerBallContainer : IBallContainer, ISubscribable, IArtefactUser
    {
        private readonly List<BallModel> _balls = new();
        private readonly Dictionary<BallModel, Ball> _activeBalls = new();

        private readonly List<ArtefactModel> _artefacts = new();
        private readonly HashSet<BallModel> _pendingRemoval = new();

        private IBallFactory _ballFactory;
        private IIdentifierService _identifierService;
        private IPlayerProvider _playerProvider;
        private CardsData _cardsData;
        private BallConfig _baseBallConfig;
        private SoundSpawner _soundSpawner;
        private int _currentBallCount = 0;
        private int _createdBallsCount = 0;

        public PlayerBallContainer (Stat ballCount)
        {
            BallCount = ballCount;
        }

        [Inject]
        public void Construct (
            IResourceLoader resourceLoader,
            IBallFactory ballFactory,
            IIdentifierService identifierService,
            IPlayerProvider playerProvider,
            SoundSpawner soundSpawner)
        {
            _ballFactory = ballFactory;
            _identifierService = identifierService;
            _playerProvider = playerProvider;
            _cardsData = resourceLoader.Load<CardsData>();
            _baseBallConfig = _cardsData.Balls[BallType.Base];
            _soundSpawner = soundSpawner;
        }

        public IEnumerable<BallModel> Balls => _balls;

        public Stat BallCount { get; private set; }

        public void Subscribe () => _ballFactory.BallReturned += OnReturnBullet;

        public void Unsubscribe () => _ballFactory.BallReturned -= OnReturnBullet;

        public void AddEffects (ArtefactModel artefactModel) => _artefacts.Add(artefactModel);

        public void RemoveEffects (ArtefactModel artefactModel) => _artefacts.Remove(artefactModel);

        public bool TryGetNextBullet (out Ball ball)
        {
            ball = null;
            BallModel model;

            if (_createdBallsCount < BallCount.CurrentValue)
            {
                model = new BallModel(_identifierService.ID, _baseBallConfig);
                ball = CreateNewBall(model);
                _createdBallsCount++;
                return true;
            }

            if (_currentBallCount > 0 && _balls.Count > 0)
            {
                int last = _balls.Count - 1;
                model = _balls[last];
                _balls.RemoveAt(last);
                _currentBallCount--;
                ball = CreateNewBall(model);
                return true;
            }

            return false;
        }

        public bool AddUniqueBall (BallModel uniqueBall)
        {
            if (uniqueBall?.Config == null)
                return false;

            _balls.Add(uniqueBall);
            _currentBallCount++;
            return true;
        }

        public bool RemoveUniqueBall (BallModel ball)
        {
            if (ball == null)
                return false;

            if (_balls.Remove(ball))
            {
                _currentBallCount = Mathf.Max(0, _currentBallCount - 1);
                return true;
            }

            if (_activeBalls.Remove(ball))
            {
                _pendingRemoval.Add(ball);
                return true;
            }

            return false;
        }

        private void OnReturnBullet (BallModel ballModel)
        {
            if (ballModel == null)
                return;

            if (_pendingRemoval.Remove(ballModel))
                return;

            _activeBalls.Remove(ballModel);
            _balls.Add(ballModel);
            _currentBallCount++;
        }

        private Ball CreateNewBall (BallModel ballModel)
        {
            if (ballModel == null)
                throw new ArgumentNullException(nameof(ballModel));

            var ball = _ballFactory.Create(
                ballModel,
                EntityType.Player,
                _playerProvider.Player.Transform,
                CreateCollisionStrategies());

            if (_artefacts.Count > 0)
                ball.AddEffects(_artefacts);

            ball.Subscribe();
            ball.EventHandler.Spawn();

            _activeBalls[ballModel] = ball;
            return ball;
        }

        private List<ICollisionStrategy> CreateCollisionStrategies ()
        {
            return new List<ICollisionStrategy>
            {
                new BallCollisionStrategy(),
                new PlayerCollisionStrategy(),
                new BackWallCollisionStrategy(_playerProvider.Player),
                new EnemyCollisionStrategy(),
                new ReflectCollisionStrategy(_soundSpawner)
            };
        }
    }
}