using System;
using UnityEngine;


namespace BigBalls.Configs
{
    [CreateAssetMenu(fileName = "DamageConfig", menuName = "Configs/BahaviourConfig/DamageConfig")]
    [Serializable]

    public class DamageConfig : EffectConfig
    {
        public float Damage;

        public float GetDamage(int level) => Damage + ((level - 1) * Damage);
    }
}