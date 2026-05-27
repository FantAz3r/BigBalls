using BigBalls.GameplayObjects;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BigBalls.StaticData
{
    public class EnemyConfig : ScriptableObject, IEntityConfig
    {
        [SerializeField] private List<StatStruct> _stats;

        [field: SerializeField] public Enemy Prefab { get; private set; }
        [field: SerializeField] public LayerMask ObstacleLayers { get; private set; }

        public List<StatStruct> Stats => _stats;

        public StatStruct Get(StatType statType) => Stats.Where(stat => stat.StatType == statType).FirstOrDefault();
    }
}