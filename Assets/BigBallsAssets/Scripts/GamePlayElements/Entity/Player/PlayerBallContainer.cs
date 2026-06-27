using BigBalls.Configs;
using BigBalls.Factories;
using BigBalls.Services;
using BigBalls.StaticData;
using System.Collections.Generic;

namespace BigBalls.GameplayObjects
{
    public class PlayerBallContainer : IBallContainer, ISubscribable, IArtefactUser
    {
        private readonly Stat _ballCount;
        private readonly IBallFactory _ballFactory;
        private readonly IEffectFactory _effectFactory;
        private readonly IIdentifierService _identifierService;
        private readonly CardsData _cardsData;
        private readonly BallConfig _baseBallConfig;

        private ItemModel _weapon;

        private int _currentBallCount = 0;
        private int _createdBallsCount = 0;
        private int _weaponConfigIndex = 0;

        private Queue<BallModel> _balls;
        private List<EffectBehaviour> _effectBehaviours = new();

        public PlayerBallContainer(
            Stat ballCount,
            IResourceLoader resourceLoader,
            IBallFactory ballFactory,
            IEffectFactory effectFactory,
            IIdentifierService identifierService,
            BallsRepository ballRepository)
        {
            _ballCount = ballCount;
            _ballFactory = ballFactory;
            _effectFactory = effectFactory;
            _identifierService = identifierService;
            _cardsData = resourceLoader.Load<CardsData>();
            _baseBallConfig = _cardsData.Balls[BallType.Base];

            _balls = new Queue<BallModel>((int)ballCount.MaxValue);
        }

        public IEnumerable<BallModel> Balls => _balls;

        public void Subscribe() => _ballFactory.BallReturned += ReturnBullet;
        public void Unsubscribe() => _ballFactory.BallReturned -= ReturnBullet;

        public bool TryGetNextBullet(out Ball ball)
        {
            ball = null;

            if (_currentBallCount > 0 && _balls.Count > 0)
            {
                var ballModel = _balls.Dequeue();
                _balls.Enqueue(ballModel);
                _currentBallCount--;

                ball = _ballFactory.Create(ballModel);
                return true;
            }

            if (_createdBallsCount < _ballCount.MaxValue)
            {
                ball = CreateNewBall();

                if (ball != null)
                {
                    _createdBallsCount++;
                    return true;
                }
            }

            return false;
        }

        public void ReturnBullet() => _currentBallCount++;

        public void AddEffects(List<EffectBehaviour> effects) => _effectBehaviours.AddRange(effects);

        public void AddUniqueBall(ItemModel item)
        {
            if (item is not IWeapon weapon)
                return;

            foreach (var ball in weapon.UniqueBalls)
                AddUniqueBall(ball);
        }

        public bool AddUniqueBall(BallModel uniqueBall)
        {
            if (uniqueBall?.Config == null)
                return false;

            if (_balls.Count == 0)
            {
                _balls.Enqueue(uniqueBall);
                _currentBallCount = _balls.Count;
                return true;
            }

            var tempQueue = new Queue<BallModel>(_balls.Count + 1);

            bool replaced = false;

            while (_balls.Count > 0)
            {
                var ball = _balls.Dequeue();

                if (replaced == false && ball.BallConfig.IsUnique == false)
                {
                    tempQueue.Enqueue(uniqueBall);
                    replaced = true;
                }
                else
                {
                    tempQueue.Enqueue(ball);
                }
            }

            if (replaced == false)
            {
                _currentBallCount++;
                tempQueue.Enqueue(uniqueBall);
            }

            _balls = tempQueue;
            return true;
        }

        private Ball CreateNewBall()
        {
            var ballModel = GetNextAvailableConfig();

            if (ballModel?.Config == null)
                return null;

            var ball = _ballFactory.Create(ballModel);

            if (_effectBehaviours.Count > 0)
                ball.AddEffects(_effectBehaviours);

            _balls.Enqueue(ballModel);
            return ball;
        }

        private BallModel GetNextAvailableConfig()
        {
            if (_weapon?.Config is IWeapon weapon && weapon.UniqueBalls != null && _weaponConfigIndex < weapon.UniqueBalls.Count)
            {
                return weapon.UniqueBalls[_weaponConfigIndex++];
            }

            return new BallModel(_identifierService.ID, _baseBallConfig);
        }
    }
}