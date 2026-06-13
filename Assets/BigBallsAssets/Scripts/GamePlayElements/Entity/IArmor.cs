using System.Collections.Generic;
using BigBalls.GameplayObjects;
using BigBalls.StaticData;

namespace BigBalls.Configs
{
    public interface IArmor : IItem
    {
        Dictionary<StatType, StatStruct> Stats { get; }
    }
}