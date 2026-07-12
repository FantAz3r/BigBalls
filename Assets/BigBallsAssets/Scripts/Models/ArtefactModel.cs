using BigBalls.Configs;
using BigBalls.GameplayObjects;
using BigBalls.Saves;
using BigBalls.StaticData;
using System.Collections.Generic;

public class ArtefactModel : ItemModel
{
    public ArtefactModel(int id, ArtefactConfig config, int level = 1, float exp = 0) : base(id, config, level, exp)
    {
        ArtefactConfig = config;
    }

    public ArtefactConfig ArtefactConfig { get; private set; }

    public override CardSaveData CreateSaveData() => new ArtefactSaveData((int)ArtefactConfig.ArtefactType, IsOpen, ItemEXP, Level);

    protected override List<ItemStat> GetStats() => ArtefactConfig.GetStats(Level);
}
