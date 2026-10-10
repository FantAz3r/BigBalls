using System.Collections;
using BigBalls.Services;
using UnityEngine;
using VContainer;

public class IceTrailSpawner : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private IceSegment _segmentPrefab;
    [SerializeField] private float _distanceBetweenSegments = 0.5f;
    [SerializeField] private float _segmentScale = 1.5f;
    [SerializeField] private float _segmentLifeTime = 3f;

    private Vector3 _lastSpawnPosition;
    private ICoroutineRunner _coroutineRunner;
    private IPoolService _poolService;
    private Coroutine _coroutine;
    private bool _canSpawn = true;

    [Inject]
    public void Construct (ICoroutineRunner coroutineRunner, IPoolService poolService)
    {
        _poolService = poolService;
        _coroutineRunner = coroutineRunner;

        _lastSpawnPosition = transform.position;
        _coroutine = _coroutineRunner.StartCoroutine(SpawnRoutine());
    }

    private void OnEnable ()
    {
        _canSpawn = true;

        if (_coroutineRunner != null)
        {
            _lastSpawnPosition = transform.position;
            _coroutine = _coroutineRunner.StartCoroutine(SpawnRoutine());
        }
    }

    private void OnDisable ()
    {
        _canSpawn = false;

        if (_coroutineRunner != null)
        {
            _coroutineRunner.StopCoroutine(_coroutine);
        }
    }

    private IEnumerator SpawnRoutine ()
    {
        while (_canSpawn)
        {
            float distanceMoved = Vector3.SqrMagnitude(transform.position - _lastSpawnPosition);

            if (distanceMoved >= _distanceBetweenSegments * _distanceBetweenSegments)
            {
                SpawnSegment();
                _lastSpawnPosition = transform.position;
            }

            yield return null;
        }
    }

    void SpawnSegment ()
    {
        IceSegment segment = _poolService.Get(_segmentPrefab, transform.position, Quaternion.identity);
        segment.Returned += OnSegmentReturned;
        segment.StartScale(_segmentLifeTime);
    }

    private void OnSegmentReturned (IceSegment segment)
    {
        _poolService.Release(segment);
        segment.Returned -= OnSegmentReturned;
    }
}

