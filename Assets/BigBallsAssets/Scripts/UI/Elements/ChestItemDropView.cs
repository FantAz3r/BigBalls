using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class ChestItemDropView : MonoBehaviour
{
    [Header("Chest References")]
    [SerializeField] private Image _chestImage;
    [SerializeField] private RectTransform _chestTransform;
    [SerializeField] private Button _openButton;
    [SerializeField] private CanvasGroup _chestCanvasGroup;
    [SerializeField] private ChestConfig _chestConfig;

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

    private void Awake ()
    {
        InitializePool();

        if (_openButton != null)
        {
            _openButton.onClick.AddListener(OnOpenChestClicked);
        }
    }

    private void OnDestroy ()
    {
        KillCurrentAnimation();
        DOTween.Kill(transform);
    }

    [Inject]
    public void Initialize (ChestCalculator chestCalculator)
    {
        _chestCalculator = chestCalculator;

        _chestImage.sprite = _chestConfig.CloseIcon;

        ResetChest();
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
            Destroy(card);
        }

        _activeCards.Clear();
    }

    private void InitializePool ()
    {
        for (int i = 0; i < _poolSize; i++)
        {
            ChestCardView card = Instantiate(_cardPrefab, _cardsContainer);
            card.gameObject.SetActive(false);
            _cardPool.Enqueue(card);
        }
    }

    private ChestCardView GetCard ()
    {
        ChestCardView card = Instantiate(_cardPrefab, _cardsContainer);
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

        // Звук открытия
        if (_audioSource != null && _openSound != null)
        {
            _currentAnimation.AppendCallback(() => _audioSource.PlayOneShot(_openSound));
        }

        // Анимация тряски сундука
        _currentAnimation.Append(_chestTransform.DOShakeRotation(_chestShakeDuration, _chestShakeStrength, 10, 90));
        _currentAnimation.Join(_chestTransform.DOShakeScale(_chestShakeDuration, 0.2f, 10, 90));

        _currentAnimation.AppendCallback(() =>
        {
            _chestImage.sprite = _chestCalculator.ChestConfig.OpenIcon;
        });

        // Небольшая пауза перед появлением карт
        _currentAnimation.AppendInterval(0.5f);

        // Появление карт
        _currentAnimation.AppendCallback(() => DropCards());

        // Автоматический сбор карт через заданное время
        _currentAnimation.AppendInterval(_cardDisplayDuration);
        _currentAnimation.AppendCallback(() => CollectAllCards());

        _currentAnimation.Play();
    }

    private void DropCards ()
    {
        ChestOpenResult result = _chestCalculator.OpenChest(_chestConfig);
        List<ICardModel> cards = result.Cards;
        int goldAmount = result.GoldAmount;

        // Создаем карты для выпавших предметов
        for (int i = 0; i < cards.Count; i++)
        {
            ChestCardView cardView = GetCard();
            cardView.RenderCard(cards[i]);


            // Анимация появления
            float delay = i * _cardAppearDelay;
            cardView.CreateAppearAnimation(delay);

            // Звук появления карты
            if (_audioSource != null && _cardAppearSound != null && i == 0)
            {
                DOVirtual.DelayedCall(delay, () => _audioSource.PlayOneShot(_cardAppearSound));
            }
        }

        // Показываем золото, если есть
        if (goldAmount > 0)
        {
            ShowGoldAnimation(goldAmount, cards.Count);
        }
    }

    private void ShowGoldAnimation (int amount, int cardCount)
    {
        // Создаем карту для золота
        ChestCardView goldCard = GetCard();
        float delay = cardCount * _cardAppearDelay;

        goldCard.RenderGold(_chestConfig.GoldSprite, amount);
        goldCard.CreateAppearAnimation(delay);

        // Звук золота
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
        if (_chestImage != null)
        {
            _currentAnimation.AppendCallback(() =>
            {
                _chestImage.sprite = _chestCalculator.ChestConfig.ShowIcon;
            });
        }

        // Затухание сундука
        _currentAnimation.Append(_chestCanvasGroup.DOFade(0f, 0.5f));

        // Собираем все карты
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
        // Здесь нужно реализовать логику поиска целевого слота для карты
        // Например, найти соответствующий слот в UI инвентаря
        // Пока возвращаем null для примера

        // Пример:
        // if (InventoryUI.Instance != null)
        // {
        //     return InventoryUI.Instance.GetSlotForCard(index);
        // }

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
