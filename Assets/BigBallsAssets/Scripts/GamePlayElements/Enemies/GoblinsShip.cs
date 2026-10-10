using System.Collections;
using System.Collections.Generic;
using BigBalls.Configs;
using BigBalls.Factories;
using BigBalls.GameplayObjects;
using BigBalls.Services;
using DG.Tweening;
using UnityEngine;
using Utils;
using VContainer;


public class GoblinsShip : Enemy
{
    private const float SpawnOffsetY = 0.5f;

    [SerializeField] private int _minSpawnCount = 1;
    [SerializeField] private int _maxSpawnCount = 2;
    [SerializeField] private float _flightHeight = 5f;
    [SerializeField] private float _projectileSpeed = 3f;
    [SerializeField] private List<EnemyDetector> _enemyDetectors = new List<EnemyDetector>();
    [SerializeField] private List<EnemyConfig> _enemyConfigs = new List<EnemyConfig>();
    [SerializeField] private Transform _model;

    private Stat _spawnTime;
    private Coroutine _spawnCoroutine;
    private IEnemyFactory _enemyFactory;
    private ICoroutineRunner _coroutineRunner;
    private WaitForSeconds _startDelay = new WaitForSeconds(2.5f);
    private bool _canSpawn = true;

    private List<Vector2> _cells = new List<Vector2>();

    [Inject]
    public void Construct (IEnemyFactory enemyFactory, ICoroutineRunner coroutineRunner)
    {
        _enemyFactory = enemyFactory;
        _coroutineRunner = coroutineRunner;
    }

    protected override void ChildInit ()
    {
        _spawnTime = StatHolder[StatType.AttackSpeed];
        _spawnCoroutine = _coroutineRunner.StartCoroutine(SpawnRoutine());
    }


    private void OnDisable ()
    {

        if (_coroutineRunner != null && _spawnCoroutine != null)
            _coroutineRunner.StopCoroutine(_spawnCoroutine);
    }

    private IEnumerator SpawnRoutine ()
    {
        yield return _startDelay;

        while (_canSpawn)
        {
            float elapsed = 0;

            while (elapsed < 1f / _spawnTime.CurrentValue)
            {
                yield return null;
                elapsed += Time.deltaTime;
            }

            SpawnEnemies();
        }
    }

    public void SpawnEnemies ()
    {
        _cells.Clear();

        foreach (var point in _enemyDetectors)
        {
            if (point.HasEnemy == false)
            {
                _cells.Add(new Vector2(point.transform.localPosition.x, point.transform.localPosition.z));
            }
        }

        int countToSpawn = Mathf.Min(Random.Range(_minSpawnCount, _maxSpawnCount + 1), _cells.Count);

        if (countToSpawn == 0)
            return;

        for (int i = 0; i < countToSpawn; i++)
        {
            int randomIndex = Random.Range(0, _cells.Count);
            Vector2 chosenOffset = _cells[randomIndex];
            Vector3 targetPos = new Vector3(transform.position.x + chosenOffset.x, SpawnOffsetY, transform.position.z + chosenOffset.y + _model.transform.localPosition.z);
            Vector3 spawnPos = transform.position + Vector3.up * 2f;


            var config = _enemyConfigs[Random.Range(0, _enemyConfigs.Count)];
            Enemy enemy = _enemyFactory.Create(config, spawnPos);

            Vector3[] path = Math.CalculateParabolaPath(spawnPos, targetPos, _flightHeight);
            float distance = Vector3.Distance(spawnPos, targetPos);
            float duration = distance / _projectileSpeed;

            enemy.transform.DOPath(path, duration, PathType.CatmullRom)
                .SetEase(Ease.Linear);

            _cells.Remove(chosenOffset);
        }
    }
}
