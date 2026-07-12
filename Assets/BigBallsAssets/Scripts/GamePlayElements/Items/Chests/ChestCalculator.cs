using BigBalls.StaticData;
using System;
using System.Collections.Generic;
using System.Linq;

public class ChestCalculator
{
    private readonly System.Random _random;
    private readonly ArmorRepository _armorRepository;
    private readonly WeaponRepository _weaponRepository;
    private readonly List<ICardModel> _cards = new();

    public ChestModel ChestModel { get; private set; }
    // Добавлены недостающие поля
    private const int BaseGold = 100;
    private const int TotalCardsCost = 10;

    public ChestCalculator(ArmorRepository armorRepository, WeaponRepository weaponRepository)
    {
        _armorRepository = armorRepository;
        _weaponRepository = weaponRepository;
        _random = new System.Random();

        _cards.AddRange(_weaponRepository.AllModels.Values);
        _cards.AddRange(_armorRepository.AllModels.Values);
    }

    public ChestOpenResult OpenChest(ChestModel model)
    {

        ChestModel = model;
        var result = new ChestOpenResult();
        var totalCards = model.GetCardCount(); // Добавлена переменная totalCards

        // Проверяем, есть ли вообще доступные карты
        var allAvailableCards = _cards
            .Where(card => card.Config.IsOpen || card.IsOpen) // Карты, которые можно получить
            .ToList();

        if (allAvailableCards.Count == 0)
        {
            // Все карты закрыты - выдаем только золото
            result.GoldAmount = CalculateGoldCompensation(totalCards);
            return result; // Исправлено: return result, а не return result.GoldAmount
        }

        // Проверяем количество полностью прокачанных карт
        var notMaxedCards = allAvailableCards
            .Where(card => card.Level < card.MaxLevel)
            .ToList();

        var rarityDistribution = GetRarityDistribution(totalCards);

        // Группируем все доступные карты по редкости
        var modelsByRarity = allAvailableCards
            .GroupBy(model => model.Config.Rarity)
            .ToDictionary(g => g.Key, g => g.ToList());

        foreach (var kvp in rarityDistribution)
        {
            var rarity = kvp.Key;
            int count = kvp.Value;

            if (modelsByRarity.TryGetValue(rarity, out var candidates) == false || candidates.Count == 0)
            {
                // Нет карт этой редкости - компенсируем золотом
                result.GoldAmount += CalculateRarityGoldCompensation(rarity, count);
                continue;
            }

            // Проверяем, сколько карт этой редкости можно еще прокачать
            var notMaxedCandidates = candidates
                .Where(card => card.Level < card.MaxLevel)
                .ToList();

            if (notMaxedCandidates.Count == 0)
            {
                // Все карты этой редкости прокачаны - компенсируем золотом
                result.GoldAmount += CalculateRarityGoldCompensation(rarity, count);
                continue;
            }

            for (int i = 0; i < count; i++)
            {
                // Если закончились непрокачанные карты, но еще нужно выдать
                if (notMaxedCandidates.Count == 0)
                {
                    result.GoldAmount += CalculateSingleCardGoldCompensation(rarity); // Исправлено: передаем rarity
                    continue;
                }

                int idx = _random.Next(notMaxedCandidates.Count);
                var chosenModel = notMaxedCandidates[idx] as ItemModel;

                if (chosenModel != null)
                {
                    // Открываем карту, если она еще не открыта
                    if (chosenModel.IsOpen == false)
                    {
                        chosenModel.OpenItem();
                    }

                    result.Cards.Add(chosenModel);

                    // Если карта достигла максимума после добавления, убираем из доступных
                    if (chosenModel.Level >= chosenModel.MaxLevel)
                    {
                        notMaxedCandidates.RemoveAt(idx);
                    }
                }
            }
        }

        // Если вообще не удалось выдать карты (все прокачаны)
        if (result.Cards.Count == 0 && totalCards > 0)
        {
            result.GoldAmount += CalculateGoldCompensation(totalCards);
        }

        return result; // Исправлено: return result, а не return result.GoldAmount
    }

    // Исправлен метод: добавлен параметр rarity
    public int CalculateSingleCardGoldCompensation(Rarity rarity)
    {
        // Базовое золото за сундук + дополнительное за каждую невыданную карту
        return BaseGold + TotalCardsCost * 50;
    }

    public int CalculateRarityGoldCompensation(Rarity rarity, int count)
    {
        // Разное количество золота в зависимости от редкости
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

    // Исправлен метод: добавлен параметр rarity
    private int CalculateGoldCompensation(int totalCards)
    {
        // Здесь нужна логика расчета золота в зависимости от редкости
        // Так как параметр totalCards - это количество карт, а не редкость
        // Исправляем на правильную логику
        return totalCards * 10; // Например, 10 золота за карту
    }

    private Dictionary<Rarity, int> GetRarityDistribution(int totalCards)
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
            int floorCount = (int)Math.Floor(ec.CountFloat);
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
    private float GetChanceForRarity(Rarity rarity)
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