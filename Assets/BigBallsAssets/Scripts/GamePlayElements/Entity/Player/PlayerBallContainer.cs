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
        private readonly BallsData _ballsData;

        private IWeapon _weapon;
        private IBuffer _buffer;

        private Queue<BallConfig> _balls;
        private int _weaponConfigIndex = 0;
        private int _createdBallsCount = 0;

        public PlayerBallContainer(Stat ballCount, IResourceLoader resourceLoader, IBallFactory ballFactory)
        {
            _ballCount = ballCount;
            _ballFactory = ballFactory;
            _ballsData = resourceLoader.Load<BallsData>();

            _balls = new Queue<BallConfig>((int) ballCount.MaxValue);
        }

        public void Set(IWeapon weapon = null) => _weapon = weapon;
        public void Set(IBuffer buffer = null) => _buffer = buffer;

        public void Subscribe() => _ballFactory.BallReturned += ReturnBullet;

        public void Unsubscribe() => _ballFactory.BallReturned -= ReturnBullet;

        public bool TryGetNextBullet(out Ball ball)
        {
            ball = null;

            if (_balls.Count > 0)
            {
                ball = _ballFactory.Create(_balls.Dequeue());
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

        public void ReturnBullet(Ball ball) => _balls.Enqueue(ball.Config);

        private Ball CreateNewBall()
        {
            BallConfig config = GetNextAvailableConfig();

            if (config == null)
            {
                return null;
            }

            Ball ball = _ballFactory.Create(config);

            if(_buffer != null)
            {
                ball.AddEffects(_buffer.EffectConfigs);
            }

            return ball;
        }

        private BallConfig GetNextAvailableConfig()
        {
            if (_weapon != null && _weapon.UniqueBalls != null && _weaponConfigIndex < _weapon.UniqueBalls.Count)
            {
                var config = _weapon.UniqueBalls[_weaponConfigIndex];
                _weaponConfigIndex++;
                return config;
            }

            return _ballsData.BallConfigs[BallType.Base];
        }
    }
}