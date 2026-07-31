using BigBalls.Services;
using System;
using System.Collections;
using UnityEngine;
using VContainer;

namespace BigBalls.GameplayObjects
{
    public class EntityTrigger : MonoBehaviour
    {
        [SerializeField] private SphereCollider _sphereCollider;
        [SerializeField] private float _playerDetectionDelay = 2.0f;

        private Coroutine _wallDetectionRoutine;
        private Coroutine _detectionCoroutine;
        private bool _isPlayerInZone = false;
        private bool _canDetectWall = true;
        private WaitForSeconds _waitDelay;
        private ICoroutineRunner _coroutineRunner;

        public event Action WallDetected;
        public event Action PlayerDetected;
        public event Action PlayerStayedLongEnough;


        private void Awake()
        {
            _waitDelay = new WaitForSeconds(_playerDetectionDelay);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<BackWall>(out _))
            {
                if (_canDetectWall)
                    _wallDetectionRoutine = _coroutineRunner.StartCoroutine(WallDetectionDelay());
            }

            if (other.TryGetComponent<Player>(out _))
            {
                _isPlayerInZone = true;
                PlayerDetected?.Invoke();

                _detectionCoroutine = _coroutineRunner.StartCoroutine(PlayerDetectionTimer());
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent<Player>(out _))
            {
                _isPlayerInZone = false;

                if (_detectionCoroutine != null)
                {
                    _coroutineRunner.StopCoroutine(_detectionCoroutine);
                    _detectionCoroutine = null;
                }
            }

            if (other.TryGetComponent<BackWall>(out _))
            {
                if (_wallDetectionRoutine != null)
                    _coroutineRunner.StopCoroutine(_wallDetectionRoutine);
            }
        }

        [Inject]
        public void Construct(ICoroutineRunner coroutineRunner) => _coroutineRunner = coroutineRunner;

        private IEnumerator PlayerDetectionTimer()
        {
            yield return _waitDelay;

            if (_isPlayerInZone)
            {
                PlayerStayedLongEnough?.Invoke();
            }
        }

        private IEnumerator WallDetectionDelay()
        {
            _canDetectWall = false;
            yield return _waitDelay;
            WallDetected?.Invoke();
            _canDetectWall = true;
        }
    }
}