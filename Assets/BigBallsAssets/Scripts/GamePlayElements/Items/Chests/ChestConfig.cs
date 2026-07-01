using AYellowpaper.SerializedCollections;
using UnityEngine;

[CreateAssetMenu(fileName = "ChestConfig", menuName = "Configs/Chest")]

public class ChestConfig :ScriptableObject
{
    [SerializeField] private SerializedDictionary<Rarity, float> _rarityChance = new();
    [SerializeField] private AnimationCurve _rarityChanceCurve;

    [field: SerializeField] public Rarity Rarity { get; private set; }
    [field: SerializeField] public Sprite CloseIcon { get; private set; }
    [field: SerializeField] public Sprite OpenIcon { get; private set; }
    [field: SerializeField] public Sprite ShowIcon { get; private set; }

    [field: SerializeField] public int BaseCardCount { get; private set; }
    [field: SerializeField] public int Level { get; private set; }
    [field: SerializeField] public int CardPerLevel { get; private set; }

    public int GetCardCount ()
    {
        return BaseCardCount + CardPerLevel * Level;
    }

    public float GetRarityChance (Rarity candidateRarity)
    {
        int rarityGap = (int) candidateRarity - (int) Rarity + 1;
        if (rarityGap < 0)
            return 0f;

        if (_rarityChance.TryGetValue(candidateRarity, out float baseChance) == false)
            baseChance = 0f;

        float input = rarityGap + Level * 0.1f;
        float modifier = _rarityChanceCurve != null ? _rarityChanceCurve.Evaluate(input) : 1f;

        return Mathf.Clamp01(baseChance * modifier);
    }
}
