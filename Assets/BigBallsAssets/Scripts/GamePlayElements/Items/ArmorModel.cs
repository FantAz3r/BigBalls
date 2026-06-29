using BigBalls.Configs;
using BigBalls.GameplayObjects;
using BigBalls.StaticData;
using System.Collections.Generic;

public class ArmorModel : ItemModel, IArmor
{
    public ArmorModel(int id, ArmorConfig config, int level = 0, float exp = 0) : base(id, config, level, exp)
    {
        ArmorConfig = config;
    }

    public ArmorConfig ArmorConfig { get; private set; }
    public Dictionary<StatType, StatStruct> Stats => ArmorConfig.Stats;
}
