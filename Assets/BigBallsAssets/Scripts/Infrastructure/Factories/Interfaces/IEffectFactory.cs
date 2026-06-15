using BigBalls.Configs;
using BigBalls.GameplayObjects;
using System.Collections.Generic;

namespace BigBalls.Factories
{
    public interface IEffectFactory
    {
        List<EffectBehaviour> Create(List<EffectConfig> effectConfigs, int level);
    }
}