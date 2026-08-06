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

    public class BallConfig : ItemConfig, IEntityConfig
    {
        [HideInInspector] public LayerMask ObstacleLayers { get; private set; }
        [field: SerializeField] public Ball Prefab { get; private set; }
        [field: SerializeField] public Color DamageColor { get; private set; }
        [field: SerializeField] public EntityType EntityType { get; private set; }
        [field: SerializeField] public BallType BallType { get; private set; }
        [field: SerializeField] public bool IsMaterial { get; private set; }
        [field: SerializeField] public bool IsUnique { get; private set; } = true;
        [field: SerializeField] public List<StatStruct> Stats { get; private set; }
        [field: SerializeField] public List<EffectConfig> Effects { get; private set; }

        [field: SerializeField] public List<BallType> ParentTypes { get; private set; }
        [field: SerializeField] public int UnlockPrice { get; private set; }
        [field: SerializeField] public int RequiredEXP { get; private set; }
        [field: SerializeField] public Vector2 Position { get; set; }
        public bool IsRoot => ParentTypes == null || ParentTypes.Count == 0;

        public StatStruct Get(StatType statType) => Stats.Where(stat => stat.StatType == statType).FirstOrDefault();

        public override List<ItemStat> GetStats(int level)
        {
            List<ItemStat> stats = new List<ItemStat>();

            foreach (var item in Effects)
            {
                stats.AddRange(item.GetStats(level));
            }

            return stats;
        }
    }
}