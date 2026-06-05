using BigBalls.GameplayObjects;
using System.Collections.Generic;
using UnityEngine;
using BigBalls.Attributes;

namespace BigBalls.StaticData
{
    [CustomScriptableObjectListEditor]
    [CreateAssetMenu(fileName = "BallConfig", menuName = "Configs/BallConfig")]
    public class BallConfig : ScriptableObject
    {
        [field: SerializeField] public Ball Prefab { get; private set; }
        [field: SerializeField] public float Speed { get; private set; }
        [field: SerializeField] public float Radius { get; private set; } = 0.2f;
        [field: SerializeField] public bool IsMaterial { get; private set; }

        [field: SerializeField] public List<EffectConfig> BehaviourConfigs = new ();

        public bool TryGetBehaviour(out EffectConfig behaviourConfig, BehaviourType type)
        {
            behaviourConfig = null;

            foreach (var behaviour in BehaviourConfigs)
            {
                if (type == behaviour.Type)
                {
                    behaviourConfig = behaviour;
                    return true;
                }
            }

            return false;
        }
    }
}