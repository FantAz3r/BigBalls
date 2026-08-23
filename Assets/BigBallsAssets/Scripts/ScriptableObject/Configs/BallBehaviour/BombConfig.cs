using System;
using System.Collections.Generic;
using BigBalls.Configs;
using BigBalls.GameplayObjects;
using BigBalls.Localization;
using UnityEngine;
using Math = Utils.Math;

[CreateAssetMenu(fileName = "BombConfig", menuName = "Configs/BahaviourConfig/BombConfig")]
[Serializable]

public class BombConfig : EffectConfig
{
    public ParticleObject ParticleObject;
    public float Radius = 1f;
    public float Damage = 10f;
    public float RespawnDelay = 3f;

    public float RadiusPerLevel = 1f;
    public float DamagePerLevel = 10f;

    public override BehaviourType Type => BehaviourType.Explosion;
    public float GetDamage (int level) => Math.Additive(Damage, DamagePerLevel, level);
    public float GetRadius (int level) => Math.Additive(Radius, RadiusPerLevel, level);

    public override List<ItemStat> GetStats (int level)
    {
        int nextLevel = level + 1;

        return new List<ItemStat>()
            {
                new ItemStat(TextLocalizator.Damage,  GetDamage(level),  GetDamage(nextLevel)),
                new ItemStat(TextLocalizator.Radius, GetRadius(level), GetRadius(nextLevel))
            };
    }
}
