using System.Collections.Generic;
using BigBalls.Attributes;
using BigBalls.GameplayObjects;
using UnityEngine;

namespace BigBalls.Configs
{
    [CustomScriptableObjectListEditor]
    [CreateAssetMenu(fileName = "Artefact", menuName = "Configs/Item/WeaponConfigs")]

    public class ArtefactConfig : ScriptableObject
    {
        [field: SerializeField] public List<EffectConfig> Effects { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; }

        public bool TryGetEffect(out EffectConfig behaviourConfig, BehaviourType type)
        {
            behaviourConfig = null;

            foreach (var behaviour in Effects)
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