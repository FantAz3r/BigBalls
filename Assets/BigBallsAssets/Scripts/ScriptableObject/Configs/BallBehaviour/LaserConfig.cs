using BigBalls.Configs;
using BigBalls.GameplayObjects;
using BigBalls.Localization;
using System;
using System.Collections.Generic;
using UnityEngine;
using Math = Utils.Math;

[CreateAssetMenu(fileName = "LaserConfig", menuName = "Configs/BahaviourConfig/LaserConfig")]
[Serializable]

public class LaserConfig : EffectConfig
{
    public Color Color;
    public ParticleObject Particle;
    public float LaserDamage = 10f;
    public float LaserDamagePerLevel = 3f;
    public float Offset;
    public int RayCount = 5;
    public float SpreadAngle = 45f;
    public float MaxRange = 100f;
    public override BehaviourType Type => BehaviourType.Laser;
    public float GetDamage(int level) => Math.Additive(LaserDamage, LaserDamagePerLevel, level);

    public override List<ItemStat> GetStats(int level)
    {
        int nextLevel = level + 1;
        return new List<ItemStat>()
        {
            new ItemStat(TextLocalizator.LaserDamage, GetDamage(level), GetDamage(nextLevel))
        };
    }
}

