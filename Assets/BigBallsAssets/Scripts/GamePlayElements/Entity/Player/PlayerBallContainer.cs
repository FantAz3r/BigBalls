using System;
using System.Collections.Generic;
using BigBalls.Configs;
using BigBalls.Factories;
using BigBalls.Services;
using BigBalls.StaticData;

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

        private WeaponModel _weapon;

        private int _currentBallCount = 0;
        private int _createdBallsCount = 0;
        private int _weaponConfigIndex = 0;

        private Queue<BallModel> _balls = new();
        private List<EffectBehaviour> _effectBehaviours = new();

        public PlayerBallContainer (
            Stat ballCount,
            IResourceLoader resourceLoader,
            IBallFactory ballFactory,
            IEffectFactory effectFactory,
            IIdentifierService identifierService,
            BallRepository ballRepository)
        {
            _ballCount = ballCount;
            _ballFactory = ballFactory;
            _effectFactory = effectFactory;
            _identifierService = identifierService;
            _cardsData = resourceLoader.Load<CardsData>();
            _baseBallConfig = _cardsData.Balls[BallType.Base];
        }

        public IEnumerable<BallModel> Balls => _balls;

        public void Subscribe () => _ballFactory.BallReturned += ReturnBullet;
        public void Unsubscribe () => _ballFactory.BallReturned -= ReturnBullet;

        public bool TryGetNextBullet (out Ball ball)
        {
            ball = null;

            if (_createdBallsCount < _ballCount.CurrentValue)
            {
                var ballModel = GetNextBallModel();
                ball = CreateNewBall(ballModel);
                _createdBallsCount++;
                return true;
            }

            if (_currentBallCount > 0 && _balls.Count > 0)
            {
                var ballModel = _balls.Dequeue();
                ball = CreateNewBall(ballModel);
                _currentBallCount--;
                return true;
            }

            return false;
        }

        private BallModel GetNextBallModel ()
        {
            if (_weapon != null && _weapon.UniqueBallConfigs != null && _weaponConfigIndex < _weapon.UniqueBallConfigs.Count)
            {
                return _weapon.UniqueBallModels()[_weaponConfigIndex++];
            }

            return new BallModel(_identifierService.ID, _baseBallConfig);
        }

        public void ReturnBullet (BallModel ballModel)
        {
            _balls.Enqueue(ballModel);
            _currentBallCount++;
        }

        public void AddEffects (List<EffectBehaviour> effects) => _effectBehaviours.AddRange(effects);

        public void AddUniqueBall (WeaponModel weapon)
        {
            _weapon = weapon;

            foreach (var ball in weapon.UniqueBallModels())
            {
                AddUniqueBall(ball);
            }
        }

        public bool AddUniqueBall (BallModel uniqueBall)
        {
            if (uniqueBall?.Config == null)
                return false;

            _balls.Enqueue(uniqueBall);
            _ballCount.AddCurrentValue(1);
            _currentBallCount++;
            return true;
        }

        private Ball CreateNewBall (BallModel ballModel)
        {
            if (ballModel == null)
                throw new ArgumentNullException(nameof(ballModel));

            var ball = _ballFactory.Create(ballModel);

            if (_effectBehaviours.Count > 0)
                ball.AddEffects(_effectBehaviours);

            return ball;
        }
    }
}