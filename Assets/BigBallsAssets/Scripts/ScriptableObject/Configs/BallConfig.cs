using BigBalls.Attributes;
using BigBalls.GameplayObjects;
using BigBalls.StaticData;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BigBalls.Configs
{
    [CustomScriptableObjectListEditor]
    [CreateAssetMenu(fileName = "BallConfig", menuName = "Configs/BallConfig")]

    public class BallConfig : ScriptableObject, IEntityConfig
    {
        [HideInInspector] public LayerMask ObstacleLayers { get; private set; }
        [field: SerializeField] public Ball Prefab { get; private set; }
        [field: SerializeField] public float Radius { get; private set; } = 0.2f;
        [field: SerializeField] public bool IsMaterial { get; private set; }
        [field: SerializeField] public List<StatStruct> Stats { get; private set; }


        [field: SerializeField] public List<EffectConfig> EffectConfigs = new ();

        public bool TryGetBehaviour(out EffectConfig behaviourConfig, BehaviourType type)
        {
            behaviourConfig = null;

            foreach (var behaviour in EffectConfigs)
            {
                if (type == behaviour.Type)
                {
                    behaviourConfig = behaviour;
                    return true;
                }
            }

            return false;
        }

        public StatStruct Get(StatType statType) => Stats.Where(stat => stat.StatType == statType).FirstOrDefault();
    }
}