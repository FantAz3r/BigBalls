using System;
using System.Collections.Generic;
using BigBalls.Configs;
using BigBalls.GameplayObjects;
using BigBalls.Localization;
using UnityEngine;
using Math = Utils.Math;


[CreateAssetMenu(fileName = "VimpireKnifeConfig", menuName = "Configs/BahaviourConfig/VimpireKnifeConfig")]
[Serializable]

public class VimpireKnifeConfig : EffectConfig
{
    public ParticleObject Particle;
    public int DamagePerShot = 2;
    public int StartHealPerKill = 5;
    public int HealPerKillPerLevel = 1;
    public override BehaviourType Type => BehaviourType.VimpireKnife;

    public float GetHeal (int level) => Math.Additive(StartHealPerKill, HealPerKillPerLevel, level);

    public override List<ItemStat> GetStats (int level)
    {
        int nextLevel = level + 1;

        return new List<ItemStat>()
        {
            new ItemStat(TextLocalizator.Vampirism, GetHeal(level), GetHeal(nextLevel)),
        };
    }
}