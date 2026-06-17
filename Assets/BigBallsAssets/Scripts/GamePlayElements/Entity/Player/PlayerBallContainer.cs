using System.Collections.Generic;
using BigBalls.Configs;
using BigBalls.Factories;
using BigBalls.Services;
using BigBalls.StaticData;

namespace BigBalls.GameplayObjects
{
    public class PlayerBallContainer : IBallContainer, ISubscribable
    {
        private readonly Stat _ballCount;
        private readonly IBallFactory _ballFactory;
        private readonly IEffectFactory _effectFactory;
        private readonly IIdentifierService _identifierService;
        private readonly BallsData _ballsData;
        private readonly BallConfig _baseBallConfig;

        private ItemModel _weapon;
        private ItemModel _buffer;
        private int _currentBallCount = 0;
        private int _createdBallsCount = 0;
        private int _weaponConfigIndex = 0;

        private Queue<BallModel> _balls;

        public PlayerBallContainer (Stat ballCount, IResourceLoader resourceLoader, IBallFactory ballFactory,
            IEffectFactory effectFactory, IIdentifierService identifierService)
        {
            _ballCount = ballCount;
            _ballFactory = ballFactory;
            _effectFactory = effectFactory;
            _identifierService = identifierService;
            _ballsData = resourceLoader.Load<BallsData>();
            _baseBallConfig = _ballsData.BallConfigs[BallType.Base];
            _balls = new Queue<BallModel>((int) ballCount.MaxValue);
        }

        public IEnumerable<BallModel> Balls => _balls;

        public void Set (ItemModel item)
        {
            if (item == null)
                return;

            if (item.Config is IWeapon)
                _weapon = item;

            else if (item.Config is IBuffer)
                _buffer = item;
        }

        public void Subscribe () => _ballFactory.BallReturned += ReturnBullet;
        public void Unsubscribe () => _ballFactory.BallReturned -= ReturnBullet;

        public bool TryGetNextBullet (out Ball ball)
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

        public void ReturnBullet () => _currentBallCount++;

        private Ball CreateNewBall ()
        {
            var ballModel = GetNextAvailableConfig();

            if (ballModel?.Config == null)
                return null;

            var ball = _ballFactory.Create(ballModel);
            ApplyBufferEffects(ball);

            _balls.Enqueue(ballModel);
            return ball;
        }

        private void ApplyBufferEffects (Ball ball)
        {
            if (_buffer?.Config is not IBuffer buffer)
                return;

            var effects = new List<EffectBehaviour>();

            foreach (var artefact in buffer.Artefacts)
            {
                effects.AddRange(_effectFactory.Create(artefact.ArtefactConfig.Effects, _buffer.Level));
            }

            if (effects.Count > 0)
                ball.AddEffects(effects);
        }

        private BallModel GetNextAvailableConfig ()
        {
            if (_weapon?.Config is IWeapon weapon && weapon.UniqueBalls != null && _weaponConfigIndex < weapon.UniqueBalls.Count)
            {
                return weapon.UniqueBalls[_weaponConfigIndex++];
            }

            return new BallModel(_identifierService.ID, _baseBallConfig);
        }

        public bool ReplaceOneBallWithUnique (BallModel uniqueBall)
        {
            if (uniqueBall?.Config == null)
                return false;

            if (_balls.Count == 0)
            {
                _balls.Enqueue(uniqueBall);
                _currentBallCount++;
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
                tempQueue.Enqueue(uniqueBall);
            }

            _balls = tempQueue;
            _currentBallCount = _balls.Count;
            return true;
        }
    }
}