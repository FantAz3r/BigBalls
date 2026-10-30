using System;
using System.Collections.Generic;
using System.Linq;
using BigBalls.GameplayObjects;
using BigBalls.Providers;
using BigBalls.StaticData;

public class PlayerCardHolder
{
    private readonly IBallContainer _ballContainer;
    private readonly IArtefactContainer _artefactContainer;
    private readonly IItemConainerProvider _itemConainer;
    private readonly List<ICardModel> _cards = new();
    private readonly Dictionary<ItemModel, List<ICardModel>> _itemCards = new();
    private bool _canEnable;
    private List<BallModel> _balls = new();
    private List<ArtefactModel> _artefacts = new();

    public PlayerCardHolder (IBallContainer ballContainer, IArtefactContainer artefactContainer, IItemConainerProvider itemConainer)
    {
        _ballContainer = ballContainer;
        _artefactContainer = artefactContainer;
        _itemConainer = itemConainer;

        _itemConainer.HelmetSeted += AddItem;
        _itemConainer.WeaponSeted += AddItem;
        _itemConainer.HelmetRemoved += RemoveItem;
        _itemConainer.WeaponRemoved += RemoveItem;
    }

    public event Action<ICardModel> CardAdded;
    public event Action<ICardModel> CardRemoved;
    public event Action<ICardModel> CardChanged;
    public event Action<ItemModel> ItemAdded;
    public event Action<ItemModel> ItemRemoved;

    public int ArtefactCount { get; private set; }
    public IEnumerable<BallModel> Balls => _balls;
    public IEnumerable<ArtefactModel> Artefacts => _artefacts;
    public IReadOnlyList<ICardModel> Cards => _cards;


    public void Init()
    {
        _canEnable = true;

        foreach (var card in _cards)
        {
            if (card is BallModel ball)
            {
                if (_ballContainer.Balls.Contains(ball))
                    return;

                _ballContainer.AddUniqueBall(ball);
            }
            else if (card is ArtefactModel artefact)
            {
                _artefactContainer.Enable(artefact);
            }
        }
    }

    public void AddItem (ItemModel item)
    {
        if (item == null)
            return;

        if (_itemCards.ContainsKey(item))
            return;

        var cards = new List<ICardModel>();

        if (item is WeaponModel weapon)
        {
            weapon.AddEquipmentLevel();
            foreach (var ball in weapon.UniqueBallModels())
                cards.Add(ball);
        }
        else if (item is HelmetModel helmet)
        {
            helmet.AddEquipmentLevel();
            foreach (var artefact in helmet.GetArtefacts())
                cards.Add(artefact);
        }

        _itemCards[item] = cards;

        foreach (var card in cards)
            Add(card);

        ItemAdded?.Invoke(item);
    }

    public void RemoveItem (ItemModel item)
    {
        if (item == null)
            return;

        if (_itemCards.TryGetValue(item, out var cards) == false)
            return;

        foreach (var card in cards.ToList())
            Remove(card);

        if (item is WeaponModel weapon)
        {
            weapon.RemoveEquipmentLevel();
        }
        else if (item is HelmetModel helmet)
        {
            helmet.RemoveEquipmentLevel();
        }

        _itemCards.Remove(item);
        ItemRemoved?.Invoke(item);
        return;
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
        card.AddToPlayer();

        if (card is BallModel ball)
        {
            if (_ballContainer.Balls.Contains(ball))
                return;
            _balls.Add(ball);

            if(_canEnable)
                _ballContainer.AddUniqueBall(ball);
        }
        else if (card is ArtefactModel artefact)
        {
             if(_canEnable)
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
        card.RemoveFromPlayer();

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
        _itemConainer.HelmetSeted -= AddItem;
        _itemConainer.WeaponSeted -= AddItem;
        _itemConainer.HelmetRemoved -= RemoveItem;
        _itemConainer.WeaponRemoved -= RemoveItem;

        foreach (var card in _cards.ToList())
            Remove(card);

        ArtefactCount = 0;
        _itemCards.Clear();
    }

    private void OnCardChanged (ICardModel card) => CardChanged?.Invoke(card);
}