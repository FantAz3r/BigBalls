using System;
using System.Collections.Generic;
using BigBalls.Configs;
using BigBalls.GameplayObjects;
using BigBalls.Localization;
using UnityEngine;
using Math = Utils.Math;

[CreateAssetMenu(fileName = "MagnetConfig", menuName = "Configs/BahaviourConfig/MagnetConfig")]
[Serializable]

public class MagnetConfig : EffectConfig
{
    public float StartAddRange = 1;
    public float AddRangePerLevel = 1;

    public override BehaviourType Type => BehaviourType.Magnet;

    public override List<ItemStat> GetStats (int level)
    {
        int nextLevel = level + 1;

        return new List<ItemStat>()
        {
            new ItemStat(TextLocalizator.Radius, GetRange(level), GetRange(nextLevel)),
        };
    }

    public float GetRange (int level) => Math.Additive(StartAddRange, AddRangePerLevel, level);
}
