using System;
using BigBalls.Configs;
using BigBalls.GameplayObjects;
using UnityEngine;

namespace BigBalls.StaticData
{
    [CreateAssetMenu(fileName = "IceConfig", menuName = "Configs/BahaviourConfig/IceConfig")]
    [Serializable]

    public class IceConfig : EffectConfig
    {
        public float FreezeDuration = 3;
        public float SlowPercent = 0.1f;

        public override BehaviourType Type => BehaviourType.Freeze;
    }
}