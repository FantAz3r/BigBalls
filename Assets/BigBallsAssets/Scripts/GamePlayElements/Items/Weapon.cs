using BigBalls.StaticData;
using System.Collections.Generic;

namespace BigBalls.GameplayObjects
{
    public class Weapon : IWeapon
    {
        public List<BallConfig> UniqueBalls { get; private set; }
    }
}