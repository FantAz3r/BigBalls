using System;
using System.Collections.Generic;
using System.Linq;
using BigBalls.Configs;
using BigBalls.Saves;
using BigBalls.StaticData;
using UnityEngine;

public class ChestCalculator
{

    private readonly ChestConfig _chestConfig;
    private readonly ItemRepository<Enum, ItemModel, ItemConfig, CardSaveData> _itemRepository;
    private readonly System.Random _random;

    private readonly ArmorRepository _armorRepository;
    private readonly WeaponRepository _weaponRepository;
    private readonly List<ICardModel> _cards = new();

        public ChestCalculator (ChestConfig chestConfig, ArmorRepository armorRepository, WeaponRepository weaponRepository , int? seed = null)
    {
        _armorRepository = armorRepository;
        _weaponRepository = weaponRepository;
        _chestConfig = chestConfig ?? throw new ArgumentNullException(nameof(chestConfig));
        _random = seed.HasValue ? new System.Random(seed.Value) : new System.Random();

        _cards.AddRange(_weaponRepository.AllModels.Values);
        _cards.AddRange(_armorRepository.AllModels.Values);
    }

    public List<ItemModel> GetCardsFromChest ()
    {
        int totalCards = _chestConfig.GetCardCount();
        var rarityDistribution = GetRarityDistribution(totalCards);

        var allModels = _itemRepository.AllModels.Values;
        var modelsByRarity = allModels.GroupBy(m => m.Config.Rarity)
                                     .ToDictionary(g => g.Key, g => g.ToList());

        var result = new List<ItemModel>(totalCards);

        foreach (var kvp in rarityDistribution)
        {
            var rarity = kvp.Key; int count = kvp.Value; if (!modelsByRarity.TryGetValue(rarity, out var candidates) || candidates.Count == 0) continue; for (int i = 0; i < count; i++)
            {
                int idx = _random.Next(candidates.Count);
                var chosenModel = candidates[idx];

                if (chosenModel.IsOpened == false)
                { chosenModel.Open(); } // Добавить количество опыта или очков, равное количеству выпадений 
                                        // Но тут учитываем, что одна карта может выпадать несколько раз подряд, значит 
                                        // нужно аккумулировать количество выпадений var itemExpToAdd = 1; 
                                        // за каждую итерацию добавляем 1 опыта chosenModel.ItemEXP += itemExpToAdd; } }

                return result;
            }
        }
    }

    private Dictionary<Rarity, int> GetRarityDistribution (int totalCards)
    {
        var raritiesWithChance = Enum.GetValues(typeof(Rarity)).Cast<Rarity>()
            .Select(rarity => new
            {
                Rarity = rarity,
                Chance = _chestConfig.GetRarityChance(rarity)
            })
            .Where(x => x.Chance > 0f)
            .ToList();

        float totalChance = raritiesWithChance.Sum(x => x.Chance);

        var expectedCounts = raritiesWithChance
            .Select(x => new
            {
                x.Rarity,
                CountFloat = (x.Chance / totalChance) * totalCards,
                CountInt = 0,
                Fraction = 0f
            })
            .ToList();

        int cardsAssigned = 0;

        for (int i = 0; i < expectedCounts.Count; i++)
        {
            int floorCount = Mathf.FloorToInt(expectedCounts[i].CountFloat);
            expectedCounts[i] = new
            {
                expectedCounts[i].Rarity,
                CountFloat = expectedCounts[i].CountFloat,
                CountInt = floorCount,
                Fraction = expectedCounts[i].CountFloat - floorCount
            };
            cardsAssigned += floorCount;
        }

        int remaining = totalCards - cardsAssigned;
        var sortedByFraction = expectedCounts.OrderByDescending(x => x.Fraction).ToList();
        var result = new Dictionary<Rarity, int>();

        foreach (var ec in expectedCounts)
            result[ec.Rarity] = ec.CountInt;

        for (int i = 0; i < remaining; i++)
        {
            result[sortedByFraction[i].Rarity]++;
        }

        return result;
    }

}
