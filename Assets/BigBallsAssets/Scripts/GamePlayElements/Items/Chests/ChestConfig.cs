using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ChestConfig", menuName = "Configs/Chest")]

public class ChestConfig : ScriptableObject
{
    [SerializeField] private SerializedDictionary<Rarity, float> _rarityChance = new();
    [field: SerializeField] public AnimationCurve RarityChanceCurve { get; private set; }

    [field: SerializeField] public Rarity Rarity { get; private set; }
    [field: SerializeField] public Sprite CloseIcon { get; private set; }
    [field: SerializeField] public Sprite OpenIcon { get; private set; }
    [field: SerializeField] public Sprite ShowIcon { get; private set; }
    [field: SerializeField] public Sprite GoldSprite { get; private set; }

    [field: SerializeField] public int MaxCoin { get; private set; }
    [field: SerializeField] public int MinCoin { get; private set; }

    [field: SerializeField] public int BaseCardCount { get; private set; }
    [field: SerializeField] public int CardPerLevel { get; private set; }

    public Dictionary<Rarity, float> RarityChance => _rarityChance;
    public int GetGold() => Random.Range(MinCoin, MinCoin + 1);
}
