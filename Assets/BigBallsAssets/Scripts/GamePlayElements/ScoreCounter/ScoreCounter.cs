
using System;
using BigBalls.Factories;
using BigBalls.GameplayObjects;
using BigBalls.Infrastructure;
using BigBalls.Services;
using UnityEngine;

public class ScoreCounter : IUpdateble
{
    private readonly IUpdateService _updateService;
    private IPlayerExperience _experienceModel;
    private IEnemyFactory _enemyFactory;
    private GameConfig _scoreConfig;
    private float _currentScore;
    private float _timeLeft;

    public ScoreCounter (IUpdateService updateService, IPlayerExperience playerExperience, IEnemyFactory enemyFactory, IResourceLoader resourceLoader)
    {
        _updateService = updateService;
        _experienceModel = playerExperience;
        _enemyFactory = enemyFactory;
        _scoreConfig = resourceLoader.Load<GameConfig>();
    }

    public event Action<int> ComboChanged;
    public event Action ComboReseted;
    public event Action<float> ScoreChanged;

    public int Combo { get; private set; }
    public float CurrentScore => _currentScore;


    public void Start (LevelID level)
    {
        _experienceModel.LevelUpped += OnLevelUP;
        _enemyFactory.Killed += OnKill;
        _updateService.Register(this);
    }

    public void Stop ()
    {
        _experienceModel.LevelUpped -= OnLevelUP;
        _enemyFactory.Killed -= OnKill;
    }

    private void OnLevelUP ()
    {
        float levelMultiplier = 1f + Mathf.Log(_experienceModel.Level, 2f) * (_scoreConfig.LevelGrowth - 1f) * 10f;
        Add(_scoreConfig.BaseLevelScore * levelMultiplier);
    }

    private void OnKill (IEntity entity)
    {
        RegisterKill(entity);
        float comboMultiplier = 1f + Mathf.Log(1f + Combo, 2f) * (_scoreConfig.ComboKillBonus - 1f) * 10f;
        float score = _scoreConfig.BaseKillScore * comboMultiplier;
        Add(score);
    }

    public void Reset ()
    {
        Combo = 0;
        _timeLeft = 0f;
        ComboReseted?.Invoke();
    }

    public void Tick ()
    {
        if (Combo == 0)
            return;

        _timeLeft -= Time.deltaTime;

        if (_timeLeft <= 0f)
            Reset();
    }

    private void RegisterKill (IEntity entity)
    {
        _timeLeft = _scoreConfig.ComboWindowSeconds;

        if (_timeLeft > 0f)
            Combo++;
        else
            Combo = 1;

        ComboChanged?.Invoke(Combo);
    }

    private void Add (float score)
    {
        if (score <= 0)
            return;

        _currentScore += score;
        ScoreChanged?.Invoke(_currentScore);
    }

    private void Reduce (float score)
    {
        if (score <= 0)
            return;

        _currentScore -= score;
    }

    private void Multyply (float scoreMultyplier)
    {
        if (scoreMultyplier <= 0)
            return;

        _currentScore *= scoreMultyplier;
    }
}