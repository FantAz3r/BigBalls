using System.Collections.Generic;
using UnityEngine;
using BigBalls.GameplayObjects;
using System.Linq;

namespace BigBalls.StaticData
{
    [CreateAssetMenu(fileName = "PlayerConfig", menuName = "Configs/PlayerConfig")]
    public class PlayerConfig : ScriptableObject, IEntityConfig
    {
        [field: SerializeField] public string Name { get; private set; }

        [SerializeField] private List <StatStruct> _stats;

        public IEnumerable<StatStruct> Stats => _stats;

        public StatStruct Get(StatType statType) => Stats.Where(stat=>stat.StatType == statType).FirstOrDefault();
    }
}
