using System;
using System.Collections.Generic;
using System.Linq;
using BigBalls.GameplayObjects;
using BigBalls.StaticData;

public class PlayerCardHolder
{
    private readonly IBallContainer _ballContainer;
    private readonly IArtefactContainer _artefactContainer;
    private readonly List<ICardModel> _cards = new();
    private readonly Dictionary<ItemModel, List<ICardModel>> _itemCards = new();

    private List<BallModel> _balls = new();

    public PlayerCardHolder (IBallContainer ballContainer, IArtefactContainer artefactContainer)
    {
        _ballContainer = ballContainer;
        _artefactContainer = artefactContainer;
    }

    public event Action<ICardModel> CardAdded;
    public event Action<ICardModel> CardRemoved;
    public event Action<ICardModel> CardChanged;
    public event Action<ItemModel> ItemAdded;
    public event Action<ItemModel> ItemRemoved;

    public int ArtefactCount { get; private set; }
    public IEnumerable<BallModel> Balls => _balls;
    public IReadOnlyList<ICardModel> Cards => _cards;

    public void AddItem (ItemModel item)
    {
        if (item == null)
            return;

        if (_itemCards.ContainsKey(item))
            return;

        var cards = new List<ICardModel>();

        if (item is WeaponModel weapon)
        {
            foreach (var ball in weapon.UniqueBallModels())
            {
                cards.Add(ball);
            }
        }
        else if (item is HelmetModel helmet)
        {
            foreach (var artefact in helmet.GetArtefacts())
                cards.Add(artefact);
        }

        _itemCards[item] = cards;

        foreach (var card in cards)
            Add(card);

        ItemAdded?.Invoke(item);
    }

    public bool RemoveItem (ItemModel item)
    {
        if (item == null)
            return false;

        if (!_itemCards.TryGetValue(item, out var cards))
            return false;

        foreach (var card in cards.ToList())
            Remove(card);

        _itemCards.Remove(item);
        ItemRemoved?.Invoke(item);
        return true;
    }

    public bool HasItem (ItemModel item) => item != null && _itemCards.ContainsKey(item);


    public void Add (ICardModel card)
    {
        if (card == null)
            return;

        if (_cards.Contains(card))
            return;

        card.Upgraded += OnCardChanged;
        card.Changed += OnCardChanged;

        _cards.Add(card);

        if (card is BallModel ball)
        {
            if (_ballContainer.Balls.Contains(ball))
                return;
            _balls.Add(ball);
            _ballContainer.AddUniqueBall(ball);
        }
        else if (card is ArtefactModel artefact)
        {
            _artefactContainer.Enable(artefact);
            ArtefactCount++;
        }

        CardAdded?.Invoke(card);
    }

    public bool Remove (ICardModel card)
    {
        if (card == null)
            return false;

        if (!_cards.Remove(card))
            return false;

        card.Upgraded -= OnCardChanged;
        card.Changed -= OnCardChanged;

        if (card is BallModel ball)
        {
            _ballContainer.RemoveUniqueBall(ball);
            _balls.Remove(ball);
        }
        else if (card is ArtefactModel artefact)
        {
            _artefactContainer.Disable(artefact);
            ArtefactCount--;
        }

        foreach (var kvp in _itemCards)
            kvp.Value.Remove(card);

        CardRemoved?.Invoke(card);
        return true;
    }

    public bool Has (ICardModel card) => card != null && _cards.Contains(card);

    public void Clear ()
    {
        foreach (var card in _cards.ToList())
            Remove(card);

        ArtefactCount = 0;
        _itemCards.Clear();
    }

    private void OnCardChanged (ICardModel card) => CardChanged?.Invoke(card);
}