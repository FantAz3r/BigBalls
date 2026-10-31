using System;
using System.Collections.Generic;
using System.Linq;
using BigBalls.StaticData;

public class ChestCalculator
{
    private readonly System.Random _random;
    private readonly ArmorRepository _armorRepository;
    private readonly WeaponRepository _weaponRepository;
    private readonly HelmetRepository _helmetRepository;
    private readonly List<ICardModel> _cards = new();

    public ChestModel ChestModel { get; private set; }
    private const int BaseGold = 100;
    private const int TotalCardsCost = 10;

    public ChestCalculator (ArmorRepository armorRepository, WeaponRepository weaponRepository, HelmetRepository helmetRepository)
    {
        _armorRepository = armorRepository;
        _weaponRepository = weaponRepository;
        _helmetRepository = helmetRepository;
        _random = new System.Random();

        _cards.AddRange(_weaponRepository.AllModels.Values);
        _cards.AddRange(_armorRepository.AllModels.Values);
        _cards.AddRange(_helmetRepository.AllModels.Values);
    }

    public ChestOpenResult OpenChest (ChestModel model)
    {
        ChestModel = model;
        var result = new ChestOpenResult();
        var totalCards = model.GetCardCount();

        var allAvailableCards = _cards
            .Where(card => card.Config.IsOpen || card.IsOpen)
            .ToList();

        if (allAvailableCards.Count == 0)
        {
            result.GoldAmount = CalculateGoldCompensation(totalCards);
            return result;
        }

        var notMaxedCards = allAvailableCards
            .Where(card => card.Level < card.MaxLevel)
            .ToList();

        var rarityDistribution = GetRarityDistribution(totalCards);
        var modelsByRarity = allAvailableCards
            .GroupBy(model => model.Config.Rarity)
            .ToDictionary(g => g.Key, g => g.ToList());

        foreach (var kvp in rarityDistribution)
        {
            var rarity = kvp.Key;
            int count = kvp.Value;

            if (modelsByRarity.TryGetValue(rarity, out var candidates) == false || candidates.Count == 0)
            {
                result.GoldAmount += CalculateRarityGoldCompensation(rarity, count);
                continue;
            }

            var notMaxedCandidates = candidates
                .Where(card => card.Level < card.MaxLevel)
                .ToList();

            if (notMaxedCandidates.Count == 0)
            {
                result.GoldAmount += CalculateRarityGoldCompensation(rarity, count);
                continue;
            }

            for (int i = 0; i < count; i++)
            {
                if (notMaxedCandidates.Count == 0)
                {
                    result.GoldAmount += CalculateSingleCardGoldCompensation(rarity);
                    continue;
                }

                int idx = _random.Next(notMaxedCandidates.Count);
                var chosenModel = notMaxedCandidates[idx] as ItemModel;

                if (chosenModel != null)
                {
                    if (chosenModel.IsOpen == false)
                    {
                        chosenModel.OpenItem();
                    }

                    result.Cards.Add(chosenModel);

                    if (chosenModel.Level >= chosenModel.MaxLevel)
                    {
                        notMaxedCandidates.RemoveAt(idx);
                    }
                }
            }
        }

        if (result.Cards.Count == 0 && totalCards > 0)
        {
            result.GoldAmount += CalculateGoldCompensation(totalCards);
        }

        return result;
    }

    public int CalculateSingleCardGoldCompensation (Rarity rarity)
    {
        return BaseGold + TotalCardsCost * 50;
    }

    public int CalculateRarityGoldCompensation (Rarity rarity, int count)
    {
        int goldPerCard = rarity switch
        {
            Rarity.Common => 5,
            Rarity.Rare => 50,
            Rarity.Epic => 500,
            Rarity.Legendary => 2000,
            _ => 10
        };

        return goldPerCard * count;
    }

    private int CalculateGoldCompensation (int totalCards)
    {
        return totalCards * 10;
    }

    private Dictionary<Rarity, int> GetRarityDistribution (int totalCards)
    {
        var raritiesWithChance = Enum.GetValues(typeof(Rarity))
            .Cast<Rarity>()
            .Select(rarity => new
            {
                Rarity = rarity,
                Chance = GetChanceForRarity(rarity)

            })
            .Where(x => x.Chance > 0f)
            .ToList();

        if (raritiesWithChance.Count == 0)
            return new Dictionary<Rarity, int>();

        float totalChance = raritiesWithChance.Sum(x => x.Chance);

        var expectedCounts = raritiesWithChance
            .Select(x => new
            {
                x.Rarity,
                CountFloat = (x.Chance / totalChance) * totalCards
            })
            .ToList();

        var result = new Dictionary<Rarity, int>();
        int cardsAssigned = 0;

        var fractions = new List<(Rarity rarity, float fraction)>();

        foreach (var ec in expectedCounts)
        {
            int floorCount = (int) Math.Floor(ec.CountFloat);

            result[ec.Rarity] = floorCount;
            cardsAssigned += floorCount;

            fractions.Add((ec.Rarity, ec.CountFloat - floorCount));
        }

        int remaining = totalCards - cardsAssigned;
        var sortedByFraction = fractions.OrderByDescending(x => x.fraction).ToList();

        for (int i = 0; i < remaining && i < sortedByFraction.Count; i++)
        {
            result[sortedByFraction[i].rarity]++;
        }

        return result;
    }
    private float GetChanceForRarity (Rarity rarity)
    {
        return rarity switch
        {
            Rarity.Common => 0.5f,
            Rarity.Rare => 0.3f,
            Rarity.Epic => 0.15f,
            Rarity.Legendary => 0.05f,
            _ => 0f
        };
    }
}
