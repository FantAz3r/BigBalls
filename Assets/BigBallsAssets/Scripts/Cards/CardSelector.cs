using System;
using System.Collections.Generic;
using System.Linq;

public class CardSelector
{
    private const int MaxLevel = 10;
    private const int CardsPerSelect = 3;

    private readonly BallsRepository _ballsRepository;
    private readonly ArtefactsRepository _artefactsRepository;
    private readonly Dictionary<Rarity, float> _rarityChances = new()
    {
        { Rarity.Common, 60f },
        { Rarity.Rare, 25f },
        { Rarity.Epic, 10f },
        { Rarity.Legendary, 5f }
    };

    private List<ICardModel> _currentCards = new();

    public CardSelector (BallsRepository ballsRepository, ArtefactsRepository artefactsRepository)
    {
        _ballsRepository = ballsRepository;
        _artefactsRepository = artefactsRepository;
    }

    public IEnumerable<ICardModel> GetCardsForSelection ()
    {
        if (_currentCards != null && _currentCards.Count > 0)
        {
            return _currentCards;
        }

        List<ICardModel> availableCards = GetAvailableModels();

        if (availableCards.Count == 0)
        {
            throw new ArgumentNullException(nameof(availableCards));
        }

        var shuffledModels = availableCards.OrderBy(x => Guid.NewGuid()).ToList();

        _currentCards = SelectCardsByRarityChance(availableCards, CardsPerSelect);

        return _currentCards;
    }

    public void SaveCurrentCards(List<ICardModel> cardModels)
    {
        _currentCards = cardModels;
    }

    private List<ICardModel> GetAvailableModels ()
    {
        var allModels = new List<ICardModel>();
        allModels.AddRange(_ballsRepository.AllModels.Values);
        allModels.AddRange(_artefactsRepository.AllModels.Values);

        var filteredModels = allModels
            .Where(model => model.IsOpen && model.Level < MaxLevel)
            .ToList();

        return filteredModels;
    }

    public void ClearCurrentSelection ()
    {
        _currentCards.Clear();
    }

    private List<ICardModel> SelectCardsByRarityChance (List<ICardModel> cards, int count)
    {
        var selectedCards = new List<ICardModel>();
        var random = new Random();

        var groupedByRarity = cards.GroupBy(c => c.Config.Rarity).ToDictionary(g => g.Key, g => g.ToList());
        var weightedList = new List<(ICardModel card, float weight)>();

        foreach (var rarityGroup in groupedByRarity)
        {
            float rarityChance = _rarityChances.ContainsKey(rarityGroup.Key) ? _rarityChances[rarityGroup.Key] : 0f;

            foreach (var card in rarityGroup.Value)
            {
                float weight = rarityChance / rarityGroup.Value.Count;
                weightedList.Add((card, weight));
            }
        }

        while (selectedCards.Count < count && weightedList.Count > 0)
        {
            float totalWeight = weightedList.Sum(x => x.weight);
            float roll = (float) (random.NextDouble() * totalWeight);
            float cumulative = 0f;

            foreach (var (card, weight) in weightedList)
            {
                cumulative += weight;

                if (roll <= cumulative)
                {
                    selectedCards.Add(card);
                }

                weightedList.Remove((card, weight));
                break;
            }
        }

        return selectedCards;
    }
}
