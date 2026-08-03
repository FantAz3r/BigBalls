using System;
using System.Collections.Generic;
using BigBalls.Configs;
using BigBalls.GameplayObjects;
using BigBalls.Localization;
using UnityEngine;
using Math = Utils.Math;

[CreateAssetMenu(fileName = "TunderConfig", menuName = "Configs/BahaviourConfig/TunderConfig")]
[Serializable]

public class TunderConfig : EffectConfig
{
    public Color Color;
    public int HitCount = 3;
    public float Damage = 3f;
    public float SearchRange = 1.5f;

    public int HitCountPerLevel = 1;
    public float DamagePerLevel = 0.5f;

    public override BehaviourType Type => BehaviourType.Tunder;
    public float GetDamage (int level) => Math.Additive(Damage, DamagePerLevel, level);
    public float GetHitCount (int level) => Math.Additive(HitCount, HitCountPerLevel, level);

    public override List<ItemStat> GetStats (int level)
    {
        int nextLevel = level + 1;

        return new List<ItemStat>()
            {
                new ItemStat(TextLocalizator.Damage,  GetDamage(level),  GetDamage(nextLevel)),
                new ItemStat(TextLocalizator.HitCount, GetHitCount(level), GetHitCount(nextLevel))
            };
    }
}
