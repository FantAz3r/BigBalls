using BigBalls.Configs;
using BigBalls.Saves;
using BigBalls.StaticData;

public class ArtefactModel : ItemModel
{
    public ArtefactConfig ArtefactConfig { get; private set; }
    private int _id;
    public ArtefactModel(int id, ArtefactConfig config, int level = 1) : base(id, config, level)
    {
        ArtefactConfig = config;
        _id = id;
    }

    public override CardSaveData CreateSaveData () => new ArtefactSaveData(_id, IsOpen, ItemEXP, Level);
}
