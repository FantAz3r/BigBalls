using BigBalls.Configs;
using BigBalls.Factories;
using BigBalls.Services;
using BigBalls.StaticData;
using System;
using System.Collections.Generic;
using VContainer;

namespace BigBalls.GameplayObjects
{
    public class PlayerBallContainer : IBallContainer, ISubscribable, IArtefactUser
    {
        private readonly Stat _ballCount;
        private IBallFactory _ballFactory;
        private IIdentifierService _identifierService;
        private IPlayerProvider _playerProvider;
        private CardsData _cardsData;
        private BallConfig _baseBallConfig;

        private WeaponModel _weapon;

        private int _currentBallCount = 0;
        private int _createdBallsCount = 0;
        private int _weaponConfigIndex = 0;

        private Queue<BallModel> _balls = new();
        private List<ArtefactModel> _artefacts = new();

        public PlayerBallContainer(Stat ballCount)
        {
            _ballCount = ballCount;
        }

        [Inject]
        public void Construct(
            IResourceLoader resourceLoader,
            IBallFactory ballFactory,
            IIdentifierService identifierService,
            IPlayerProvider playerProvider)
        {
            _ballFactory = ballFactory;
            _identifierService = identifierService;
            _playerProvider = playerProvider;
            _cardsData = resourceLoader.Load<CardsData>();
            _baseBallConfig = _cardsData.Balls[BallType.Base];
        }

        public IEnumerable<BallModel> Balls => _balls;

        public void Subscribe() => _ballFactory.BallReturned += OnReturnBullet;

        public void Unsubscribe() => _ballFactory.BallReturned += OnReturnBullet;

        public void AddEffects(ArtefactModel artefactModel) => _artefacts.Add(artefactModel);

        public bool TryGetNextBullet(out Ball ball)
        {
            ball = null;
            BallModel model = null;

            if (_createdBallsCount < _ballCount.CurrentValue)
            {
                model = GetNextBallModel();
                ball = CreateNewBall(model);
                _createdBallsCount++;
                return true;
            }

            if (_currentBallCount > 0 && _balls.Count > 0)
            {
                model = _balls.Dequeue();
                ball = CreateNewBall(model);
                _currentBallCount--;
                return true;
            }

            return false;
        }

        private BallModel GetNextBallModel()
        {
            if (_weapon != null && _weapon.UniqueBallConfigs != null && _weaponConfigIndex < _weapon.UniqueBallConfigs.Count)
            {
                return _weapon.UniqueBallModels()[_weaponConfigIndex++];
            }

            return new BallModel(_identifierService.ID, _baseBallConfig);
        }

        public void AddUniqueBall(WeaponModel weapon)
        {
            _weapon = weapon;

            foreach (var ball in weapon.UniqueBallModels())
            {
                AddUniqueBall(ball);
            }
        }

        public bool AddUniqueBall(BallModel uniqueBall)
        {
            if (uniqueBall?.Config == null)
                return false;

            _balls.Enqueue(uniqueBall);
            _ballCount.AddCurrentValue(1);
            _currentBallCount++;
            return true;
        }

        private void OnReturnBullet(BallModel ballModel)
        {
            _balls.Enqueue(ballModel);
            _currentBallCount++;
        }

        private Ball CreateNewBall(BallModel ballModel)
        {
            if (ballModel == null)
                throw new ArgumentNullException(nameof(ballModel));

            var ball = _ballFactory.Create(ballModel, EntityType.Player, _playerProvider.Player.Transform);

            if (_artefacts.Count > 0)
                ball.AddEffects(_artefacts);

            ball.Subscribe();
            ball.EventHandler.Spawn();
            return ball;
        }
    }
}