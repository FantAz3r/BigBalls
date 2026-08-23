using BigBalls.Configs;
using BigBalls.GameplayObjects;
using BigBalls.Saves;
using BigBalls.StaticData;
using System.Collections.Generic;

public class BallModel : ItemModel
{
    public List<EffectBehaviour> _effects = new List<EffectBehaviour>();
    public BallModel(int id, BallConfig config, int level = 1, float exp = 0) : base(id, config, level, exp)
    {
        BallConfig = config;
    }

    public BallConfig BallConfig { get; private set; }

    

    public override CardSaveData CreateSaveData() => new BallSaveData((int)BallConfig.BallType, ItemEXP, IsOpen, NoneGameLevel);
    protected override List<ItemStat> GetStats() => BallConfig.GetStats(Level);
}
