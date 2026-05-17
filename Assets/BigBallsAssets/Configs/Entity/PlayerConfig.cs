using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;
using BigBalls.GameplayObjects;

namespace BigBalls.StaticData
{
    [CreateAssetMenu(fileName = "PlayerConfig", menuName = "Configs/PlayerConfig")]
    public class PlayerConfig : ScriptableObject
    {
        [field: SerializeField] public string Name { get; private set; }

        [SerializedDictionary("StatType", "Value")] 
        [SerializeField] private AYellowpaper.SerializedCollections.SerializedDictionary<StatType, float> _stats;

        public IReadOnlyDictionary<StatType, float> Stats => _stats; 
    }
}
