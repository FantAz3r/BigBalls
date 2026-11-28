using System;
using System.Collections;
using BigBalls.Configs;
using BigBalls.Services;
using UnityEngine;
using VContainer;

namespace BigBalls.GameplayObjects
{
    public class EntityTrigger : MonoBehaviour
    {
        [SerializeField] private SphereCollider _sphereCollider;

        private float _playerDetectionDelay = 2.5f;
        private Coroutine _wallDetectionRoutine;
        private Coroutine _detectionCoroutine;
        private bool _isPlayerInZone = false;
        private bool _canDetectWall = true;
        private WaitForSeconds _waitDelay;
        private ICoroutineRunner _coroutineRunner;
        private SoundSpawner _soundSpawner;
        private Enemy _enemy;

        public event Action WallDetected;
        public event Action PlayerDetected;
        public event Action PlayerLost;
        public event Action PlayerStayedLongEnough;
        public event Action<float> DetectionProgressChanged;

        private void Awake ()
        {
            _waitDelay = new WaitForSeconds(_playerDetectionDelay);
            _enemy = GetComponentInParent<Enemy>();
        }

        private void OnTriggerEnter (Collider other)
        {
            if (other.TryGetComponent<BackWall>(out _))
            {
                if (_canDetectWall)
                    _wallDetectionRoutine = _coroutineRunner.StartCoroutine(WallDetectionDelay());
            }

            if (_enemy.Config.HasMeeleAttack)
            {
                if (other.TryGetComponent<Player>(out _))
                {
                    _isPlayerInZone = true;
                    PlayerDetected?.Invoke();
                    _soundSpawner.Spawn(SoundType.EnemyDetectPlayer);

                    _detectionCoroutine = _coroutineRunner.StartCoroutine(PlayerDetectionTimer());
                }
            }
        }

        private void OnTriggerExit (Collider other)
        {
            if (other.TryGetComponent<Player>(out _))
            {
                PlayerLost?.Invoke();
                _isPlayerInZone = false;

                if (_detectionCoroutine != null)
                {
                    _coroutineRunner.StopCoroutine(_detectionCoroutine);
                    _detectionCoroutine = null;
                }

                DetectionProgressChanged?.Invoke(0f);
            }

            if (other.TryGetComponent<BackWall>(out _))
            {
                if (_wallDetectionRoutine != null)
                    _coroutineRunner.StopCoroutine(_wallDetectionRoutine);
            }
        }

        [Inject]
        public void Construct (ICoroutineRunner coroutineRunner, SoundSpawner soundSpawner)
        {
            _soundSpawner = soundSpawner;
            _coroutineRunner = coroutineRunner;
        }

        private IEnumerator PlayerDetectionTimer ()
        {
            float elapsed = 0f;
            DetectionProgressChanged?.Invoke(0f);

            while (elapsed < _playerDetectionDelay)
            {
                if (!_isPlayerInZone)
                    yield break;

                elapsed += Time.deltaTime;
                DetectionProgressChanged?.Invoke(Mathf.Clamp01(elapsed / _playerDetectionDelay));
                yield return null;
            }

            if (_isPlayerInZone)
            {
                DetectionProgressChanged?.Invoke(1f);
                PlayerStayedLongEnough?.Invoke();
            }
        }

        private IEnumerator WallDetectionDelay ()
        {
            _canDetectWall = false;
            yield return _waitDelay;
            WallDetected?.Invoke();
            _canDetectWall = true;
        }
    }
}