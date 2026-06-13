using System.Collections.Generic;

namespace BigBalls.Configs
{
    public interface IWeapon : IItem
    {
        List<BallConfig> UniqueBalls { get; }
    }
}