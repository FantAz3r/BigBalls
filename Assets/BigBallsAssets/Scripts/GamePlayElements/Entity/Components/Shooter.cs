using BigBalls.Services;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using VContainer;

namespace BigBalls.GameplayObjects
{
    public class Shooter : ISubscribable
    {
        private readonly Vector3 _fireOffset = new Vector3(0, 0.5f, 0);
        private readonly Transform _firePoint;
        private readonly IBallContainer _ballContainer;
        private readonly WaitForSeconds _attackDelay;

        private Coroutine _shootRoutine;
        private ICoroutineRunner _coroutineRunner;
        private IInputService _inputService;

        private PlayerAttackService _playerAttackService;
        private bool _canShoot = true;
        private Transform _target;

        public Shooter(
            Stat attackSpeed,
            Transform firePoint,
            IBallContainer ballContainer,
            Transform target = null)
        {
            _attackDelay = new WaitForSeconds(1 / attackSpeed.CurrentValue);
            _firePoint = firePoint;
            _ballContainer = ballContainer;
            _target = target;
        }

        public event Action Shooted;

        [Inject]
        public void Construct(ICoroutineRunner coroutineRunner, PlayerAttackService playerAttackService, IInputService inputService)
        {
            _playerAttackService = playerAttackService;
            _coroutineRunner = coroutineRunner;
            _inputService = inputService;
        }

        public void Subscribe()
        {
            _canShoot = true;
            _inputService.Attack += HandleShoot;
        }

        public void StartShoot(Ball ball = null)
        {
            _shootRoutine = _coroutineRunner?.StartCoroutine(ShootRoutine(ball));
        }

        public void Unsubscribe()
        {
            if (_shootRoutine != null && _coroutineRunner != null)
            {
                _coroutineRunner.StopCoroutine(_shootRoutine);
                _shootRoutine = null;
                _canShoot = false;
            }

            _inputService.Attack -= HandleShoot;
        }

        public void Shoot(Ball ball)
        {
            Vector3 forward = _firePoint.forward;

            if (_target == null)
            {
                ball.Mover.SetDirection(new Vector2(forward.x, forward.z));
            }
            else
            {
                ball.Mover.SetDirection(new Vector2(_target.position.x, _target.position.z));
            }

            ball.transform.position = _firePoint.position + _fireOffset;
            ball.transform.rotation = _firePoint.rotation;

            Shooted?.Invoke();
        }

        private IEnumerator ShootRoutine(Ball @ball = null)
        {
            while (_canShoot)
            {
                if (@ball == null)
                {
                    if (_playerAttackService.IsAutoAttack)
                    {
                        yield return _attackDelay;

                        if (_ballContainer.TryGetNextBullet(out Ball ballForShoot))
                        {
                            Shoot(ballForShoot);
                        }
                    }
                }
                else
                {
                    Shoot(ball);
                }
            }
        }

        private void HandleShoot()
        {
            if (_canShoot && _playerAttackService.IsAutoAttack == false)
            {
                int ballCount = _ballContainer.Balls.Count();
                _canShoot = false;

                for (int i = 0; i < ballCount; i++)
                {
                    if (_ballContainer.TryGetNextBullet(out Ball ball))
                    {
                        Shoot(ball);
                    }
                }

                _canShoot = true;
            }
        }
    }
}