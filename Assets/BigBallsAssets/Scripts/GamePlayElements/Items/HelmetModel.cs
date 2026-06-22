using System.Collections.Generic;
using BigBalls.Configs;
using BigBalls.Factories;
using BigBalls.GameplayObjects;
using BigBalls.StaticData;
using VContainer;

public class HelmetModel : ItemModel, IBuffer
{
    public HelmetModel (int id, HelmetConfig config, int level = 1) : base(id, config, level)
    {
        HelmetConfig = config;
    }

    public HelmetConfig HelmetConfig { get; private set; }
    public List<ArtefactModel> Artefacts => HelmetConfig.Artefacts;

}
