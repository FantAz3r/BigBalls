using BigBalls.Configs;
using BigBalls.Factories;
using BigBalls.GameplayObjects;
using BigBalls.Infrastructure;
using BigBalls.Services;
using System;
using System.Collections;
using UnityEngine;

public class LevelTimeline
{
    private const int LineSpawnDelay = 5;

    private readonly ICoroutineRunner _coroutineRunner;
    private readonly IResourceLoader _resourceLoader;
    private readonly EnemySpawner _enemySpawner;
    private readonly TileFactory _tileFactory;

    private WaitForSeconds _fiveSrconds = new WaitForSeconds(LineSpawnDelay);
    private Coroutine _timerCoroutine;
    private LevelConfig _levelConfig;

    public event Action Won;

    public LevelTimeline(ICoroutineRunner coroutineRunner, IResourceLoader resourceLoader, EnemySpawner enemySpawner, TileFactory tileFactory)
    {
        _coroutineRunner = coroutineRunner;
        _resourceLoader = resourceLoader;
        _enemySpawner = enemySpawner;
        _tileFactory = tileFactory;
    }

    public void Start(LevelID level)
    {
        _levelConfig = _resourceLoader.Load<LevelData>().Get(level);
        _enemySpawner.Init(_levelConfig.Enemies);
        _timerCoroutine = _coroutineRunner.StartCoroutine(TimerRoutine());
    }

    public void Stop()
    {
        if (_timerCoroutine != null)
        {
            _coroutineRunner.StopCoroutine(_timerCoroutine);
            _timerCoroutine = null;
        }
    }

    private IEnumerator TimerRoutine()
    {
        var waves = _levelConfig.Waves;
        var fiveSeconds = new WaitForSeconds(LineSpawnDelay);

        foreach (var wave in waves)
        {
            yield return new WaitForSeconds(wave.WaveCooldown);

            if (wave.BossConfig != null)
                _enemySpawner.SpawnBoss(wave.BossConfig);


            for (int i = 0; i < wave.LineCount; i++)
            {
                yield return _fiveSrconds;
                _enemySpawner.SpawnLine();
            }

            _enemySpawner.Reset();
            ScaleField();
        }

        yield return new WaitForSeconds(20);

        _enemySpawner.SpawnLevelBoss(_levelConfig.LevelBoss);
    }

    private void ScaleField()
    {
        _enemySpawner.ScaleField(_tileFactory.ScaleRoad());
    }
}