using BigBalls.Configs;
using BigBalls.GameplayObjects;
using BigBalls.Saves;
using BigBalls.StaticData;
using System.Collections;
using System.Collections.Generic;

public class ArtefactModel : ItemModel
{
    public ArtefactModel(int id, ArtefactConfig config, int level = 1, float exp = 0) : base(id, config, level, exp)
    {
        ArtefactConfig = config;
    }

    public ArtefactConfig ArtefactConfig { get; private set; }

    private List<EffectBehaviour> _effects = new List<EffectBehaviour>();

    public IEnumerable<EffectBehaviour> Effects => _effects;
    public void SetEffects (List<EffectBehaviour> effects)
    {
        _effects = effects;
    }

    public override CardSaveData CreateSaveData() => new ArtefactSaveData((int)ArtefactConfig.ArtefactType, IsOpen, ItemEXP, NoneGameLevel);

    protected override List<ItemStat> GetStats() => ArtefactConfig.GetStats(Level);

}
