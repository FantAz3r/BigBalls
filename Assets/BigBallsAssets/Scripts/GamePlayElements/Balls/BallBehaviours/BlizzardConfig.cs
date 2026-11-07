using System;
using System.Collections.Generic;
using BigBalls.Configs;
using BigBalls.GameplayObjects;
using BigBalls.Localization;
using UnityEngine;
using Math = Utils.Math;

[CreateAssetMenu(fileName = "BlizzardConfig", menuName = "Configs/BahaviourConfig/BlizzardConfig")]
[Serializable]
public class BlizzardConfig : EffectConfig
{
    public Color FreezeColor = new Color(0.6f, 0.85f, 1f);
    public float StartFreezeDuration = 0.5f;
    public float StartRadius = 2f;

    public float FreezeDurationPerLevel = 0.05f;
    public float RadiusPerLevel = 0.2f;

    public float GetFreezeDuration (int level) => Math.Additive(StartFreezeDuration, FreezeDurationPerLevel, level);
    public float GetRadius (int level) => Math.Additive(StartRadius, RadiusPerLevel, level);

    public override BehaviourType Type => BehaviourType.Blizzard;

    public override List<ItemStat> GetStats (int level)
    {
        int next = level + 1;
        return new List<ItemStat>
        {
            new ItemStat(TextLocalizator.FreezeDuration, GetFreezeDuration(level), GetFreezeDuration(next)),
            new ItemStat(TextLocalizator.Radius, GetRadius(level),GetRadius(next))
        };
    }
}