using BigBalls.GameplayObjects;
using System.Collections.Generic;
using UnityEngine;

namespace BigBalls.StaticData
{
    public interface IEntityConfig
    {
        List<StatStruct> Stats { get; }
        LayerMask ObstacleLayers { get; }

        StatStruct Get(StatType statType);
    }
}