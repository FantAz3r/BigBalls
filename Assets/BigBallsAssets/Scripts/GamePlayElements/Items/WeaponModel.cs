using BigBalls.Configs;
using BigBalls.StaticData;
using System.Collections.Generic;

public class WeaponModel : ItemModel
{
    private readonly BallRepository _ballRepository;

    public WeaponModel(
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

    public List<BallModel> UniqueBallModels()
    {
        List<BallModel> balls = new List<BallModel>();

        foreach (var config in WeaponConfig.UniqueBallConfigs)
        {
            BallModel ballModel = _ballRepository.AllModels[config.BallType];
            ballModel.AddEquipmentLevel(Level);
            balls.Add(ballModel);
        }

        return balls;
    }
}
