using System.Collections.Generic;

namespace BigBalls.Configs
{
    public interface IWeapon : IItem
    {
        List<BallStruct> UniqueBalls { get; }
    }
}