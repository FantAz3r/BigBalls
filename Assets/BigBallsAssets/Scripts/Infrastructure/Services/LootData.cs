using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using UnityEngine;

namespace BigBalls.Services
{
    [CreateAssetMenu(menuName = "Datas/LootData")]

    public class LootData : ScriptableObject
    {
        [SerializeField] private SerializedDictionary<int, Loot> _coins = new SerializedDictionary<int, Loot>();
        [SerializeField] private SerializedDictionary<int, Loot> _exp = new SerializedDictionary<int, Loot>();
        [SerializeField] private SerializedDictionary<int, Loot> _health = new SerializedDictionary<int, Loot>();

        public List<Loot> GetAllLoot()
        {
            List<Loot> loots = new List<Loot>();
            loots.AddRange(_coins.Values);
            loots.AddRange(_exp.Values);
            loots.AddRange(_health.Values);
            return loots;
        }

        public List<LootResult> GetLoot(LootType type, int totalValue)
        {
            var dictionary = type switch
            {
                LootType.Coin => _coins,
                LootType.Experience => _exp,
                LootType.HealthPotion => _health,
                _ => null
            };

            if (dictionary == null || dictionary.Count == 0)
                return new List<LootResult>();

            return CalculateLoot(dictionary, totalValue);
        }

        private List<LootResult> CalculateLoot(SerializedDictionary<int, Loot> dict, int remainingValue)
        {
            List<LootResult> results = new List<LootResult>();
            List<int> sortedValues = new List<int>(dict.Keys);
            sortedValues.Sort((a, b) => b.CompareTo(a));

            foreach (int value in sortedValues)
            {
                if (remainingValue <= 0) break;

                if (remainingValue >= value)
                {
                    int count = remainingValue / value;
                    remainingValue %= value;

                    results.Add(new LootResult
                    {
                        Prefab = dict[value],
                        Amount = count
                    });
                }
            }

            return results;
        }
    }
}