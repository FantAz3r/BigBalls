using System.Collections.Generic;
using System.Linq;
using BigBalls.Infrastructure.DI;
using BigBalls.Services;
using BigBalls.UI;
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

    private IWindowService _windowService;
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

    public void Init (PlayerCardHolder playerCardHolder)
    {
        _playerCardHolder = playerCardHolder;
    }

    public void OpenMenu ()
    {
        Open();
        _levelText.text = (_playerExperience.Stat.CurrentValue + 1).ToString();

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

    private void DestroyCards ()
    {
        if (_cardsButtons == null)
            return;

        foreach (var cardButton in _cardsButtons)
        {
            if (cardButton == null) continue;
            cardButton.Selected -= CloseMenu;
            cardButton.PlayDisappear();
        }

        _cardsButtons.Clear();
        _currentCards = null;
    }

    private void CloseCardMenu ()
    {
        Close();
        _timeService.SmoothEditTimeScalse(0, 0);
        _windowService.Open<HUD>();
        _timeService.SmoothEditTimeScalse(1, 1);
    }
}