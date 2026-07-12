using AYellowpaper.SerializedCollections;
using BigBalls.GameplayObjects;
using BigBalls.Localization;
using BigBalls.StaticData;
using System.Collections.Generic;
using UnityEngine;
using Utils;


namespace BigBalls.Configs
{
    [CreateAssetMenu(fileName = "ArmorConfig", menuName = "Configs/Item/ArmorConfigs")]

    public class ArmorConfig : ItemConfig, IArmor
    {
        [SerializeField] private SerializedDictionary<StatType, StatStruct> _baseStats;
        [SerializeField] private SerializedDictionary<StatType, StatStruct> _statsUpgradePerLevel;

        public Dictionary<StatType, StatStruct> Stats => _baseStats;

        public float GetMaxStatValue(int level, StatType type)
            => _baseStats[type].StartMaxValue + _statsUpgradePerLevel[type].StartMaxValue * level;

        public float GetCurrentStatValue(int level, StatType type)
            => _baseStats[type].StartCurrentValue + _statsUpgradePerLevel[type].StartCurrentValue * level;

        public float GetMinStatValue(int level, StatType type)
            => Math.Additive(_baseStats[type].StartCurrentValue, _statsUpgradePerLevel[type].StartCurrentValue, level);

        public override List<ItemStat> GetStats(int level)
        {
            List<ItemStat> stats = new List<ItemStat>();
            int nextLevel = level + 1;

            foreach (var stat in Stats)
            {
                stats.Add(new ItemStat(TextLocalizator.GetStatTypeText(stat.Key), GetCurrentStatValue(level, stat.Key), GetCurrentStatValue(nextLevel, stat.Key)));
            }

            return stats;
        }
    }
}