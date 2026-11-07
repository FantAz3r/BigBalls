using System;
using System.Collections.Generic;
using BigBalls.Configs;
using BigBalls.GameplayObjects;
using BigBalls.Localization;
using BigBalls.StaticData;
using UnityEngine;

[CreateAssetMenu(fileName = "SplitConfig", menuName = "Configs/BahaviourConfig/SplitConfig")]
[Serializable]
public class SplitConfig : EffectConfig
{
    public BallType ChildType;

    [Header("Split Settings")]
    public int HitsPerSplit = 5;
    public int BallsPerSplit = 1;

    [Header("Hits thresholds (уменьшение порога)")]
    public int[] HitsThresholdLevels = { 5, 10, 15 };

    [Header("Balls growth (увеличение шаров)")]
    public int BallsGrowthEveryLevels = 2;
    public int MaxBallsPerSplit = 8;

    public override BehaviourType Type => BehaviourType.Split;

    private static int ToHumanLevel (int level) => level + 1;

    public int GetHitsPerSplit (int level)
    {
        int humanLevel = ToHumanLevel(level);
        int reductions = 0;

        if (HitsThresholdLevels != null)
        {
            foreach (int threshold in HitsThresholdLevels)
                if (humanLevel >= threshold)
                    reductions++;
        }

        int value = HitsPerSplit - reductions;
        return System.Math.Max(1, value);
    }

    public int GetBallsPerSplit (int level)
    {
        int humanLevel = ToHumanLevel(level);

        int growth = BallsGrowthEveryLevels > 0
            ? (humanLevel - 1) / BallsGrowthEveryLevels
            : 0;

        int value = BallsPerSplit + growth;
        value = System.Math.Min(value, MaxBallsPerSplit);
        return System.Math.Max(0, value);
    }

    public override List<ItemStat> GetStats (int level)
    {
        int next = level + 1;

        return new List<ItemStat>()
        {
            new ItemStat(TextLocalizator.SplitHits,  GetHitsPerSplit(level),  GetHitsPerSplit(next)),
            new ItemStat(TextLocalizator.SplitBalls, GetBallsPerSplit(level), GetBallsPerSplit(next)),
        };
    }
}