using System;
using System.Collections.Generic;
using BigBalls.GameplayObjects;
using BigBalls.Localization;
using UnityEngine;
using Math = Utils.Math;


namespace BigBalls.Configs
{
    [CreateAssetMenu(fileName = "DamageConfig", menuName = "Configs/BahaviourConfig/DamageConfig")]
    [Serializable]

    public class DamageConfig : EffectConfig
    {
        public float Damage;
        public float DamagePerLevel;

        public override BehaviourType Type => BehaviourType.Damage;

        public float GetDamage (int level) => Math.Additive(Damage, DamagePerLevel, level);

        public override List<ItemStat> GetStats(int level)
        {
            int nextLevel = level + 1;

            return new List<ItemStat>()
            {
                new ItemStat(TextLocalizator.Damage, GetDamage(level), GetDamage(nextLevel))
            };
        }
    }
}