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

        private ItemStruct _weapon;
        private ItemStruct _buffer;
        private int _currentBallCount = 0;

        private Queue<BallStruct> _balls;
        private int _weaponConfigIndex = 0;
        private int _createdBallsCount = 0;

        public PlayerBallContainer(Stat ballCount, IResourceLoader resourceLoader, IBallFactory ballFactory, IEffectFactory effectFactory, IIdentifierService identifierService)
        {
            _ballCount = ballCount;
            _ballFactory = ballFactory;
            _effectFactory = effectFactory;
            _identifierService = identifierService;
            _ballsData = resourceLoader.Load<BallsData>();

            _balls = new Queue<BallStruct>((int) ballCount.MaxValue);
        }

        public void Set(ItemStruct item = default)
        {
            if(item.Config is IWeapon)
            {
                _weapon = item;
            }
            else if(item.Config is IBuffer)
            {
                _buffer = item;
            }
        }

        public void Subscribe() => _ballFactory.BallReturned += ReturnBullet;

        public void Unsubscribe() => _ballFactory.BallReturned -= ReturnBullet;

        public bool TryGetNextBullet(out Ball ball)
        {
            ball = null;

            if (_currentBallCount > 0)
            {
                BallStruct ballStruct = _balls.Dequeue();
                _balls.Enqueue(ballStruct);
                _currentBallCount--;

                ball = _ballFactory.Create(ballStruct.Config, ballStruct.Level);
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

        private Ball CreateNewBall()
        {
           BallStruct ballStruct = GetNextAvailableConfig();

            if (ballStruct.Config == null)
                return null;

            Ball ball = _ballFactory.Create(ballStruct.Config);

            if(_buffer.Config != null)
            {
                IBuffer buffer = _buffer.Config as HelmetConfig;
                var effectConfigs = _effectFactory.Create(buffer.EffectConfigs, _buffer.Level);
                ball.AddEffects(effectConfigs);
            }

            return ball;
        }

        private BallStruct GetNextAvailableConfig()
        {
            IWeapon weaponConfig = _buffer.Config as WeaponConfig;

            if (weaponConfig != null && weaponConfig.UniqueBalls != null && _weaponConfigIndex < weaponConfig.UniqueBalls.Count)
            {
                var ballStruct = weaponConfig.UniqueBalls[_weaponConfigIndex];
                _weaponConfigIndex++;
                return ballStruct;
            }

            return new BallStruct(_identifierService.ID, _ballsData.BallConfigs[BallType.Base]);
        }
    }
}