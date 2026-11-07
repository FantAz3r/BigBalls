using System;
using System.Collections.Generic;
using BigBalls.Configs;
using BigBalls.GameplayObjects;
using BigBalls.Localization;
using UnityEngine;
using Math = Utils.Math;

[CreateAssetMenu(fileName = "LavaConfig", menuName = "Configs/BahaviourConfig/LavaConfig")]
[Serializable]

public class LavaConfig : EffectConfig
{
    public SoundStruct BurnSound;
    public Color Color;
    public float StartBurnDuration;
    public float StartBurnDPS;
    public float StartRadius;

    public float BurnDurationPerLevel;
    public float BurnDPSPerLevel;
    public float RadiusPerLevel;

    public float GetBurnDuration (int level) => Math.Additive(StartBurnDuration, BurnDurationPerLevel, level);
    public float GetBurnDPS (int level) => Math.Additive(StartBurnDPS, BurnDPSPerLevel, level);
    public float GetRadius (int level) => Math.Additive(StartRadius, RadiusPerLevel, level);
    public override BehaviourType Type => BehaviourType.Lava;

    public override List<ItemStat> GetStats (int level)
    {
        int nextLevel = level + 1;

        return new List<ItemStat>()
            {
                new ItemStat(TextLocalizator.BurnDuration, GetBurnDuration(level), GetBurnDuration(nextLevel)),
                new ItemStat(TextLocalizator.BurnDPS, GetBurnDPS(level), GetBurnDPS(nextLevel)),
                new ItemStat(TextLocalizator.Radius, GetRadius(level), GetRadius(nextLevel))
            };
    }
}
