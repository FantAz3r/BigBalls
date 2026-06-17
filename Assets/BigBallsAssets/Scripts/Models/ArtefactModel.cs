using BigBalls.Configs;
using BigBalls.StaticData;

public class ArtefactModel : ItemModel
{
    public ArtefactConfig ArtefactConfig { get; private set; }

    public ArtefactModel(int id, ArtefactConfig config, int level = 1) : base(id, config, level)
    {
        ArtefactConfig = config;
    }
}
