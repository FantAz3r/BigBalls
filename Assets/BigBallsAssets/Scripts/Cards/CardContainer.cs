using System;
using System.Collections.Generic;
using BigBalls.Services;
using BigBalls.Saves;

public class CardContainer
{
    private readonly IResourceLoader _resourceLoader;
    private List<ICardModel> _cards = new List<ICardModel>();
    private GameProgress _gameProgress;
    public CardContainer (IResourceLoader resourceLoader, GameProgress gameProgress)
    {
        _resourceLoader = resourceLoader;
        _gameProgress = gameProgress;
    }

    public void FillContainer ()
    {
        if (_gameProgress.Items == null && _gameProgress.Items.Count == 0)
        {
            _cards = LoadStartCards();
        }
        else
        {
            _cards = LoadCards();
        }
    }

    private List<ICardModel> LoadStartCards ()
    {
        throw new NotImplementedException();
    }

    public void SaveCards ()
    {
    }

    private List<ICardModel> LoadCards ()
    {
        return null;
    }
}
