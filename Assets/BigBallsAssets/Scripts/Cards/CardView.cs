using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using BigBalls.Localization;
using BigBalls.Services;
using BigBalls.UI;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VContainer;

public class CardView : ButtonClickHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private const string CardReward = "CardReward";

    [SerializeField] private Image _image;
    [SerializeField] private Image _background;
    [SerializeField] private TMP_Text _name;
    [SerializeField] private TMP_Text _level;
    [SerializeField] private TMP_Text _descrioptionText;
    [SerializeField] private Image _descrioptionPanel;
    [SerializeField] private Toggle _toggle;
    [SerializeField] private AddButton _advButton;

    [SerializeField] private RectTransform _statsParent;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private SoundStruct _appearSpound;
    [SerializeField] private SerializedDictionary<Rarity, Color> _colorMap = new();

    [Header("Appear/Disappear Animation")]
    [SerializeField] private float _appearDuration = 0.3f;
    [SerializeField] private float _disappearDuration = 0.2f;
    [SerializeField] private float _appearStartScale = 0.6f;
    [SerializeField] private float _appearStartOffsetY = -40f;
    [SerializeField] private float _clickPopScale = 1.08f;
    [SerializeField] private float _clickPopDuration = 0.08f;

    [Header("Hover Animation")]
    [SerializeField] private float _hoverScale = 1.06f;
    [SerializeField] private float _hoverDuration = 0.12f;
    [SerializeField] private float _hoverOffsetY = 8f;

    private IPoolService _poolService;
    private IADVServise _advServise;
    private PlayerCardHolder _playerCardHolder;
    private StatTextHolder _statTextHolder;
    private List<StatTextHolder> _statTextHolders = new();
    private SoundSpawner _soundSpawner;

    private RectTransform _rectTransform;
    private Vector2 _restAnchoredPos;
    private bool _isHovered;
    private bool _isAnimating;


    public event Action<CardView> Selected;
    public event Action Rewarded;
    public ICardModel Model { get; private set; }
    public AddButton AdvButton => _advButton;

    [Inject]
    public void Construct (IResourceLoader resourceLoader, IPoolService poolService, SoundSpawner soundSpawner, IADVServise advServise)
    {
        _poolService = poolService;
        _soundSpawner = soundSpawner;
        _advServise = advServise;
        _statTextHolder = resourceLoader.Load<StatTextHolder>();
    }

    private void Awake ()
    {
        _rectTransform = transform as RectTransform;
    }

    private void OnEnable ()
    {
        transform.localScale = Vector3.one;

        if (_canvasGroup != null)
            _canvasGroup.alpha = 1f;

        _isHovered = false;
        _isAnimating = false;
    }

    public void Init (PlayerCardHolder playerCardHolder)
    {
        _playerCardHolder = playerCardHolder;
    }

    public void Render (ICardModel card)
    {
        ClearStats();
        Model = card;
        _background.color = _colorMap[card.Config.Rarity];
        _image.sprite = card.Config.Icon;
        _name.text = card.Name;
        _level.text = card.HasPlayer ? (card.Level + 1).ToString() : TextLocalizator.New;
        _toggle.isOn = card.HasPlayer == false;
        _descrioptionPanel.gameObject.SetActive(card.HasPlayer == false);

        _advButton.gameObject.SetActive(_advServise.CanShowRewardADV(CardReward));
        _advButton.SetRewardAction(SelectCard, CardReward);

        _descrioptionText.text = card.Description;
        RenderStats(card);
    }

    public void SelectCard ()
    {
        if (Model.HasPlayer == false)
        {
            _playerCardHolder.Add(Model);
            Model.AddToPlayer();
        }
        else
        {
            Model.Upgrade();
        }

        Rewarded?.Invoke();
        transform.gameObject.SetActive(false);
    }

    public void PlayAppear (float delay = 0f)
    {
        transform.DOKill();
        if (_canvasGroup != null)
            _canvasGroup.DOKill();

        _isAnimating = true;
        _isHovered = false;

        transform.localScale = Vector3.one * _appearStartScale;

        if (_canvasGroup != null)
            _canvasGroup.alpha = 0f;

        Sequence seq = DOTween.Sequence()
            .SetUpdate(true);

        seq.AppendInterval(delay);
        seq.AppendCallback(() => _soundSpawner.Spawn(_appearSpound));
        seq.Append(transform.DOScale(Vector3.one, _appearDuration)
            .SetEase(Ease.OutBack));

        if (_canvasGroup != null)
            seq.Join(_canvasGroup.DOFade(1f, _appearDuration * 0.6f)
                .SetEase(Ease.OutQuad));

        seq.OnComplete(() => _isAnimating = false);
    }

    public void PlayDisappear (Action onComplete = null)
    {
        transform.DOKill();
        if (_canvasGroup != null)
            _canvasGroup.DOKill();

        _isAnimating = true;
        _isHovered = false;

        Sequence seq = DOTween.Sequence()
            .SetUpdate(true);

        seq.Join(transform.DOScale(Vector3.one * _appearStartScale, _disappearDuration)
            .SetEase(Ease.InBack));

        if (_canvasGroup != null)
            seq.Join(_canvasGroup.DOFade(0f, _disappearDuration).SetEase(Ease.InQuad));

        seq.OnComplete(() =>
        {
            onComplete?.Invoke();
            Destroy(gameObject);
        });
    }

    public void OnPointerEnter (PointerEventData eventData)
    {
        _isHovered = true;

        if (_isAnimating)
            return;

        PlayHoverEnter();
    }

    public void OnPointerExit (PointerEventData eventData)
    {
        _isHovered = false;

        if (_isAnimating)
            return;

        PlayHoverExit();
    }

    private void PlayHoverEnter ()
    {
        transform.DOKill();
        transform.localScale = Vector3.one;

        transform.DOScale(Vector3.one * _hoverScale, _hoverDuration)
            .SetEase(Ease.OutQuad)
            .SetUpdate(true);
    }

    private void PlayHoverExit ()
    {
        transform.DOKill();
        transform.localScale = Vector3.one * _hoverScale;

        transform.DOScale(Vector3.one, _hoverDuration)
            .SetEase(Ease.OutQuad)
            .SetUpdate(true);
    }

    private void PlayClickPop ()
    {
        transform.DOKill();
        transform.localScale = Vector3.one;

        transform.DOScale(Vector3.one * _clickPopScale, _clickPopDuration)
            .SetEase(Ease.OutQuad)
            .SetUpdate(true)
            .OnComplete(() =>
            {
                transform.DOScale(Vector3.one, _clickPopDuration)
                    .SetEase(Ease.InQuad)
                    .SetUpdate(true);
            });
    }

    private void RenderStats (ICardModel card)
    {
        foreach (var item in card.GetStatsText(card.HasPlayer))
        {
            StatTextHolder statTextHolder = _poolService.Get(_statTextHolder, _statsParent);
            statTextHolder.RenderText(item, card.HasPlayer);
            _statTextHolders.Add(statTextHolder);
        }
    }

    private void ClearStats ()
    {
        foreach (var item in _statTextHolders)
        {
            if (item != null)
                _poolService.Release(item);
        }

        _statTextHolders.Clear();
    }

    public void OnPointerClick (PointerEventData eventData)
    {
        PlayClickPop();
        SelectCard();
        Selected?.Invoke(this);
    }
}