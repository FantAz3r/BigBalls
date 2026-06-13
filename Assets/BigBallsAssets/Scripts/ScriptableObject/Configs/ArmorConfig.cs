using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using BigBalls.GameplayObjects;
using BigBalls.StaticData;
using UnityEngine;

namespace BigBalls.Configs
{
    [CreateAssetMenu(fileName = "ArmorConfig", menuName = "Configs/Item/ArmorConfigs")]

    public class ArmorConfig : ItemConfig, IArmor
    {
        [SerializeField] private SerializedDictionary<StatType, StatStruct> _stats;

        public Dictionary<StatType, StatStruct> Stats => _stats;
    }
}