using UnityEngine;

public class ChestModel
{
    private const int NonGameLevelCount = 3;

    public ChestModel(int level, ChestConfig config)
    {
        Level = level - NonGameLevelCount;
        Config = config;
    }

    public int Level { get; private set; }
    public ChestConfig Config { get; private set; }

    public int GetCardCount()
    {
        return Config.BaseCardCount + Config.CardPerLevel * Level;
    }

    public float GetRarityChance(Rarity candidateRarity)
    {
        int rarityGap = (int)candidateRarity - (int)Config.Rarity + 1;
        if (rarityGap < 0)
            return 0f;

        if (Config.RarityChance.TryGetValue(candidateRarity, out float baseChance) == false)
            baseChance = 0f;

        float input = rarityGap + Level * 0.1f;
        float modifier = Config.RarityChanceCurve != null ? Config.RarityChanceCurve.Evaluate(input) : 1f;

        return Mathf.Clamp01(baseChance * modifier);
    }

}