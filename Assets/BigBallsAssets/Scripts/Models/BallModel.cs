using BigBalls.Configs;
using BigBalls.Saves;
using BigBalls.StaticData;

public class BallModel : ItemModel
{
    public BallModel (int id, BallConfig config, int level = 1, float exp = 0) : base(id, config, level, exp)
    {
        BallConfig = config;
    }

    public override CardSaveData CreateSaveData () => new BallSaveData((int) BallConfig.BallType, ItemEXP, IsOpen, NoneGameLevel);

    public BallConfig BallConfig { get; private set; }
}
