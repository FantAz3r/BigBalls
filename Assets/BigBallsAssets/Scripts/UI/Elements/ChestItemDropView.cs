<<<<<<< HEAD
using BigBalls.UI;
using DG.Tweening;
using System.Collections.Generic;
=======
using System.Collections.Generic;
using DG.Tweening;
>>>>>>> f0e424ab67e4621452446e379326bfb4b7df25bf
using UnityEngine;
using UnityEngine.UI;
using VContainer;

<<<<<<< HEAD
public class ChestItemDropView : WindowBase
=======
public class ChestItemDropView : MonoBehaviour
>>>>>>> f0e424ab67e4621452446e379326bfb4b7df25bf
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
<<<<<<< HEAD
    private ChestModel _chestModel;

    private void Awake()
    {
        CreateCards();
    }

    private void OnDestroy()
=======

    private void Awake ()
    {
        InitializePool();

        if (_openButton != null)
        {
            _openButton.onClick.AddListener(OnOpenChestClicked);
        }
    }

    private void OnDestroy ()
>>>>>>> f0e424ab67e4621452446e379326bfb4b7df25bf
    {
        KillCurrentAnimation();
        DOTween.Kill(transform);
    }

    [Inject]
<<<<<<< HEAD
    public void Construct(ChestCalculator chestCalculator) => _chestCalculator = chestCalculator;

    public void Init(ChestModel chestModel)
    {
        Debug.Log("inited");
        _chestModel = chestModel;
        _chestImage.sprite = _chestModel.Config.CloseIcon;
        ResetChest();

        if (_openButton != null)
        {
            _openButton.onClick.AddListener(OnOpenChestClicked);
    }
    }

    public void ForceComplete()
=======
    public void Initialize (ChestCalculator chestCalculator)
    {
        _chestCalculator = chestCalculator;

        _chestImage.sprite = _chestConfig.CloseIcon;

        ResetChest();
    }

    public void ForceComplete ()
>>>>>>> f0e424ab67e4621452446e379326bfb4b7df25bf
    {
        if (_currentAnimation != null)
        {
            _currentAnimation.Complete(true);
        }
    }

<<<<<<< HEAD
    private void ResetChest()
=======
    private void ResetChest ()
>>>>>>> f0e424ab67e4621452446e379326bfb4b7df25bf
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

<<<<<<< HEAD
    private void CreateCards()
=======
    private void InitializePool ()
>>>>>>> f0e424ab67e4621452446e379326bfb4b7df25bf
    {
        for (int i = 0; i < _poolSize; i++)
        {
            ChestCardView card = Instantiate(_cardPrefab, _cardsContainer);
            card.gameObject.SetActive(false);
            _cardPool.Enqueue(card);
        }
    }

<<<<<<< HEAD
    private ChestCardView GetCard()
=======
    private ChestCardView GetCard ()
>>>>>>> f0e424ab67e4621452446e379326bfb4b7df25bf
    {
        ChestCardView card = Instantiate(_cardPrefab, _cardsContainer);
        _activeCards.Add(card);
        return card;
    }

<<<<<<< HEAD
    private void OnOpenChestClicked()
=======
    private void OnOpenChestClicked ()
>>>>>>> f0e424ab67e4621452446e379326bfb4b7df25bf
    {
        _openButton.interactable = false;
        AnimateChestOpening();
    }

<<<<<<< HEAD
    private void AnimateChestOpening()
=======
    private void AnimateChestOpening ()
