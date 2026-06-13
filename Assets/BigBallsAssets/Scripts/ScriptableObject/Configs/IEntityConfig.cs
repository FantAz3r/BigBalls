using BigBalls.GameplayObjects;
using BigBalls.StaticData;
using System.Collections.Generic;
using UnityEngine;

namespace BigBalls.Configs
{
    public interface IEntityConfig
    {
        List<StatStruct> Stats { get; }
        LayerMask ObstacleLayers { get; }
        StatStruct Get(StatType statType);
    }
}