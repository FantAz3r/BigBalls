using BigBalls.GameplayObjects;
using BigBalls.StaticData;
using System.Collections.Generic;

namespace BigBalls.Factories
{
    public interface IBallBehaivorFactory
    {
        List<EffectBehaviour> Create(BallConfig ballConfig);
    }
}