>>>>>>> f0e424ab67e4621452446e379326bfb4b7df25bf
    {
        KillCurrentAnimation();

        _currentAnimation = DOTween.Sequence();

<<<<<<< HEAD
        // Устанавливаем независимость от Time.timeScale для всей последовательности
        _currentAnimation.SetUpdate(true);

=======
>>>>>>> f0e424ab67e4621452446e379326bfb4b7df25bf
        // Звук открытия
        if (_audioSource != null && _openSound != null)
        {
            _currentAnimation.AppendCallback(() => _audioSource.PlayOneShot(_openSound));
        }

        // Анимация тряски сундука
<<<<<<< HEAD
        _currentAnimation.Append(
            _chestTransform.DOShakeRotation(_chestShakeDuration, _chestShakeStrength, 10, 90)
                .SetUpdate(true)  // Тряска не зависит от timeScale
        );

        _currentAnimation.Join(
            _chestTransform.DOShakeScale(_chestShakeDuration, 0.2f, 10, 90)
                .SetUpdate(true)  // Тряска не зависит от timeScale
        );

        _currentAnimation.AppendCallback(() =>
        {
            _chestImage.sprite = _chestCalculator.ChestModel.Config.OpenIcon;
=======
        _currentAnimation.Append(_chestTransform.DOShakeRotation(_chestShakeDuration, _chestShakeStrength, 10, 90));
        _currentAnimation.Join(_chestTransform.DOShakeScale(_chestShakeDuration, 0.2f, 10, 90));

        _currentAnimation.AppendCallback(() =>
        {
            _chestImage.sprite = _chestCalculator.ChestConfig.OpenIcon;
>>>>>>> f0e424ab67e4621452446e379326bfb4b7df25bf
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

<<<<<<< HEAD
    private void DropCards()
    {
        ChestOpenResult result = _chestCalculator.OpenChest(_chestModel);
=======
    private void DropCards ()
    {
        ChestOpenResult result = _chestCalculator.OpenChest(_chestConfig);
>>>>>>> f0e424ab67e4621452446e379326bfb4b7df25bf
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

<<<<<<< HEAD
    private void ShowGoldAnimation(int amount, int cardCount)
=======
    private void ShowGoldAnimation (int amount, int cardCount)
>>>>>>> f0e424ab67e4621452446e379326bfb4b7df25bf
    {
        // Создаем карту для золота
        ChestCardView goldCard = GetCard();
        float delay = cardCount * _cardAppearDelay;

<<<<<<< HEAD
        goldCard.RenderGold(_chestModel.Config.GoldSprite, amount);
=======
        goldCard.RenderGold(_chestConfig.GoldSprite, amount);
>>>>>>> f0e424ab67e4621452446e379326bfb4b7df25bf
        goldCard.CreateAppearAnimation(delay);

        // Звук золота
        if (_audioSource != null && _goldSound != null)
        {
            DOVirtual.DelayedCall(delay, () => _audioSource.PlayOneShot(_goldSound));
        }
    }

<<<<<<< HEAD
    private void CollectAllCards()
=======
    private void CollectAllCards ()
>>>>>>> f0e424ab67e4621452446e379326bfb4b7df25bf
    {
        KillCurrentAnimation();

        _currentAnimation = DOTween.Sequence();

        // Смена спрайта на ShowIcon
        if (_chestImage != null)
        {
            _currentAnimation.AppendCallback(() =>
            {
<<<<<<< HEAD
                _chestImage.sprite = _chestCalculator.ChestModel.Config.ShowIcon;
=======
                _chestImage.sprite = _chestCalculator.ChestConfig.ShowIcon;
>>>>>>> f0e424ab67e4621452446e379326bfb4b7df25bf
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
<<<<<<< HEAD

=======
>>>>>>> f0e424ab67e4621452446e379326bfb4b7df25bf
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

<<<<<<< HEAD
    private Transform FindTargetForCard(int index)
=======
    private Transform FindTargetForCard (int index)
>>>>>>> f0e424ab67e4621452446e379326bfb4b7df25bf
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

<<<<<<< HEAD
    private void KillCurrentAnimation()
=======
    private void KillCurrentAnimation ()
>>>>>>> f0e424ab67e4621452446e379326bfb4b7df25bf
    {
        if (_currentAnimation != null && _currentAnimation.IsActive())
        {
            _currentAnimation.Kill();
        }
    }
}
