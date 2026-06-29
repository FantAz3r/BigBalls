using System.Collections.Generic;

namespace BigBalls.Configs
{
    public interface IWeapon : IItemConfig
    {
        List<BallConfig> UniqueBallConfigs { get; }
    }
}