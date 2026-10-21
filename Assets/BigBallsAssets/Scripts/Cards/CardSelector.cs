using System;
using System.Collections.Generic;
using System.Linq;
using BigBalls.StaticData;

public class CardSelector
{
    private const int MaxLevel = 5;
    private const int CardsPerSelect = 3;

    private readonly BallRepository _ballsRepository;
    private readonly ArtefactsRepository _artefactsRepository;
    private readonly Dictionary<Rarity, float> _rarityChances = new()
    {
        { Rarity.Common, 75f },
        { Rarity.Rare, 17.5f },
        { Rarity.Epic, 5f },
        { Rarity.Legendary, 2.5f }
    };

    private List<ICardModel> _currentCards = new();

    public CardSelector (BallRepository ballsRepository, ArtefactsRepository artefactsRepository)
    {
        _ballsRepository = ballsRepository;
        _artefactsRepository = artefactsRepository;
    }

    public IEnumerable<ICardModel> GetCardsForSelection ()
    {
        if (_currentCards != null && _currentCards.Count > 0)
            return _currentCards;

        List<ICardModel> availableCards = GetAvailableModels();

        if (availableCards.Count == 0)
            throw new ArgumentNullException(nameof(availableCards));

        int countToSelect = Math.Min(CardsPerSelect, availableCards.Count);
        var shuffledModels = availableCards.OrderBy(x => Guid.NewGuid()).ToList();

        _currentCards = SelectCardsByRarityChance(shuffledModels, countToSelect);
        return _currentCards;
    }

    public void SaveCurrentCards (List<ICardModel> cardModels)
    {
        _currentCards = cardModels;
    }

    private List<ICardModel> GetAvailableModels ()
    {
        var result = new List<ICardModel>();
        var ballByType = _ballsRepository.AllModels.Values
            .ToDictionary(b => b.BallConfig.BallType);

        foreach (var ball in ballByType.Values)
        {
            if (ball.BallConfig.BallType == BallType.Base)
                continue;

            if (IsAvailable(ball, ballByType))
                result.Add(ball);
        }

        foreach (var artefact in _artefactsRepository.AllModels.Values)
        {
            if (artefact.IsOpen && artefact.InGameLevel < MaxLevel)
                result.Add(artefact);
        }

        return result;
    }

    private bool IsAvailable (BallModel ball, Dictionary<BallType, BallModel> ballByType)
    {
        if (ball.InGameLevel >= MaxLevel)
            return false;

        if (ball.BallConfig.IsRoot)
            return true;

        foreach (var parentType in ball.BallConfig.ParentTypes)
        {
            if (ballByType.TryGetValue(parentType, out var parent) == false || parent.IsOpen == false)
                return false;
        }

        return true;
    }

    public void ClearCurrentSelection ()
    {
        _currentCards.Clear();
    }

    private List<ICardModel> SelectCardsByRarityChance (List<ICardModel> cards, int count)
    {
        var selectedCards = new List<ICardModel>();
        var random = new System.Random();
        var weightedList = new List<(ICardModel card, float weight)>();

        foreach (var card in cards)
        {
            float rarityChance = _rarityChances.ContainsKey(card.Config.Rarity) ? _rarityChances[card.Config.Rarity] : 0f;
            int sameRarityCount = cards.Count(c => c.Config.Rarity == card.Config.Rarity);
            float weight = rarityChance / sameRarityCount;
            weightedList.Add((card, weight));
        }

        for (int i = 0; i < count && weightedList.Count > 0; i++)
        {
            float totalWeight = weightedList.Sum(x => x.weight);
            float roll = (float) (random.NextDouble() * totalWeight);
            float cumulative = 0f;
            ICardModel chosen = null;
            int chosenIndex = -1;

            for (int j = 0; j < weightedList.Count; j++)
            {
                cumulative += weightedList[j].weight;

                if (roll <= cumulative)
                {
                    chosen = weightedList[j].card;
                    chosenIndex = j;
                    break;
                }
            }

            if (chosen != null)
            {
                selectedCards.Add(chosen);
                weightedList.RemoveAt(chosenIndex);
            }
        }

        return selectedCards;
    }
}
