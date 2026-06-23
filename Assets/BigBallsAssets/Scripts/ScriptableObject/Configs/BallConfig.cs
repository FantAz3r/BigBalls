using System.Collections.Generic;
using System.Linq;
using BigBalls.Attributes;
using BigBalls.GameplayObjects;
using BigBalls.StaticData;
using UnityEngine;

namespace BigBalls.Configs
{
    [CustomScriptableObjectListEditor]
    [CreateAssetMenu(fileName = "BallConfig", menuName = "Configs/BallConfig")]

    public class BallConfig : ItemConfig, IEntityConfig
    {
        [HideInInspector] public LayerMask ObstacleLayers { get; private set; }
        [field: SerializeField] public Ball Prefab { get; private set; }
        [field: SerializeField] public EntityType EntityType { get; private set; }
        [field: SerializeField] public BallType BallType { get; private set; }
        [field: SerializeField] public float Radius { get; private set; } = 0.2f;
        [field: SerializeField] public bool IsMaterial { get; private set; }
        [field: SerializeField] public bool IsUnique { get; private set; } = true;
        [field: SerializeField] public List<StatStruct> Stats { get; private set; }
        [field: SerializeField] public List<EffectConfig> EffectConfigs { get; private set; }


        [field: SerializeField] public List<BallType> ParentTypes { get; private set; }
        [field: SerializeField] public int UnlockPrice { get; private set; }
        [field: SerializeField] public int RequiredEXP { get; private set; }
        [field: SerializeField] public Vector2 Position { get; set; }
        public bool IsRoot => ParentTypes == null || ParentTypes.Count == 0;

        public bool TryGetEffect (out EffectConfig behaviourConfig, BehaviourType type)
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

        public StatStruct Get (StatType statType) => Stats.Where(stat => stat.StatType == statType).FirstOrDefault();
    }
}