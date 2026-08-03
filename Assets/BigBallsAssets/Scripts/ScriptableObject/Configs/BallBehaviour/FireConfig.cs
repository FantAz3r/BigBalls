using BigBalls.GameplayObjects;
using BigBalls.Localization;
using System;
using System.Collections.Generic;
using UnityEngine;
using Math = Utils.Math;

namespace BigBalls.Configs
{
    [CreateAssetMenu(fileName = "FireConfig", menuName = "Configs/BahaviourConfig/FireConfig")]
    [Serializable]

    public class FireConfig : EffectConfig
    {
        public Color Color;
        public ParticleSystem Particle;

        public float BurnDuration = 3f;
        public float BurnDPS = 0.05f;

        public float BurnDurationPerLevel = 0.5f;
        public float BurnDPSPerLevel = 10f;

        public override BehaviourType Type => BehaviourType.Burn;

        public float GetBurnDuration(int level)
            => Math.Additive(BurnDuration, BurnDurationPerLevel, level);

        public float GetBurnDPS(int level)
            => Math.Additive(BurnDPS, BurnDPSPerLevel, level);

        public override List<ItemStat> GetStats(int level)
        {
            int nextLevel = level + 1;
            return new List<ItemStat>()
        {
            new ItemStat(
                TextLocalizator.BurnDuration,
                GetBurnDuration(level),
                GetBurnDuration(nextLevel)
            ),
            new ItemStat(
                TextLocalizator.BurnDPS,
                GetBurnDPS(level),
                GetBurnDPS(nextLevel)
            )
        };
        }
    }
}