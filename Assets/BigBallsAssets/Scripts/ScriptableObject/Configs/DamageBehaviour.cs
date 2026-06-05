using System;
using UnityEngine;

namespace BigBalls.StaticData
{
    [CreateAssetMenu(fileName = "DamageConfig", menuName = "Configs/BahaviourConfig/DamageConfig")]
    [Serializable]

    public class DamageBehaviour : EffectConfig
    {
        public float Damage;
    }
}