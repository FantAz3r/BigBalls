using BigBalls.Configs;
using BigBalls.StaticData;

public class BallModel : ItemModel
{
    public BallConfig BallConfig { get; private set; }

    public BallModel(int id, BallConfig config, int level = 1) : base(id, config, level)
    {
        BallConfig = config;
    }
}
