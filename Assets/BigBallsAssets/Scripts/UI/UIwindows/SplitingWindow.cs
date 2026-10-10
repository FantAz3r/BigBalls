using System.Collections.Generic;
using System.Linq;
using BigBalls.Services;
using BigBalls.UI;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class SplitingWindow : MonoBehaviour
{
    private readonly List<SplitingCard> _spawnedCards = new();

    [Header("Upgrades")]
    [SerializeField] private int _minUpgrades = 1;
    [SerializeField] private int _maxUpgrades = 5;

    [SerializeField] private Sprite _goldSprite;

    [Header("UI")]
    [SerializeField] private SplitingCard _slotPrefab;
    [SerializeField] private RectTransform _upgradeContainer;
    [SerializeField] private Button _continueButton;

    [Header("Animation")]
    [SerializeField] private float _circleRadius = 250f;
    [SerializeField] private float _animationDuration = 0.6f;
    [SerializeField] private float _delayBetweenCards = 0.1f;
    [SerializeField] private Ease _animationEase = Ease.OutBack;

    private int _spawnCountOffset = 2;
    private SplitingPanel _splitingPanel;
    private PlayerCardHolder _cardHolder;
    private IPoolService _poolService;
    private IUIFactory _windowService;
    private IWalletModel _wallet;

    [field: SerializeField] public int MinGoldReward { get; private set; } = 75;
    [field: SerializeField] public int MaxGoldReward { get; private set; } = 150;

    [Inject]
    public void Construct (
        IPoolService poolService,
        IUIFactory windowService,
        IWalletModel wallet)
    {
        _poolService = poolService;
        _windowService = windowService;
        _wallet = wallet;
    }

    public void Init (PlayerCardHolder playerCardHolder)
    {
        _cardHolder = playerCardHolder;
        _splitingPanel = _windowService.Get<SplitingPanel>(WindowType.MergeWindow);
    }

    public void Open ()
    {
        ShowUpgrads();
        _continueButton.gameObject.SetActive(false);
    }

    public void Close ()
    {
        _windowService.Get<HUD>(WindowType.HUD).Open();
        _splitingPanel.Close();
        ClearStats();
    }

    public bool CalculateCardsCount (out int minCount, out int maxCount, out List<ICardModel> cards)
    {
        minCount = 0;
        maxCount = 0;
        cards = _cardHolder.Cards
           .Where(card => card.MaxNoneGameLevel - card.InGameLevel > 0)
           .ToList();

        if (cards.Count == 0)
        {
            return false;
        }

        int availableLevels = cards.Sum(card => card.MaxNoneGameLevel - card.InGameLevel);

        maxCount = Mathf.Min(_maxUpgrades, availableLevels);
        minCount = Mathf.Max(_minUpgrades, maxCount - _spawnCountOffset);
        return true;
    }

    private void ShowUpgrads ()
    {
        ClearStats();

        if (CalculateCardsCount(out int minCount, out int maxCount, out List<ICardModel> cards) == false)
        {
            ShowGoldReward();
        }

        int totalUpgrades = Random.Range(minCount, maxCount + 1);
        var selectedCards = new List<ICardModel>();

        for (int i = 0; i < totalUpgrades; i++)
        {
            var candidates = cards
                .Where(card => card.MaxNoneGameLevel - card.InGameLevel > 0)
                .ToList();

            if (candidates.Count == 0)
                break;

            ICardModel target = candidates[Random.Range(0, candidates.Count)];

            target.Upgrade();
            selectedCards.Add(target);
        }

        SpawnCardsInCircle(selectedCards);
    }


    private void SpawnCardsInCircle (List<ICardModel> cards)
    {
        int cardCount = cards.Count;

        if (cardCount == 0)
            return;

        for (int i = 0; i < cardCount; i++)
        {
            SplitingCard card = CreateCard(cards[i].Config.Icon);
            AnimateCard(card, i, cardCount);
        }

        EnableContinueButtonAfter(cardCount);
    }

    private void ShowGoldReward ()
    {
        int goldAmount = Random.Range(MinGoldReward, MaxGoldReward + 1);

        SplitingCard card = CreateCard(_goldSprite, goldAmount);
        AnimateCard(card, 0, 1);
        EnableContinueButtonAfter(1);

        _wallet.AddCoins(goldAmount);
    }

    private SplitingCard CreateCard (Sprite icon, int amount = 0)
    {
        SplitingCard card = _poolService.Get(_slotPrefab, _upgradeContainer);
        card.RenderReward(icon, amount);
        _spawnedCards.Add(card);

        return card;
    }

    private void AnimateCard (SplitingCard card, int index, int cardCount)
    {
        RectTransform cardTransform = card.GetComponent<RectTransform>();

        cardTransform.anchoredPosition = Vector2.zero;
        cardTransform.localScale = Vector3.zero;

        Vector2 targetPosition = CalculateCirclePosition(index, cardCount, _circleRadius);

        DOTween.Sequence()
            .AppendInterval(index * _delayBetweenCards)
            .Append(cardTransform
                    .DOAnchorPos(targetPosition, _animationDuration)
                    .SetEase(_animationEase))
            .Join(cardTransform
                    .DOScale(Vector3.one, _animationDuration)
                    .SetEase(Ease.OutBack))
            .SetUpdate(true);
    }

    private void EnableContinueButtonAfter (int cardCount)
    {
        float delay = (cardCount - 1) * _delayBetweenCards + _animationDuration;

        DOVirtual.DelayedCall(delay, () => _continueButton.gameObject.SetActive(true))
            .SetUpdate(true);
    }

    private Vector2 CalculateCirclePosition (int index, int cardCount, float radius)
    {
        float angleStep = 360f / cardCount;
        float angle = 90f - index * angleStep;
        float angleInRadians = angle * Mathf.Deg2Rad;

        return new Vector2(
            Mathf.Cos(angleInRadians) * radius,
            Mathf.Sin(angleInRadians) * radius);
    }

    private void ClearStats ()
    {
        DOTween.Kill(_upgradeContainer);

        foreach (SplitingCard card in _spawnedCards)
        {
            if (card == null)
                continue;

            card.transform.DOKill();
            card.gameObject.SetActive(false);
            _poolService.Release(card);
        }

        _spawnedCards.Clear();

        for (int i = _upgradeContainer.childCount - 1; i >= 0; i--)
        {
            Transform child = _upgradeContainer.GetChild(i);

            if (child.gameObject.activeSelf)
                child.gameObject.SetActive(false);
        }
    }
}
