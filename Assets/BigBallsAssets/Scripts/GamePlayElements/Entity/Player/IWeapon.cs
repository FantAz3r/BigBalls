using BigBalls.StaticData;
using System.Collections.Generic;

namespace BigBalls.GameplayObjects
{
    public interface IWeapon
    {
        List<BallConfig> UniqueBalls { get; }
    }
}