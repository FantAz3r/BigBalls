using System;
using System.Collections;
using BigBalls.Configs;
using BigBalls.Services;
using UnityEngine;
using VContainer;
using Random = UnityEngine.Random;

namespace BigBalls.GameplayObjects
{
    public class Shooter : ISubscribable
    {
        private const int MaxBallCount = 20;
        private const float MaxDeviationDegrees = 30f;

        private readonly Vector3 _fireOffset = new Vector3(0.5f, 0.5f, 0.5f);
        private readonly IBallContainer _ballContainer;
        private readonly WaitForSeconds _attackDelay;

        private Stat _accuracy;
        private Cannon _cannon;
        private Coroutine _shootRoutine;
        private ICoroutineRunner _coroutineRunner;
        private IInputService _inputService;
        private SoundSpawner _soundSpawner;
        private PlayerAttackService _playerAttackService;
        private bool _canShoot = true;

        public Shooter (
            Stat attackSpeed,
            Stat accuracy,
            Cannon cannon,
            IBallContainer ballContainer)
        {
            _attackDelay = new WaitForSeconds(1 / attackSpeed.CurrentValue);
            _cannon = cannon;
            _ballContainer = ballContainer;
            _accuracy = accuracy;
        }

        public event Action Shooted;

        [Inject]
        public void Construct (ICoroutineRunner coroutineRunner, PlayerAttackService playerAttackService, InputServiceAdapter inputServiceAdapter, SoundSpawner soundSpawner)
        {
            _playerAttackService = playerAttackService;
            _coroutineRunner = coroutineRunner;
            _inputService = inputServiceAdapter.Get();
            _soundSpawner = soundSpawner;
        }

        public void SetCannon (Cannon cannon)
        {
            _cannon = cannon;
        }

        public void Subscribe ()
        {
            _canShoot = true;
            _inputService.Attack += HandleShoot;
        }

        public void StartShoot ()
        {
            _shootRoutine = _coroutineRunner?.StartCoroutine(ShootRoutine());
        }

        public void Unsubscribe ()
        {
            if (_coroutineRunner != null && _coroutineRunner is UnityEngine.Object obj && obj != null && _shootRoutine != null )
            {
                try { _coroutineRunner.StopCoroutine(_shootRoutine); }
                catch (MissingReferenceException) {}
            }

            _shootRoutine = null;
            _canShoot = false;
            _inputService.Attack -= HandleShoot;
        }

        public void Shoot (Ball ball)
        {
            Vector3 forward = _cannon.FirePoint.forward;

            float maxDeviation = Mathf.Lerp(MaxDeviationDegrees, 0f, _accuracy.CurrentValue / _accuracy.MaxValue);
            float yaw = Random.Range(-maxDeviation, maxDeviation);

            Quaternion deviation = Quaternion.Euler(0f, yaw, 0f);
            Vector3 direction = deviation * forward;

            ball.Mover.SetDirection(new Vector2(direction.x, direction.z));
            ball.transform.position = _cannon.FirePoint.position;
            ball.transform.rotation = _cannon.FirePoint.rotation * deviation;

            Shooted?.Invoke();
            _soundSpawner.Spawn(SoundType.ShootBall);

            if (_cannon.Particle != null)
                _cannon.Particle.Play();
        }

        private IEnumerator ShootRoutine (Ball @ball = null)
        {
            while (_canShoot)
            {
                if (_playerAttackService.IsAutoAttack)
                {
                    yield return _attackDelay;

                    if (_ballContainer.TryGetNextBullet(out Ball ballForShoot))
                    {
                        Shoot(ballForShoot);
                    }
                }

                yield return null;
            }
        }

        private void HandleShoot ()
        {
            if (_canShoot && _playerAttackService.IsAutoAttack == false)
            {
                _coroutineRunner.StartCoroutine(ShootAllBalls());
            }
        }

        private IEnumerator ShootAllBalls ()
        {
            _canShoot = false;

            for (int i = 0; i < MaxBallCount; i++)
            {
                if (_ballContainer.TryGetNextBullet(out Ball ball))
                {
                    Shoot(ball);
                    yield return _attackDelay;
                }
            }

            _canShoot = true;
        }
    }
}