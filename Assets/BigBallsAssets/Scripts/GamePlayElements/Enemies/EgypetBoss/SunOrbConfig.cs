using System;
using System.Collections.Generic;
using System.Linq;
using BigBalls.Configs;
using BigBalls.GameplayObjects;
using BigBalls.StaticData;
using UnityEngine;

[CreateAssetMenu(fileName = "SunOrbConfig", menuName = "Configs/SunOrbConfig")]
[Serializable]
public class SunOrbConfig : ScriptableObject, IEntityConfig
{
    [field: SerializeField] public SunOrb Prefab { get; private set; }
    [field: SerializeField] public List<StatStruct> Stats { get; private set; } = new();

    public LayerMask ObstacleLayers => throw new NotImplementedException();

    public StatStruct Get (StatType statType) => Stats.Where(stat => stat.StatType == statType).FirstOrDefault();
}
