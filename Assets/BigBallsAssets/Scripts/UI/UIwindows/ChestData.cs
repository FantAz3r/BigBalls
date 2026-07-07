using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace BigBalls.UI
{
    [CreateAssetMenu(fileName = "Datas", menuName = "Datas/ChestData")]

    public class ChestData : ScriptableObject
    {
        [SerializeField] private SerializedDictionary<Rarity, ChestConfig> _chests = new();

        public ChestConfig GetChest (Rarity rarity) => _chests[rarity];
    }
}