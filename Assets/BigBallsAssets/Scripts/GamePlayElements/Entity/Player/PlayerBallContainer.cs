using System.Collections.Generic;
using BigBalls.Configs;
using BigBalls.Factories;
using BigBalls.Services;
using BigBalls.StaticData;
using UnityEngine;

namespace BigBalls.GameplayObjects
{
    public class PlayerBallContainer : IBallContainer, ISubscribable
    {
        private readonly Stat _ballCount;
        private readonly IBallFactory _ballFactory;
        private readonly IEffectFactory _effectFactory;
        private readonly IIdentifierService _identifierService;
        private readonly BallsData _ballsData;

        private ItemModel _weapon;
        private ItemModel _buffer;
        private int _currentBallCount = 0;

        private Queue<BallModel> _balls;
        private int _weaponConfigIndex = 0;
        private int _createdBallsCount = 0;

        public PlayerBallContainer (Stat ballCount, IResourceLoader resourceLoader, IBallFactory ballFactory, IEffectFactory effectFactory, IIdentifierService identifierService)
        {
            _ballCount = ballCount;
            _ballFactory = ballFactory;
            _effectFactory = effectFactory;
            _identifierService = identifierService;
            _ballsData = resourceLoader.Load<BallsData>();

            _balls = new Queue<BallModel>((int) ballCount.MaxValue);
        }

        public IEnumerable<BallModel> Balls => _balls;

        public void Set (ItemModel item = null)
        {
            if (item == null)
                return;

            if (item.Config is IWeapon)
            {
                _weapon = item;
            }
            else if (item.Config is IBuffer)
            {
                _buffer = item;
            }
        }

        public void Subscribe () => _ballFactory.BallReturned += ReturnBullet;

        public void Unsubscribe () => _ballFactory.BallReturned -= ReturnBullet;

        public bool TryGetNextBullet (out Ball ball)
        {
            ball = null;

            if (_currentBallCount > 0 && _balls.Count > 0)
            {
                BallModel ballModel = _balls.Dequeue();

                _balls.Enqueue(ballModel);

                _currentBallCount--;

                ball = _ballFactory.Create(ballModel.Config, ballModel.Level);
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
            BallModel ballModel = GetNextAvailableConfig();

            if (ballModel == null)
                return null;

            if (ballModel.Config == null)
                return null;

            Ball ball = _ballFactory.Create(ballModel.Config);

            if (_buffer != null && _buffer.Config != null)
            {
                IBuffer buffer = _weapon.Config as HelmetConfig;

                List<EffectBehaviour> effects = new();

                foreach (var artefact in buffer.Artefacts)
                {
                    effects.AddRange(_effectFactory.Create(artefact.Config.Effects, _buffer.Level));
                }

                ball.AddEffects(effects);
            }

            _balls.Enqueue(ballModel);

            return ball;
        }

        private BallModel GetNextAvailableConfig ()
        {
            if (_buffer == null)
                return new BallModel(_identifierService.ID, _ballsData.BallConfigs[BallType.Base]);

            if (_buffer.Config is not IWeapon weaponConfig)
                return new BallModel(_identifierService.ID, _ballsData.BallConfigs[BallType.Base]);

            if (weaponConfig != null && weaponConfig.UniqueBalls != null && _weaponConfigIndex < weaponConfig.UniqueBalls.Count)
            {
                var ballStruct = weaponConfig.UniqueBalls[_weaponConfigIndex];
                _weaponConfigIndex++;
                return ballStruct;
            }

            return new BallModel(_identifierService.ID, _ballsData.BallConfigs[BallType.Base]);
        }

        public bool ReplaceOneBallWithUnique (BallModel uniqueBall)
        {
            if (uniqueBall.Config == null)
                return false;

            if (_balls.Count == 0)
            {
                _balls.Enqueue(uniqueBall);
                _currentBallCount++;
                return true;
            }

            var tempQueue = new Queue<BallModel>();
            bool replaced = false;

            while (_balls.Count > 0)
            {
                var ball = _balls.Dequeue();

                if (replaced == false && ball.Config.IsUnique == false)
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
                replaced = true;
            }

            _balls = tempQueue;
            _currentBallCount = _balls.Count;
            return replaced;
        }
    }
}