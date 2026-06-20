using System.Collections.Generic;
using System.Linq;
using BigBalls.Saves;
using BigBalls.Services;
using BigBalls.StaticData;
using UnityEngine;

public class BallsRepository : IBallRepository
{
    private readonly GameProgress _gameProgress;
    private readonly BallsData _ballsData;
    private readonly ISaveService _saveService;

    private Dictionary<BallType, CardSaveData> _saveDatas = new();
    private Dictionary<BallType, BallModel> _ballModels = new();
    public BallsRepository (IResourceLoader resourceLoader, ISaveService saveService)
    {
        _gameProgress = saveService.GameProgress;
        _ballsData = resourceLoader.Load<BallsData>();
        _saveService = saveService;

        foreach (var ball in _ballsData.BallConfigs.Values)
        {
            Debug.Log(ball.BallType);
            _saveDatas.Add(ball.BallType, new CardSaveData((int) ball.BallType, false, 0));
        }
    }

    public Dictionary<BallType, BallModel> BallModels => _ballModels;

    public void SaveBallData ()
    {
        _gameProgress.Balls = new List<BallSaveData>();

        foreach (var ballModel in _ballModels.Values)
        {
            BallSaveData ballSave = ballModel.CreateBallSave();
            _gameProgress.Balls.Add(ballSave);
        }

        _saveService.Save(_gameProgress);
    }

    public BallModel GetBallModel (BallType type)
    {
        _ballModels.TryGetValue(type, out var model);
        return model;
    }

    public void AddOrUpdateBallModel (BallModel ballModel) => _ballModels[ballModel.BallConfig.BallType] = ballModel;

    public void LoadBallData ()
    {
        if (_gameProgress.Balls == null || _gameProgress.Balls.Count == 0)
            return;

        foreach (var ballSave in _gameProgress.Balls)
        {
            var ballTypeKey = (BallType) ballSave.BallType;

            _saveDatas[ballTypeKey] = new CardSaveData(
                ballSave.BallType,
                ballSave.IsOpen,
                ballSave.Damage,
                ballSave.Level
            );
        }

        LoadModels();
    }

    private void LoadModels ()
    {
        foreach (var kvp in _saveDatas)
        {
            BallType type = kvp.Key;
            CardSaveData saveData = kvp.Value;

            if (_ballsData.BallConfigs.TryGetValue(type, out var ballConfig))
            {
                var ballModel = new BallModel((int) type, ballConfig, saveData.Level);
                ballModel.InitFromData(saveData);

                _ballModels[type] = ballModel;
            }
        }
    }
}