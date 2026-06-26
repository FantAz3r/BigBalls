using System.Collections.Generic;
using System.Linq;
using BigBalls.Saves;
using BigBalls.Services;

public class CardContainer
{
    private readonly IResourceLoader _resourceLoader;
    private readonly BallsRepository _ballsRepository;
    private readonly ArtefactsRepository _artefactsRepository;

    private List<ICardModel> _cards = new List<ICardModel>();
    private GameProgress _gameProgress;

    public CardContainer (
        IResourceLoader resourceLoader,
        GameProgress gameProgress,
        BallsRepository ballsRepository,
        ArtefactsRepository artefactsRepository)
    {
        _resourceLoader = resourceLoader;
        _gameProgress = gameProgress;
        _ballsRepository = ballsRepository;
        _artefactsRepository = artefactsRepository;
    }

    public List<ICardModel> CardModels => _cards;

    public void SaveCards ()
    {

    }

    private void UpdateCards ()
    {
        _cards.Clear();
        _cards.AddRange(_ballsRepository.AllModels.Values.Where(ball => ball.IsOpen).ToList());
        _cards.AddRange(_artefactsRepository.AllModels.Values.Where(artefact => artefact.IsOpen).ToList());
    }
}
