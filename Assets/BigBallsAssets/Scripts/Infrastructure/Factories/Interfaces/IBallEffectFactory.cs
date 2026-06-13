using BigBalls.Configs;
using BigBalls.GameplayObjects;
using System.Collections.Generic;

namespace BigBalls.Factories
{
    public interface IBallEffectFactory
    {
        List<EffectBehaviour> Create(List<EffectConfig> effectConfigs);
    }
}