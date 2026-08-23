using BigBalls.Configs;
using BigBalls.Factories;
using BigBalls.GameplayObjects;
using BigBalls.Saves;
using BigBalls.Services;
using System;
using System.Collections;
using UnityEngine;

public class LevelTimeline
{
    private const float UpdateIntervalSeconds = 0.5f;

    private readonly ICoroutineRunner _coroutineRunner;
    private readonly EnemySpawner _enemySpawner;
    private readonly TileFactory _tileFactory;
    private readonly ISaveService _saveService;
    private readonly IEnemyFactory _enemyFactory;

    private WaitForSeconds _updateInterval = new WaitForSeconds(UpdateIntervalSeconds);
    private WaitForSeconds _lineSpawnDelay;
    private Coroutine _mainCoroutine;
    private Coroutine _timeCoroutine;
    private LevelConfig _levelConfig;
    private bool _isLevelEnded = false;
    private float _levelTimer = 0f;

    public LevelTimeline(
        ICoroutineRunner coroutineRunner,
        EnemySpawner enemySpawner,
        TileFactory tileFactory,
        ISaveService saveService,
        IEnemyFactory enemyFactory)
    {
        _coroutineRunner = coroutineRunner;
        _enemySpawner = enemySpawner;
        _tileFactory = tileFactory;
        _saveService = saveService;
        _enemyFactory = enemyFactory;
    }

    public event Action<int> NewWaveStarted;
    public event Action<float> TimeElapsed;

    public int CompliteWaves { get; private set; }
    public int Kills { get; private set; }

    public void Start(LevelConfig levelConfig)
    {
        _lineSpawnDelay = new WaitForSeconds(levelConfig.LineSpawnDelay);
        _levelConfig = levelConfig;
        _enemyFactory.Died += CountKills;

        if (_mainCoroutine == null)
            _mainCoroutine = _coroutineRunner.StartCoroutine(WaveRoutine());

        if (_timeCoroutine == null)
            _timeCoroutine = _coroutineRunner.StartCoroutine(TimeRoutine());
    }

    public void Stop()
    {
        _isLevelEnded = true;
        _enemyFactory.Died -= CountKills;

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

        var currentLevelId = (int)_levelConfig.Level;
        var newStars = CompliteWaves;
        var newData = new LevelSaveData(currentLevelId, CompliteWaves, _levelTimer, Kills);

        var existingLevelSave = _saveService.GameProgress.Levels
            .Find(l => l.LevelID == currentLevelId); 

        if (existingLevelSave != null)
        {
            if (newStars > existingLevelSave.CompliteWaves)
            {
                int index = _saveService.GameProgress.Levels.IndexOf(existingLevelSave);
                _saveService.GameProgress.Levels[index] = newData;
            }
        }
        else
        {
            _saveService.GameProgress.Levels.Add(newData);
        }

        Kills = 0;
        _levelTimer = 0;
        CompliteWaves = 0;
    }

    private IEnumerator WaveRoutine()
    {
        var waves = _levelConfig.Waves;

        for (int i = 0; i < waves.Count; i++)
        {
            Wave wave = waves[i];
            _enemySpawner.SetEnemies(wave.Enemies);

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

            CompliteWaves++;
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
        _levelTimer = 0f;
        TimeElapsed?.Invoke(_levelTimer);

        while (_isLevelEnded == false)
        {
            yield return _updateInterval;
            _levelTimer += UpdateIntervalSeconds;
            TimeElapsed?.Invoke(_levelTimer);
        }
    }

    private void CountKills(IEntity entity)
    {
        Kills++;
    }
}