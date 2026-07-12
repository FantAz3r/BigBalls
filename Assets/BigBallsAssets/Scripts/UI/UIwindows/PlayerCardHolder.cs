using System;
using System.Collections.Generic;
using System.Linq;
using BigBalls.GameplayObjects;
using BigBalls.StaticData;

public class PlayerCardHolder
{
    private readonly IBallContainer _ballContainer;
    private readonly IArtefactContainer _artefactContainer;
    private List<ICardModel> _cards = new();

    public PlayerCardHolder (IBallContainer ballContainer, IArtefactContainer artefactContainer)
    {
        _ballContainer = ballContainer;
        _artefactContainer = artefactContainer;
    }

    public event Action<ICardModel> CardChanged;

    public List<ICardModel> Cards => _cards;

    public void AddItem (ItemModel item)
    {
        if (item is WeaponModel weapon)
        {
            foreach (var ball in weapon.UniqueBallModels())
            {
                Add(ball);
            }
        }
        else if (item is HelmetModel helmet)
        {
            //foreach (var artefact in helmet.)
            //{
            //    Add(artefact);
            //}
            // _artefactContainer.Set(helmet);
        }
    }

    public void Add (ICardModel card)
    {
        card.Upgraded += OnCardChanged;
        card.Changed += OnCardChanged;

        _cards.Add(card);

        if (card is BallModel ball)
        {
            if (_ballContainer.Balls.Contains(card))
                return;

            _ballContainer.AddUniqueBall(ball);
        }
        else if (card is ArtefactModel artefact)
        {
            _artefactContainer.Enable(artefact);
        }
    }

    private void OnCardChanged (ICardModel card) => CardChanged?.Invoke(card);

    public void Remove (ICardModel card)
    {
        card.Upgraded -= OnCardChanged;
        card.Changed -= OnCardChanged;
        _cards.Remove(card);
    }
}