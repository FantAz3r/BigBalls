using System.Collections.Generic;
using UnityEngine;
using BigBalls.GameplayObjects;
using System.Linq;
using BigBalls.Configs;

namespace BigBalls.StaticData
{
    [CreateAssetMenu(fileName = "PlayerConfig", menuName = "Configs/PlayerConfig")]
    public class PlayerConfig : ScriptableObject, IEntityConfig
    {
        [SerializeField] private List <StatStruct> _stats;

        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public LayerMask ObstacleLayers { get; private set; }
        public List<StatStruct> Stats => _stats;

        public StatStruct Get(StatType statType) => Stats.Where(stat=>stat.StatType == statType).FirstOrDefault();
    }
}
