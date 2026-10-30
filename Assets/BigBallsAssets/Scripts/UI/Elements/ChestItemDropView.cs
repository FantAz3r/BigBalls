using System.Collections.Generic;
using BigBalls.Services;
using BigBalls.UI;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class ChestItemDropView : WindowBase
{
    [Header("Chest References")]
    [SerializeField] private Image _closeChestImage;
    [SerializeField] private Image _openChestImage;
    [SerializeField] private RectTransform _chestTransform;
    [SerializeField] private Button _openButton;
    [SerializeField] private CanvasGroup _chestCanvasGroup;
    [SerializeField] private ChestConfig _chestConfig;
    [SerializeField] private Button _exitButton;

    [Header("Cards Container")]
    [SerializeField] private Transform _cardsContainer;
    [SerializeField] private ChestCardView _cardPrefab;
    [SerializeField] private int _poolSize = 10;

    [Header("Animation Settings")]
    [SerializeField] private float _chestShakeDuration = 0.5f;
    [SerializeField] private float _chestShakeStrength = 10f;
    [SerializeField] private float _cardAppearDelay = 0.3f;
    [SerializeField] private float _cardSpacing = 150f;
    [SerializeField] private float _cardDisplayDuration = 2f;

    [Header("Sounds")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _openSound;
    [SerializeField] private AudioClip _cardAppearSound;
    [SerializeField] private AudioClip _goldSound;

    private Queue<ChestCardView> _cardPool = new Queue<ChestCardView>();
    private List<ChestCardView> _activeCards = new List<ChestCardView>();
    private ChestCalculator _chestCalculator;
    private Sequence _currentAnimation;
    private ChestModel _chestModel;
    private IPoolService _poolService;
    private ITimeService _timeService;
    private LevelButton _levelButton;

    private void Awake ()
    {
        CreateCards();
    }

    private void OnDestroy ()
    {
        KillCurrentAnimation();
        DOTween.Kill(transform);
    }

    public override void Open ()
    {
        base.Open();
        _timeService.StopGame();
        _levelButton?.gameObject.SetActive(true);
    }

    [Inject]
    public void Construct (ChestCalculator chestCalculator, IPoolService poolService, ITimeService timeService)
    {
        _poolService = poolService;
        _chestCalculator = chestCalculator;
        _timeService = timeService;
    }

    public void Init (ChestModel chestModel, LevelButton levelButton)
    {
        _chestModel = chestModel;
        _levelButton = levelButton;
        _closeChestImage.sprite = _chestModel.Config.CloseIcon;
        _openChestImage.sprite = _chestModel.Config.OpenIcon;
        _openChestImage.gameObject.SetActive(false);
        _exitButton.gameObject.SetActive(false);
        ResetChest();

        if (_openButton != null)
        {
            _openButton.onClick.AddListener(OnOpenChestClicked);
        }
    }

    public void ForceComplete ()
    {
        if (_currentAnimation != null)
        {
            _currentAnimation.Complete(true);
        }
    }

    private void ResetChest ()
    {
        _chestTransform.localScale = Vector3.one;
        _chestCanvasGroup.alpha = 1f;
        _openButton.interactable = true;

        foreach (var card in _activeCards)
        {
            _poolService.Release(card);
        }

        _activeCards.Clear();
    }

    private void CreateCards ()
    {
        for (int i = 0; i < _poolSize; i++)
        {
            ChestCardView card = _poolService.Get(_cardPrefab, _cardsContainer);
            card.gameObject.SetActive(false);
            _cardPool.Enqueue(card);
        }
    }

    private ChestCardView GetCard ()
    {
        ChestCardView card = _poolService.Get(_cardPrefab, _cardsContainer);
        _activeCards.Add(card);
        return card;
    }

    private void OnOpenChestClicked ()
    {
        _openButton.interactable = false;
        AnimateChestOpening();
    }

    private void AnimateChestOpening ()
    {
        KillCurrentAnimation();
        _currentAnimation = DOTween.Sequence();
        _currentAnimation.SetUpdate(true);

        if (_audioSource != null && _openSound != null)
        {
            _currentAnimation.AppendCallback(() => _audioSource.PlayOneShot(_openSound));
        }

        _currentAnimation.Append(
            _chestTransform.DOShakeRotation(_chestShakeDuration, _chestShakeStrength, 10, 90)
                .SetUpdate(true)
        );

        _currentAnimation.Join(
            _chestTransform.DOShakeScale(_chestShakeDuration, 0.2f, 10, 90)
                .SetUpdate(true)
        );

        _currentAnimation.AppendCallback(() =>
        {
            _openChestImage.gameObject.SetActive(true);
            _closeChestImage.gameObject.SetActive(false);
        });

        _currentAnimation.AppendInterval(0.5f);
        _currentAnimation.AppendCallback(() => DropCards());
        _currentAnimation.AppendInterval(_cardDisplayDuration);
        _currentAnimation.AppendCallback(() => { _exitButton.gameObject.SetActive(true); });
        _currentAnimation.AppendCallback(() => CollectAllCards());
        _currentAnimation.Play();
    }

    private void DropCards ()
    {
        ChestOpenResult result = _chestCalculator.OpenChest(_chestModel);

        List<ICardModel> cards = result.Cards;
        Dictionary<ICardModel, int> cardDrop = new Dictionary<ICardModel, int>();

        foreach (var card in cards)
        {
            if (cardDrop.ContainsKey(card))
            {
                cardDrop[card]++;
            }
            else
            {
                cardDrop.Add(card, 1);
            }
        }

        int goldAmount = result.GoldAmount;
        int i = 0;

        foreach (var card in cardDrop)
        {
            ChestCardView cardView = GetCard();

            cardView.RenderCard(card.Key, card.Value);
            card.Key.AddItemEXP(card.Value);

            i++;
            float delay = i * _cardAppearDelay;
            cardView.CreateAppearAnimation(delay);

            if (_audioSource != null && _cardAppearSound != null && i == 0)
            {
                DOVirtual.DelayedCall(delay, () => _audioSource.PlayOneShot(_cardAppearSound));
            }
        }

        if (goldAmount > 0)
        {
            ShowGoldAnimation(goldAmount, cardDrop.Count);
        }
    }

    private void ShowGoldAnimation (int amount, int cardCount)
    {
        ChestCardView goldCard = GetCard();
        float delay = (cardCount + 1) * _cardAppearDelay;

        goldCard.RenderGold(_chestModel.Config.GoldSprite, amount);
        goldCard.CreateAppearAnimation(delay);

        if (_audioSource != null && _goldSound != null)
        {
            DOVirtual.DelayedCall(delay, () => _audioSource.PlayOneShot(_goldSound));
        }
    }

    private void CollectAllCards ()
    {
        KillCurrentAnimation();

        _currentAnimation = DOTween.Sequence();

        // Смена спрайта на ShowIcon
        if (_closeChestImage != null)
        {
            _currentAnimation.AppendCallback(() =>
            {
                _closeChestImage.sprite = _chestCalculator.ChestModel.Config.ShowIcon;

            });
        }

        for (int i = _activeCards.Count - 1; i >= 0; i--)
        {
            ChestCardView card = _activeCards[i];

            // Находим целевой трансформ для этой карты (например, слот в инвентаре)
            Transform target = FindTargetForCard(i);

            if (target != null)
            {
                float delay = (_activeCards.Count - 1 - i) * 0.2f;
                Sequence collectSequence = card.CreateCollectAnimation(target, 0.5f);
                collectSequence.SetDelay(delay);
                collectSequence.OnComplete(() => Destroy(card));
                _currentAnimation.Join(collectSequence);
            }
        }

        _currentAnimation.Play();
    }

    private Transform FindTargetForCard (int index)
    {

        return null;
    }

    private void KillCurrentAnimation ()
    {
        if (_currentAnimation != null && _currentAnimation.IsActive())
        {
            _currentAnimation.Kill();
        }
    }
}
