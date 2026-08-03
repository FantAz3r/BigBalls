using System;
using System.Collections.Generic;
using BigBalls.Configs;
using BigBalls.GameplayObjects;
using BigBalls.Localization;
using UnityEngine;
using Math = Utils.Math;

[CreateAssetMenu(fileName = "DamageConfig", menuName = "Configs/BahaviourConfig/SteelConfig")]
[Serializable]
public class SteelConfig : EffectConfig
{
    public float DamageMultiply;
    public float DamageMultiplyPerLevel;

    public override BehaviourType Type => BehaviourType.Steel;

    public float GetDamageMultiply (int level) => Math.Additive(DamageMultiply, DamageMultiplyPerLevel, level);

    public override List<ItemStat> GetStats (int level)
    {
        int nextLevel = level + 1;

        return new List<ItemStat>()
            {
                new ItemStat(TextLocalizator.DamageMultiply, GetDamageMultiply(level), GetDamageMultiply(nextLevel))
            };
    }
}
