using System;
using System.Collections.Generic;
using BigBalls.Configs;
using BigBalls.GameplayObjects;
using BigBalls.Localization;
using UnityEngine;
using Math = Utils.Math;

[CreateAssetMenu(fileName = "HornConfig", menuName = "Configs/BahaviourConfig/HornConfig")]
[Serializable]

public class HornConfig : EffectConfig
{
    public float StartDamagePercent = 0.2f;
    public float DamagePercentPerLevel = 0.2f;

    public float GetDamage (int level) => Math.Additive(StartDamagePercent, DamagePercentPerLevel, level);

    public override BehaviourType Type => BehaviourType.Horn;

    public override List<ItemStat> GetStats (int level)
    {
        int nextLevel = level + 1;

        return new List<ItemStat>()
        {
            new ItemStat(TextLocalizator.DamageMultiply, GetDamage(level), GetDamage(nextLevel)),
        };
    }
}
