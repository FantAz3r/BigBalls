using System.Collections.Generic;
using BigBalls.GameplayObjects;
using BigBalls.StaticData;

namespace BigBalls.Configs
{
    public interface IArmor : IItemConfig
    {
        Dictionary<StatType, StatStruct> Stats { get; }
    }
}