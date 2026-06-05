using System;
using UnityEngine;

namespace BigBalls.StaticData
{
    [CreateAssetMenu(fileName = "FireConfig", menuName = "Configs/BahaviourConfig/FireConfig")]
    [Serializable]

    public class FireConfig : EffectConfig
    {
        public float BurnDuration = 3f;
        public float DamagePerSecond = 5f;
    }
}