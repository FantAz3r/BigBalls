using System;
using System.Collections;
using BigBalls.Configs;
using BigBalls.Factories;
using BigBalls.GameplayObjects;
using BigBalls.Saves;
using BigBalls.Services;
using UnityEngine;

public class LevelTimeline : IWinReason
{
    private const float UpdateIntervalSeconds = 0.5f;
    private const float ReinforcementCooldown = 2f;

    private readonly ICoroutineRunner _coroutineRunner;
    private readonly EnemySpawner _enemySpawner;
    private readonly TileFactory _tileFactory;
    private readonly ISaveService _saveService;
    private readonly IEnemyFactory _enemyFactory;
    private readonly SoundSpawner _soundSpawner;
    private readonly ScoreCounter _scoreCounter;
    private readonly ILiderboardService _liderboardService;
    private readonly IWinService _winService;

    private GameConfig _gameConfig;
    private WaitForSeconds _updateInterval = new WaitForSeconds(UpdateIntervalSeconds);
    private WaitForSeconds _lineSpawnDelay;
    private Coroutine _mainCoroutine;
    private Coroutine _timeCoroutine;
    private LevelConfig _levelConfig;

    private bool _isLevelEnded = false;
    private bool _canReinforce;
    private bool _hasLevelBoss = false;
    private bool _isLevelStarted = false;
    private float _levelTimer = 0f;
    private Wave _currentWave;
    private int _currentLine;
    private float _lastReinforcementTime = -999f;

    public LevelTimeline (
        ICoroutineRunner coroutineRunner,
        EnemySpawner enemySpawner,
        TileFactory tileFactory,
        ISaveService saveService,
        IEnemyFactory enemyFactory,
        SoundSpawner soundSpawner,
        ScoreCounter scoreCounter,
        ILiderboardService liderboardService,
        IResourceLoader resourceLoader,
        IWinService winService)
    {
        _coroutineRunner = coroutineRunner;
        _enemySpawner = enemySpawner;
        _tileFactory = tileFactory;
        _saveService = saveService;
        _enemyFactory = enemyFactory;
        _soundSpawner = soundSpawner;
        _scoreCounter = scoreCounter;
        _liderboardService = liderboardService;
        _winService = winService;
        _gameConfig = resourceLoader.Load<GameConfig>();
    }

    public event Action<int> NewWaveStarted;
    public event Action<float> TimeElapsed;
    public event Action Won;

    public int CompliteWaves { get; private set; }
    public int Kills { get; private set; }
    public int Stars { get; private set; }

    public void Start (LevelConfig levelConfig)
    {
        _lineSpawnDelay = new WaitForSeconds(levelConfig.LineSpawnDelay);
        _isLevelStarted = true;
        _levelConfig = levelConfig;
        _enemyFactory.Died += CountKills;
        _enemyFactory.BossKilled += BossKillCount;

        if (_mainCoroutine == null)
            _mainCoroutine = _coroutineRunner.StartCoroutine(WaveRoutine());

        if (_timeCoroutine == null)
            _timeCoroutine = _coroutineRunner.StartCoroutine(TimeRoutine());

        if (_levelConfig.LevelBoss != null && _levelConfig.LevelBoss.Prefab != null)
            _hasLevelBoss = true;
        else
        {
            _hasLevelBoss = false;
            _winService.SetWinReason(this);
        }
    }

    public void Stop ()
    {
        if (_isLevelStarted == false)
            return;

        _isLevelEnded = true;
        _enemyFactory.Died -= CountKills;
        _enemyFactory.BossKilled -= BossKillCount;

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

        SaveLevel();

        Kills = 0;
        _levelTimer = 0;
        CompliteWaves = 0;
        Stars = 0;
    }

    private void SaveLevel ()
    {
        var currentLevelId = (int) _levelConfig.Level;
        int currentScore = (int) _scoreCounter.CurrentScore;
        var newData = new LevelSaveData(currentLevelId, Stars, currentScore, _levelTimer, Kills);
        var existingLevelSave = _saveService.GameProgress.Levels.Find(l => l.LevelID == currentLevelId);

        bool isNewRecord = false;

        if (existingLevelSave != null)
        {
            if (currentScore > existingLevelSave.Score)
            {
                int index = _saveService.GameProgress.Levels.IndexOf(existingLevelSave);
                _saveService.GameProgress.Levels[index] = newData;
                isNewRecord = true;
            }
        }
        else
        {
            _saveService.GameProgress.Levels.Add(newData);
            isNewRecord = true;
        }

        if (isNewRecord)
        {
            CalculateGlobalScores();
        }
    }

    private void CalculateGlobalScores ()
    {
        int totalScore = 0;

        foreach (var levelData in _saveService.GameProgress.Levels)
        {
            totalScore += levelData.Score;
        }

        _liderboardService.SubmitScore(_gameConfig.MainLiderboard, totalScore);
    }

    private IEnumerator WaveRoutine ()
    {
        var waves = _levelConfig.Waves;

        for (int i = 0; i < waves.Count; i++)
        {
            Wave wave = waves[i];
            _currentWave = wave;
            _enemySpawner.SetEnemies(wave.Enemies);

            yield return new WaitForSeconds(wave.WaveCooldown);
            NewWaveStarted?.Invoke(i);

            if (wave.BossConfig != null)
                _enemySpawner.SpawnBoss(wave.BossConfig);

            for (int line = 0; line < wave.LineCount; line++)
            {
                _currentLine = line;
                yield return _lineSpawnDelay;

                _enemySpawner.SpawnLine();
                _soundSpawner.Spawn(SoundType.SpawnEnemyLine);

                if (line == 0)
                    _canReinforce = true;

                if (line == wave.LineCount - 2)
                    _canReinforce = false;
            }

            _enemySpawner.Reset();
            ScaleField();
            CompliteWaves++;
        }

        yield return new WaitForSeconds(_levelConfig.BossTimeDelay);

        if (_hasLevelBoss)
            _enemySpawner.SpawnLevelBoss(_levelConfig.LevelBoss);

        _mainCoroutine = null;
    }

    private void ScaleField ()
    {
        _enemySpawner.ScaleField(_tileFactory.ScaleRoad());
        _soundSpawner.Spawn(SoundType.FieldExpand);
    }

    private IEnumerator TimeRoutine ()
    {
        _levelTimer = 0f;
        TimeElapsed?.Invoke(_levelTimer);

        while (_isLevelEnded == false)
        {
            yield return _updateInterval;
            _levelTimer += UpdateIntervalSeconds;
            TimeElapsed?.Invoke(_levelTimer);

            TryReinforce();
        }
    }

    private void CountKills (IEntity entity)
    {
        Kills++;
    }

    private void BossKillCount ()
    {
        Stars++;

        if (Stars == _levelConfig.Waves.Count - 1)
        {
            Won?.Invoke();
        }
    }

    private void TryReinforce ()
    {
        if (_isLevelEnded || _currentWave.Enemies == null)
            return;

        if (_currentLine >= _currentWave.LineCount - 1)
            return;

        if (_enemyFactory.HasAliveEnemies)
            return;

        if (Time.time - _lastReinforcementTime < ReinforcementCooldown)
            return;

        if (_canReinforce == false)
            return;

        int count = _levelConfig.ReinforcementCount;

        if (count <= 0)
            return;

        Debug.Log("reinforce");
        _enemySpawner.SpawnReinforcement(_currentWave.Enemies, count);
        _lastReinforcementTime = Time.time;
    }
}