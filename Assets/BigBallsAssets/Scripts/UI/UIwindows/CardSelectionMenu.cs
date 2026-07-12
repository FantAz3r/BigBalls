using BigBalls.Infrastructure.DI;
using BigBalls.Services;
using BigBalls.UI;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class CardSelectionMenu : PauseWindow
{
    [SerializeField] private RectTransform _buttonsParent;
    [SerializeField] private TMP_Text _levelText;

    private CardView _cardPrefab;
    private List<CardView> _cardsButtons;
    private CardSelector _selector;
    private List<ICardModel> _currentCards;

    private IWindowService _windowService;
    private ITimeService _timeService;
    private IObjectResolverProvider _objectResolverProvider;

    private IPlayerExperience _playerExperience;
    private PlayerCardHolder _playerCardHolder;

    private void Awake()
    {
        if (_playerExperience != null)
            _playerExperience.LevelUpped += Open;
    }

    private void OnDisable()
    {

        if (_cardsButtons == null)
            return;

        _cardsButtons.Clear();

        foreach (var button in _cardsButtons)
        {
            button.Selected -= CloseMenu;
            Destroy(button.gameObject);
        }
    }

    private void OnDestroy()
    {
        if (_playerExperience != null)
            _playerExperience.LevelUpped -= Open;
    }

    [Inject]
    public void Construct(
        IWindowService windowService,
        IResourceLoader resourceLoader,
        CardSelector cardSelector,
        IObjectResolverProvider objectResolverProvider,
        IPlayerExperience playerExperienceModel,
        ITimeService timeService)
    {
        _objectResolverProvider = objectResolverProvider;
        _windowService = windowService;
        _selector = cardSelector;
        _playerExperience = playerExperienceModel;
        _cardPrefab = resourceLoader.Load<CardView>();
        _timeService = timeService;
    }

    public void Init(PlayerCardHolder playerCardHolder)
    {
        _playerCardHolder = playerCardHolder;
    }

    public void OpenMenu()
    {
        _levelText.text = (_playerExperience.Stat.CurrentValue + 1).ToString();

        if (_currentCards == null)
        {
            _currentCards = _selector.GetCardsForSelection().ToList();
        }

        if (_currentCards.Count == 0)
        {
            return;
        }

        _cardsButtons = CreateCards();
        ShowCards(_currentCards);
    }

    public void CloseMenu()
    {
        _selector.SaveCurrentCards(null);
        DestroyCards();
        CloseCardMenu();
    }

    public void PostponeChoise()
    {
        _selector.SaveCurrentCards(_currentCards);
        CloseCardMenu();
    }

    private void ShowCards(List<ICardModel> cards)
    {
        for (int i = 0; i < _cardsButtons.Count; i++)
        {
            if (i < cards.Count)
            {
                _cardsButtons[i].gameObject.SetActive(true);
                _cardsButtons[i].Render(cards[i]);
            }
            else
            {
                _cardsButtons[i].gameObject.SetActive(false);
            }
        }
    }

    private List<CardView> CreateCards()
    {
        int maxCardCount = 3;
        List<CardView> cards = new List<CardView>();

        for (int i = 0; i < maxCardCount; i++)
        {
            CardView card = _objectResolverProvider.CurrentResolver.Instantiate(_cardPrefab, _buttonsParent);
            card.Init(_playerCardHolder);
            cards.Add(card);
            card.Selected += CloseMenu;
        }

        return cards;
    }

    private void DestroyCards()
    {
        foreach (var cardButton in _cardsButtons)
        {
            cardButton.Selected -= CloseMenu;
            Destroy(cardButton.gameObject);
        }

        _currentCards = null;
    }

    private void CloseCardMenu()
    {
        Close();
        _timeService.SmoothEditTimeScalse(0, 0);
        _windowService.Open<HUD>();
        _timeService.SmoothEditTimeScalse(1, 1);
    }
}
