using BigBalls.Configs;
using BigBalls.GameplayObjects;
using BigBalls.Localization;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GroundConfig", menuName = "Configs/BahaviourConfig/GroundConfig")]
[Serializable]

public class GroundBallConfig : EffectConfig
{
    public ParticleObject EarthShakeParticle;
    public float DamagePerSecond = 5f;
    public float Duration = 3f;
    public float Radius = 1f;

    public float DPSPerLevel = 1f;
    public float DurationPerLevel = 0.3f;
    public override BehaviourType Type => BehaviourType.EarthShake;

    public float GetDamage(int level) => DamagePerSecond + DPSPerLevel * level;
    public float GetDuration(int level) => Duration + DurationPerLevel * level;

    public override List<ItemStat> GetStats(int level)
    {
        int nextLevel = level + 1;

        return new List<ItemStat>()
            {
                new ItemStat(TextLocalizator.Damage,  GetDamage(level),  GetDamage(nextLevel)),
                new ItemStat(TextLocalizator.Duration, GetDuration(level), GetDuration(nextLevel))
            };
    }
}