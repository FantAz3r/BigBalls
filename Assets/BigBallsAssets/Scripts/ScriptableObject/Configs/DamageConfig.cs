using BigBalls.GameplayObjects;
using System;
using System.Collections.Generic;
using UnityEngine;
using BigBalls.Localization;


namespace BigBalls.Configs
{
    [CreateAssetMenu(fileName = "DamageConfig", menuName = "Configs/BahaviourConfig/DamageConfig")]
    [Serializable]

    public class DamageConfig : EffectConfig
    {
        public float Damage;

        public override BehaviourType Type => BehaviourType.Damage;

        public float GetDamage(int level) => Damage + ((level - 1) * Damage);

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