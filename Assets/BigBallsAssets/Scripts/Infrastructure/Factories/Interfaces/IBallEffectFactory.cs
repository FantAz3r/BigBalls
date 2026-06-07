using BigBalls.GameplayObjects;
using BigBalls.StaticData;
using System.Collections.Generic;

namespace BigBalls.Factories
{
    public interface IBallEffectFactory
    {
        List<EffectBehaviour> Create(BallConfig ballConfig);
    }
}