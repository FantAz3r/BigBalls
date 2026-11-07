using System;
using System.Collections.Generic;
using BigBalls.Configs;
using BigBalls.GameplayObjects;
using BigBalls.Localization;
using UnityEngine;

[CreateAssetMenu(fileName = "GoldenConfig", menuName = "Configs/BahaviourConfig/GoldenConfig")]
[Serializable]
public class GoldenConfig : EffectConfig
{
    [Header("Golden mark")]
    public Color Color = new Color(1f, 0.85f, 0.1f);
    public Loot GoldPrefab;
    public int LootPerDeath = 1;
    public float MarkDuration = 5f;

    public int LootPerDeathPerLevel = 1;
    public float MarkDurationPerLevel = 0.5f;

    public override BehaviourType Type => BehaviourType.Golden;

    public float GetMarkDuration (int level)
        => Utils.Math.Additive(MarkDuration, MarkDurationPerLevel, level);

    public int GetBonusGold (int level)
        => System.Math.Max(0, LootPerDeath + LootPerDeathPerLevel * level);

    public override List<ItemStat> GetStats (int level)
    {
        int next = level + 1;
        return new List<ItemStat>()
        {
            new ItemStat(TextLocalizator.Duration, GetMarkDuration(level), GetMarkDuration(next)),
            new ItemStat(TextLocalizator.Gold,    GetBonusGold(level),    GetBonusGold(next)),
        };
    }
}
