using System;
using BigBalls.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class CardView : ButtonClickHandler
{
    [SerializeField] private Image _image;
    [SerializeField] private TMP_Text _name;
    [SerializeField] private TMP_Text _description;
    [SerializeField] private TMP_Text _stats;
    [SerializeField] private TMP_Text _level;

    private ITranslateService _translateService;
    private PlayerCardHolder _playerCardHolder;
    private ICardModel _card;

    public event Action Selected;

    [Inject]
    public void Construct (ITranslateService translateService)
    {
        _translateService = translateService;
    }

    public void Init (PlayerCardHolder playerCardHolder)
    {
        _playerCardHolder = playerCardHolder;
    }

    public void Render (ICardModel card)
    {
        _card = card;
        _image.sprite = card.Config.Icon;
        _name.text = card.Config.GetName(_translateService.CurrentLanguage);
        //_description.text = card.Config.GetDescription(_translateService.CurrentLanguage);
        //_stats.text = RenderStats();
        _level.text = card.Level.ToString();
    }

    private string RenderStats ()
    {
        return string.Empty;
    }

    protected override void OnClick ()
    {
        if (_card.Level == 1)
        {
            Debug.Log("add");
            _playerCardHolder.Add(_card);
            _card.Upgrade();
        }
        else
        {
            Debug.Log("Upgrade");
            _card.Upgrade();
        }

        Debug.Log(_card.Level);
        Selected?.Invoke();
    }
}
