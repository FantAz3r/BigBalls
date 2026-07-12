using BigBalls.Configs;
using BigBalls.GameplayObjects;
using BigBalls.Localization;
using System;
using System.Collections.Generic;
using UnityEngine;
using Math = Utils.Math;

namespace BigBalls.StaticData
{
    [CreateAssetMenu(fileName = "IceConfig", menuName = "Configs/BahaviourConfig/IceConfig")]
    [Serializable]

    public class IceConfig : EffectConfig
    {
        public float FreezeDuration = 3;
        public float SlowPercent = 0.2f;

        public float FreezeDurationPerLevel = 1;
        public float SlowPercentPerLevel = 0.05f;

        public override BehaviourType Type => BehaviourType.Freeze;

        public float GetFreezeDuration(int level) => Math.Additive(FreezeDuration, FreezeDurationPerLevel, level);
        public float GetSlowPercent(int level) => Math.Additive(FreezeDuration, FreezeDurationPerLevel, level);

        public override List<ItemStat> GetStats(int level)
        {
            int nextLevel = level + 1;

            return new List<ItemStat>()
            {
                new ItemStat(TextLocalizator.FreezeDuration, GetFreezeDuration(level), GetFreezeDuration(nextLevel)),
                new ItemStat(TextLocalizator.SlowPercent, GetSlowPercent(level), GetSlowPercent(nextLevel))
            };
        }
    }
}