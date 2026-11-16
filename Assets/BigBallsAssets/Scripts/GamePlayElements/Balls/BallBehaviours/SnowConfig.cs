using System;
using System.Collections.Generic;
using BigBalls.Configs;
using BigBalls.GameplayObjects;
using UnityEngine;

[CreateAssetMenu(fileName = "SnowConfig", menuName = "Configs/BahaviourConfig/SnowConfig")]
[Serializable]
public class SnowConfig : EffectConfig
{
    public Color FreezeColor;
    public float FreezeDuration = 3;
    public float SlowPercent = 0.3f;
    public float ShieldToEnwmy = 15;

    public override BehaviourType Type => BehaviourType.Snow;

    public override List<ItemStat> GetStats (int level)
    {
        return new List<ItemStat>() { };
    }
}
