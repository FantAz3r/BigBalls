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
    [field: SerializeField] public int Level { get; private set; }
    [field: SerializeField] public int CardPerLevel { get; private set; }

    public Dictionary<Rarity, float> RarityChance => _rarityChance;
    public int GetCardCount()
    {
        return BaseCardCount + CardPerLevel * Level;
    }

    public int GetGold() => Random.Range(MinCoin, MinCoin + 1);

    public float GetRarityChance(Rarity candidateRarity)
    {
        int rarityGap = (int)candidateRarity - (int)Rarity + 1;
        if (rarityGap < 0)
            return 0f;

        if (_rarityChance.TryGetValue(candidateRarity, out float baseChance) == false)
            baseChance = 0f;

        float input = rarityGap + Level * 0.1f;
        float modifier = RarityChanceCurve != null ? RarityChanceCurve.Evaluate(input) : 1f;


        return Mathf.Clamp01(baseChance * modifier);
    }
}
