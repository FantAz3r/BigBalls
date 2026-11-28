using System.Collections;
using System.Collections.Generic;
using BigBalls.Factories;
using BigBalls.GameplayObjects;
using BigBalls.Services;
using UnityEngine;
using VContainer;

public class IceYeti : Enemy
{
    [SerializeField] private IceWall _prefab;
    [SerializeField] private int _spawnCount;
    [SerializeField] private int _spawnZoneWidth = 7;
    [SerializeField] private int _spawnZoneDepth = 13;
    [SerializeField] private float _spacing = 1.5f;
    [SerializeField] private Vector3 _offset = new Vector3(0, 0, -2);

    [Header("Wall attack")]
    [SerializeField] private float _minAttackDelay = 5f;
    [SerializeField] private float _maxAttackDelay = 7f;
    [SerializeField] private float _wallStopDistance = 1.5f;
    [SerializeField] private float _destroyDelay = 0.5f;
    [SerializeField] private float _returnStopDistance = 0.1f;

    private List<IceWall> _iceWalls = new List<IceWall>();
    private IPoolService _poolService;
    private ICoroutineRunner _coroutineRunner;
    private Coroutine _coroutine;
    private bool _canAttack;
    private IceWall _currentWall = null;

    private void OnDisable ()
    {
        foreach (var wall in _iceWalls)
        {
            _poolService.Release(wall);
        }

        _canAttack = false;

        if (_coroutineRunner != null && _coroutine != null)
            _coroutineRunner.StopCoroutine(_coroutine);

        _iceWalls.Clear();
    }

    [Inject]
    public void Construct (IPoolService poolService, ICoroutineRunner coroutineRunner)
    {
        _poolService = poolService;
        _coroutineRunner = coroutineRunner;
    }

    protected override void ChildInit ()
    {
        _coroutine = _coroutineRunner.StartCoroutine(WallAttackRoutine());
    }

    private void SpawnWalls ()
    {
        List<Vector2Int> possiblePositions = new List<Vector2Int>();

        for (int x = 0; x < _spawnZoneWidth; x++)
        {
            for (int z = 0; z < _spawnZoneDepth; z++)
            {
                possiblePositions.Add(new Vector2Int(x, z));
            }
        }

        for (int i = 0; i < possiblePositions.Count; i++)
        {
            Vector2Int temp = possiblePositions[i];
            int randomIndex = Random.Range(i, possiblePositions.Count);
            possiblePositions[i] = possiblePositions[randomIndex];
            possiblePositions[randomIndex] = temp;
        }

        int actualToSpawn = Mathf.Min(_spawnCount, possiblePositions.Count);

        for (int i = 0; i < actualToSpawn; i++)
        {
            Vector2Int pos = possiblePositions[i];

            IceWall iceWall = _poolService.Get(_prefab);
            _iceWalls.Add(iceWall);
            Vector3 position = new Vector3(
                pos.x * _spacing - (_spawnZoneWidth * _spacing) / 2f,
                0.75f,
                pos.y * _spacing - (_spawnZoneDepth * _spacing) / 2f
            );

            iceWall.transform.position = position + _offset;
            float rotationY = Random.value > 0.5f ? 180f : 90f;

            if (rotationY == 90)
            {
                rotationY = Random.value > 0.5f ? 90 : 270;
            }

            iceWall.transform.rotation = Quaternion.Euler(0, rotationY, 0);
            iceWall.Play();
        }
    }

    private IEnumerator WallAttackRoutine ()
    {
        EnemyAnimator.Shoot(SpawnWalls);
        _canAttack = true;

        while (isActiveAndEnabled)
        {
            Rotator.SetDirection(Vector2.down);
            float delay = Random.Range(_minAttackDelay, _maxAttackDelay);

            IceWall targetWall = FindNearestWall();

            if(targetWall)
                Rotator.SetTarget(targetWall.transform);

            yield return new WaitForSeconds(delay);

            if (targetWall == null)
            {
                EnemyAnimator.Shoot(SpawnWalls);
                yield return new WaitForSeconds(delay);
                continue;
            }

            _currentWall = targetWall;
            Rotator.SetTarget(targetWall.transform);
            EnemyAnimator.Jump(DestroyWall);
        }
    }

    private IceWall FindNearestWall ()
    {
        IceWall nearestWall = null;
        float nearestDistanceSqr = float.MaxValue;

        for (int i = _iceWalls.Count - 1; i >= 0; i--)
        {
            IceWall wall = _iceWalls[i];

            if (wall == null)
            {
                _iceWalls.RemoveAt(i);
                continue;
            }

            float distanceSqr = (wall.transform.position - transform.position).sqrMagnitude;

            if (distanceSqr < nearestDistanceSqr)
            {
                nearestDistanceSqr = distanceSqr;
                nearestWall = wall;
            }
        }

        return nearestWall;
    }

    private void DestroyWall ()
    {
        _currentWall.BreakWall();
        _poolService.Release(_currentWall);
        _iceWalls.Remove(_currentWall);
    }
}
