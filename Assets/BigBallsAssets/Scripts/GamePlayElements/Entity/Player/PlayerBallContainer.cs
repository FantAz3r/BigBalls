using BigBalls.Factories;
using BigBalls.Services;
using BigBalls.StaticData;
using System.Collections.Generic;

namespace BigBalls.GameplayObjects
{
    public class PlayerBallContainer : IBallContainer
    {
        private readonly Stat _ballCount;
        private readonly IBallFactory _ballFactory;
        private readonly BallsData _ballsData;
        private readonly IWeapon _weapon;

        private  Queue<Ball> _balls;
        private int _weaponConfigIndex = 0;
        private int _createdBallsCount = 0; 

        public PlayerBallContainer(Stat ballCount, IResourceLoader resourceLoader, IBallFactory ballFactory, IWeapon weapon = null)
        {
            _ballCount = ballCount;
            _ballFactory = ballFactory;
            _ballsData = resourceLoader.Load<BallsData>();
            _weapon = weapon;

            _balls = new Queue<Ball>((int)ballCount.MaxValue);
        }

        public void Subscribe()
        {
            _ballFactory.BallReturned += ReturmBullet;
        }

        public void Unsubscribe()
        {
            _ballFactory.BallReturned -= ReturmBullet;
        }

        public bool TryGetNextBullet(out Ball ball)
        {
            ball = null;

            if (_balls.Count > 0)
            {
                ball = _balls.Dequeue();
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

        public void ReturmBullet(Ball ball)
        {
            _balls.Enqueue(ball);
        }

        private Ball CreateNewBall()
        {
            BallConfig config = GetNextAvailableConfig();

            if (config == null)
                return null;

            Ball ball = _ballFactory.Create(config);
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