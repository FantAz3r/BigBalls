using System.Collections.Generic;
using System.Linq;
using BigBalls.Infrastructure.DI;
using BigBalls.Localization;
using BigBalls.Services;
using BigBalls.UI;
using DG.Tweening;
using TMPro;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class CardSelectionMenu : PauseWindow
{
    [SerializeField] private RectTransform _buttonsParent;
    [SerializeField] private TMP_Text _levelText;

    [Header("Appear Stagger")]
    [SerializeField] private float _appearStagger = 0.08f;

    private CardView _cardPrefab;
    private List<CardView> _cardsButtons;
    private CardSelector _selector;
    private List<ICardModel> _currentCards;

    private IUIFactory _uIFactory;
    private ITimeService _timeService;
    private IObjectResolverProvider _objectResolverProvider;
    private IPlayerExperience _playerExperience;
    private PlayerCardHolder _playerCardHolder;

    private void Awake ()
    {
        if (_playerExperience != null)
            _playerExperience.LevelUpped += OpenMenu;
    }

    private void OnDisable ()
    {
        if (_cardsButtons == null)
            return;

        foreach (var button in _cardsButtons)
        {
            if (button == null) continue;
            button.Selected -= CloseMenu;
        }

        _cardsButtons.Clear();
    }

    private void OnDestroy ()
    {
        if (_playerExperience != null)
            _playerExperience.LevelUpped -= OpenMenu;
    }

    [Inject]
    public void Construct (
        IWindowService windowService,
        IUIFactory uIFactory,
        IResourceLoader resourceLoader,
        CardSelector cardSelector,
        IObjectResolverProvider objectResolverProvider,
        IPlayerExperience playerExperienceModel,
        ITimeService timeService)
    {
        _uIFactory = uIFactory;
        _objectResolverProvider = objectResolverProvider;
        _selector = cardSelector;
        _playerExperience = playerExperienceModel;
        _cardPrefab = resourceLoader.Load<CardView>();
        _timeService = timeService;
    }

    public void Init (PlayerCardHolder playerCardHolder)
    {
        _playerCardHolder = playerCardHolder;
        _selector.Init(_playerCardHolder);
    }

    public void OpenMenu ()
    {
        _uIFactory.Get<HUD>(WindowType.HUD).Close();
        base.Open();
        _levelText.text = $"{TextLocalizator.LevelUP} {_playerExperience.Level + 1}";

        if (_currentCards == null)
        {
            _currentCards = _selector.GetCardsForSelection().ToList();
        }

        if (_currentCards.Count == 0)
            return;

        _cardsButtons = CreateCards();
        ShowCards(_currentCards);
    }

    public void CloseMenu (CardView useles)
    {
        _selector.SaveCurrentCards(null);
        DestroyCards();
        CloseCardMenu();
    }

    public void PostponeChoise ()
    {
        _selector.SaveCurrentCards(_currentCards);
        CloseCardMenu();
    }

    public void Reroll ()
    {
        _selector.SaveCurrentCards(null);
        _currentCards = _selector.GetCardsForSelection().ToList();

        if (_currentCards.Count == 0)
            return;

        if (_cardsButtons == null || _cardsButtons.Count == 0)
        {
            _cardsButtons = CreateCards();
            ShowCards(_currentCards);
            return;
        }

        HideCards(() =>
        {
            ShowCards(_currentCards);
        });
    }

    private void HideCards (System.Action onComplete)
    {
        int remaining = _cardsButtons.Count;

        if (remaining == 0)
        {
            onComplete?.Invoke();
            return;
        }

        foreach (var card in _cardsButtons)
        {
            if (card == null)
            {
                remaining--;
                if (remaining <= 0) onComplete?.Invoke();
                continue;
            }

            card.Selected -= CloseMenu;
            card.PlayDisappear(() =>
            {
                remaining--;
                if (remaining <= 0)
                {
                    _cardsButtons.Clear();
                    onComplete?.Invoke();
                }
            });
        }
    }

    private void ShowCards (List<ICardModel> cards)
    {
        if (_cardsButtons == null || _cardsButtons.Count == 0)
            _cardsButtons = CreateCards();

        for (int i = 0; i < _cardsButtons.Count; i++)
        {
            if (i < cards.Count)
            {
                _cardsButtons[i].gameObject.SetActive(true);
                _cardsButtons[i].Render(cards[i]);
                _cardsButtons[i].PlayAppear(delay: i * _appearStagger);
                _cardsButtons[i].Selected += CloseMenu;
                _cardsButtons[i].Rewarded += Rerender;
            }
            else
            {
                _cardsButtons[i].gameObject.SetActive(false);
            }
        }
    }

    private List<CardView> CreateCards ()
    {
        int maxCardCount = 3;
        List<CardView> cards = new List<CardView>();

        for (int i = 0; i < maxCardCount; i++)
        {
            CardView card = _objectResolverProvider.CurrentResolver.Instantiate(_cardPrefab, _buttonsParent);
            card.Init(_playerCardHolder);
            cards.Add(card);
        }

        return cards;
    }

    private void Rerender ()
    {
        foreach (CardView card in _cardsButtons)
            card.Render(card.Model);
    }

    private void DestroyCards ()
    {
        if (_cardsButtons == null)
            return;

        foreach (var cardButton in _cardsButtons)
        {
            if (cardButton == null) continue;
            cardButton.Selected -= CloseMenu;
            cardButton.Rewarded -= Rerender;
            cardButton.PlayDisappear();
        }

        _cardsButtons.Clear();
        _currentCards = null;
    }

    private void CloseCardMenu ()
    {
        Close();
        _timeService.SmoothEditTimescale(0, 0);
        _uIFactory.Get<HUD>(WindowType.HUD).Open();
        _timeService.SmoothRecoverTimescale(1);
    }
}