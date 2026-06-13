using System.Collections.Generic;

namespace BigBalls.Configs
{
    public interface IBuffer : IItem
    {
        List<EffectConfig> EffectConfigs { get; }
    }
}