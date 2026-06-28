using System.Collections.Generic;

namespace BigBalls.Configs
{
    public interface IBuffer : IItemConfig
    {
        List<ArtefactConfig> Artefacts { get; }
    }
}