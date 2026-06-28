using BigBalls.Configs;
using BigBalls.Saves;
using BigBalls.StaticData;

public class ArtefactModel : ItemModel
{
    private int _id;
    public ArtefactModel(int id, ArtefactConfig config, int level = 1, float exp = 0) : base(id, config, level, exp)
    {
        ArtefactConfig = config;
        _id = id;
    }
    public ArtefactConfig ArtefactConfig { get; private set; }

    public override CardSaveData CreateSaveData () => new ArtefactSaveData(_id, IsOpen, ItemEXP, Level);
}
