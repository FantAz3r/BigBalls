using System.Collections;
using BigBalls.Services;
using UnityEngine;

namespace BigBalls.GameplayObjects
{
    public class Shooter : ISubscribable
    {
        private readonly Vector3 _fireOffset = new Vector3(0, 0.5f, 0);
        private readonly Transform _firePoint;
        private readonly IBallContainer _ballContainer;
        private readonly ICoroutineRunner _coroutineRunner;
        private readonly WaitForSeconds _attackDelay;

        private Coroutine _shootRoutine;
        private Stat _damage;
        private bool _canShoot = true;

        public Shooter (Stat damage, Stat attackSpeed, Transform firePoint, IBallContainer ballContainer, ICoroutineRunner coroutineRunner)
        {
            _attackDelay = new WaitForSeconds(1 / attackSpeed.CurrentValue);
            _firePoint = firePoint;
            _ballContainer = ballContainer;
            _coroutineRunner = coroutineRunner;
            _damage = damage;
        }

        public void Subscribe()
        {
            _canShoot = true;
            _shootRoutine = _coroutineRunner?.StartCoroutine(ShootRoutine());
        }

        public void Unsubscribe()
        {
            if(_shootRoutine != null && _coroutineRunner != null)
            {
                _coroutineRunner.StopCoroutine(_shootRoutine);
                _shootRoutine = null;
                _canShoot = false;
            }
        }

        private IEnumerator ShootRoutine()
        {
            while (_canShoot)
            {
                yield return _attackDelay;
                Shoot();
            }
        }

        private void Shoot()
        {
            Vector3 forward = _firePoint.forward;

            if (_ballContainer.TryGetNextBullet(out Ball ball))
            {
                ball.transform.position = _firePoint.position + _fireOffset;
                ball.transform.rotation = _firePoint.rotation;
                ball.Mover.SetDirection(new Vector2(forward.x, forward.z));
            }
        }
    }
}