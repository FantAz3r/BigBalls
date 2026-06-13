using BigBalls.Configs;
using BigBalls.GameplayObjects;
using System.Collections.Generic;
using UnityEngine;

namespace BigBalls.StaticData
{
    [CreateAssetMenu(menuName = "Datas/BehaiviourData")]

    public class BallBehaivourData : ScriptableObject
    {
        public List<EffectConfig> behaviourConfigs = new List<EffectConfig>();

        public EffectConfig Get(BehaviourType type)
        {
            foreach (var config in behaviourConfigs)
            {
                if(config.Type == type)
                    return config;
            }

            throw new System.Exception();
        }
    }
}