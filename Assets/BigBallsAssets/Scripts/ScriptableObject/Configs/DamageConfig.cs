using System;
using BigBalls.GameplayObjects;
using UnityEngine;


namespace BigBalls.Configs
{
    [CreateAssetMenu(fileName = "DamageConfig", menuName = "Configs/BahaviourConfig/DamageConfig")]
    [Serializable]

    public class DamageConfig : EffectConfig
    {
        public float Damage;

        public override BehaviourType Type => BehaviourType.Damage;

        public float GetDamage(int level) => Damage + ((level - 1) * Damage);
    }
}