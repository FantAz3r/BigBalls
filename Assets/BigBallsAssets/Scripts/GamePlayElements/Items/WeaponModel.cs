using System.Collections.Generic;
using BigBalls.Configs;
using BigBalls.Saves;
using BigBalls.StaticData;

public class WeaponModel : ItemModel
{
    private readonly BallRepository _ballRepository;

    public WeaponModel (
        BallRepository ballRepository,
        int id,
        WeaponConfig config,
        int level = 0,
        float exp = 0)
        : base(id, config, level, exp)
    {
        _ballRepository = ballRepository;
        WeaponConfig = config;
    }

    public WeaponConfig WeaponConfig { get; private set; }
    public List<BallConfig> UniqueBallConfigs => WeaponConfig.UniqueBallConfigs;

    public override CardSaveData CreateSaveData ()
    {
        return new WeaponSaveData((int) WeaponConfig.ItemType, IsOpen, ItemEXP, NoneGameLevel, HasPlayer);
    }

    public List<BallModel> UniqueBallModels ()
    {
        List<BallModel> balls = new List<BallModel>();

        if (WeaponConfig.UniqueBallConfigs.Count == 0)
            return balls;

        foreach (var config in WeaponConfig.UniqueBallConfigs)
        {
            BallModel ballModel = _ballRepository.AllModels[config.BallType];
            balls.Add(ballModel);
        }

        return balls;
    }

    public void AddEquipmentLevel()
    {
        foreach (var config in WeaponConfig.UniqueBallConfigs)
        {
            BallModel ballModel = _ballRepository.AllModels[config.BallType];
            ballModel.SetEquipmentLevel(Level);
        }
    }

    public void RemoveEquipmentLevel ()
    {
        foreach (var config in WeaponConfig.UniqueBallConfigs)
        {
            BallModel ballModel = _ballRepository.AllModels[config.BallType];
            ballModel.SetEquipmentLevel(0);
        }
    }
}
