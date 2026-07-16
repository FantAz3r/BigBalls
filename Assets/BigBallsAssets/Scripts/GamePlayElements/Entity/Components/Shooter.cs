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

        private Stat _damage;
        private PlayerAttackService _playerAttackService;
        private bool _canShoot = true;

        public Shooter(
            Stat damage,
            Stat attackSpeed,
            Transform firePoint,
            IBallContainer ballContainer
           )
        {
            _attackDelay = new WaitForSeconds(1 / attackSpeed.CurrentValue);
            _firePoint = firePoint;
            _ballContainer = ballContainer;
            _damage = damage;
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
            _shootRoutine = _coroutineRunner?.StartCoroutine(ShootRoutine());
            _inputService.Attack += HandleShoot;
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

        private IEnumerator ShootRoutine()
        {
            while (_canShoot)
            {
                if (_playerAttackService.IsAutoAttack)
                {
                    yield return _attackDelay;
                    Shoot();
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
                    Shoot();
                }

                _canShoot = true;
            }
        }

        private void Shoot()
        {
            Vector3 forward = _firePoint.forward;

            if (_ballContainer.TryGetNextBullet(out Ball ball))
            {
                ball.Mover.SetDirection(new Vector2(forward.x, forward.z));
                ball.transform.position = _firePoint.position + _fireOffset;
                ball.transform.rotation = _firePoint.rotation;
                Shooted?.Invoke();
            }
        }
    }
}