using BigBalls.Services;
using BigBalls.UI;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class CardView : ButtonClickHandler
{
    [SerializeField] private Image _image;
    [SerializeField] private TMP_Text _name;
    [SerializeField] private TMP_Text _level;
    [SerializeField] private RectTransform _statsParent;


    private ITranslateService _translateService;
    private PlayerCardHolder _playerCardHolder;
    private ICardModel _card;
    private StatTextHolder _statTextHolder;
    private List<StatTextHolder> _statTextHolders = new();


    public event Action Selected;

    [Inject]
    public void Construct(ITranslateService translateService, IResourceLoader resourceLoader)
    {
        _translateService = translateService;
        _statTextHolder = resourceLoader.Load<StatTextHolder>();
    }

    public void Init(PlayerCardHolder playerCardHolder)
    {
        _playerCardHolder = playerCardHolder;
    }

    public void Render(ICardModel card)
    {
        ClearStats();
        _card = card;
        _image.sprite = card.Config.Icon;
        _name.text = card.Name;
        _level.text = card.Level.ToString();
        RenderStats(card);
    }

    private void RenderStats(ICardModel card)
    {
        foreach (var item in card.GetStatsText(true))
        {
            StatTextHolder statTextHolder = Instantiate(_statTextHolder, _statsParent);
            statTextHolder.RenderText(item);
            _statTextHolders.Add(statTextHolder);
        }
    }

    private void ClearStats()
    {
        if (_statTextHolders.Count == 0)
            return;

        foreach (var item in _statTextHolders)
        {
            _statTextHolders.Remove(item);
            Destroy(item.gameObject);
        }
    }

    protected override void OnClick()
    {
        if (_card.HasPlayer == false)
        {
            _playerCardHolder.Add(_card);
            _card.AddToPlayer();
            _card.Upgrade();
        }
        else
        {
            _card.Upgrade();
        }

        Selected?.Invoke();
    }
}
