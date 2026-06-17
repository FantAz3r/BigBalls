using System;
using System.Collections;
using BigBalls.Configs;
using BigBalls.Factories;
using BigBalls.GameplayObjects;
using BigBalls.Services;
using UnityEngine;

public class LevelTimeline
{
    private const float UpdateIntervalSeconds = 0.5f;

    private readonly ICoroutineRunner _coroutineRunner;
    private readonly EnemySpawner _enemySpawner;
    private readonly TileFactory _tileFactory;

    private WaitForSeconds _updateInterval = new WaitForSeconds(UpdateIntervalSeconds);
    private WaitForSeconds _lineSpawnDelay;
    private Coroutine _mainCoroutine;
    private Coroutine _timeCoroutine;
    private LevelConfig _levelConfig;
    private bool _isLevelEnded = false;

    public event Action<int> NewWaveStarted;
    public event Action<float> TimeElapsed;

    public LevelTimeline(ICoroutineRunner coroutineRunner, EnemySpawner enemySpawner, TileFactory tileFactory)
    {
        _coroutineRunner = coroutineRunner;
        _enemySpawner = enemySpawner;
        _tileFactory = tileFactory;
    }

    public void Start(LevelConfig levelConfig)
    {
        _lineSpawnDelay = new WaitForSeconds(levelConfig.LineSpawnDelay);
        _levelConfig = levelConfig;
        _enemySpawner.Init(_levelConfig.Enemies);

        if (_mainCoroutine == null)
            _mainCoroutine = _coroutineRunner.StartCoroutine(WaveRoutine());

        if (_timeCoroutine == null)
            _timeCoroutine = _coroutineRunner.StartCoroutine(TimeRoutine());
    }

    public void Stop()
    {
        _isLevelEnded = false;

        if (_mainCoroutine != null)
        {
            _coroutineRunner.StopCoroutine(_mainCoroutine);
            _mainCoroutine = null;
        }

        if (_timeCoroutine != null)
        {
            _coroutineRunner.StopCoroutine(_timeCoroutine);
            _timeCoroutine = null;
        }
    }

    private IEnumerator WaveRoutine()
    {

        
        var waves = _levelConfig.Waves;
        
        Debug.Log(waves.Count);

        for (int i = 0; i < waves.Count; i++)
        {
            Wave wave = waves[i];

            yield return new WaitForSeconds(wave.WaveCooldown);

            NewWaveStarted?.Invoke(i);

            if (wave.BossConfig != null)
                _enemySpawner.SpawnBoss(wave.BossConfig);
            
            for (int line = 0; line < wave.LineCount; line++)
            {
                yield return _lineSpawnDelay;
                _enemySpawner.SpawnLine();
            }

            _enemySpawner.Reset();
            ScaleField();
        }

        yield return new WaitForSeconds(_levelConfig.BossTimeDelay);
        
        
        _enemySpawner.SpawnLevelBoss(_levelConfig.LevelBoss);
        _mainCoroutine = null;
    }


    private void ScaleField()
    {
        _enemySpawner.ScaleField(_tileFactory.ScaleRoad());
    }

    private IEnumerator TimeRoutine()
    {
        float elapsed = 0f;
        TimeElapsed?.Invoke(elapsed);

        while (_isLevelEnded == false)
        {
            yield return _updateInterval;
            elapsed += UpdateIntervalSeconds;
            TimeElapsed?.Invoke(elapsed);
        }
    }
}