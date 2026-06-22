using System.Collections.Generic;
using BigBalls.GameplayObjects;

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

    public void Add (ICardModel card)
    {
        _cards.Add(card);

        if (card is BallModel ball)
        {
            _ballContainer.AddUniqueBall(ball);
        }
        else if (card is ArtefactModel artefact)
        {
            _artefactContainer.Enable(artefact);
        }
    }

    public void Remove (ICardModel card)
    {
        _cards.Remove(card);
    }
}