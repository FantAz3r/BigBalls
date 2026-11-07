using System;
using System.Collections.Generic;
using BigBalls.GameplayObjects;
using BigBalls.Localization;
using BigBalls.Services;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class UpgradePanel : MonoBehaviour
{
    private const string None = "???";

    [SerializeField] private Image _image;
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _descriptionText;
    [SerializeField] private TMP_Text _level;
    [SerializeField] private ItemUpgradeButton _upgradeButton;
    [SerializeField] private RectTransform _statsParent;
    [SerializeField] private Slider _expSlider;
    [SerializeField] private RectTransform _costPanel;
    [SerializeField] private TMP_Text _updateText;
    [SerializeField] private TMP_Text _costText;
    [SerializeField] private TMP_Text _expProgresText;
    [SerializeField] private TMP_Text _maxLevelText;

    private BallTreeNodeUI _node;
    private BallModel _cardModel;
    private StatTextHolder _statTextHolder;
    private List<StatTextHolder> _statTextHolders = new();
    private SoundSpawner _soundSpawner;
    private Tween _notEnoughXpTween;
    private IBallUnlockService _ballUnlockService;
    private IPoolService _poolService;
    private ItemStat _noneStat = new ItemStat(None, 0);

    public event Action Upgraded;
    public event Action PurchaseRejected;

    private void Start ()
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy ()
    {
        _upgradeButton.Clicked -= Upgrade;

        if (_cardModel != null)
            _cardModel.Upgraded -= View;
    }

    [Inject]
    public void Construct (SoundSpawner soundSpawner, IBallUnlockService ballUnlockService, IPoolService poolService)
    {
        _soundSpawner = soundSpawner;
        _ballUnlockService = ballUnlockService;
        _poolService = poolService;
    }

    public void Init (StatTextHolder statTextHolder)
    {
        _statTextHolder = statTextHolder;
    }

    public void SetNode (BallTreeNodeUI ballTreeNodeUI)
    {
        _node = ballTreeNodeUI;
        _cardModel = ballTreeNodeUI.BallModel;

        if (_cardModel != null)
            _cardModel.Upgraded -= View;

        _cardModel.Upgraded += View;

        this.Enable();
        _upgradeButton.Clicked -= Upgrade;
        _upgradeButton.Clicked += Upgrade;
        _upgradeButton.Init(_cardModel);

        bool isMaxLevel = _cardModel.NoneGameLevel == _cardModel.MaxNoneGameLevel;
        _upgradeButton.SetActive(isMaxLevel == false);
        _maxLevelText.SetActive(isMaxLevel);
        View(_cardModel);
    }

    public void View (ICardModel useles)
    {
        if (useles is not BallModel ballModel)
            return;

        ClearStats();

        bool isOpen = _cardModel.IsOpen;
        bool canUnlock = _ballUnlockService.CanUnlock(ballModel);
        Debug.Log(canUnlock);

        _expSlider.maxValue = _cardModel.EXPForNextLevel;
        _expSlider.minValue = 0;
        _expSlider.SetActive(isOpen);
        _costPanel.SetActive(isOpen == false);
        _image.sprite = _cardModel.Config.Icon;
        _image.color = Color.white;
        _descriptionText.text = _cardModel.Description;
        _level.text = _cardModel.Level.ToString();
        _nameText.text = _cardModel.Name;
        _expSlider.value = _cardModel.ItemEXP;
        _expProgresText.text = $"{TextLocalizator.Damage}: {(int) _cardModel.ItemEXP} / {(int) _cardModel.EXPForNextLevel:0}";

        if (isOpen)
        {
            _updateText.text = TextLocalizator.Upgrade;
            RenderStats(_cardModel, isOpen);

        }
        else
        {
            _updateText.text = TextLocalizator.Unlock;
            _costText.text = _cardModel.BallConfig.UnlockPrice.ToString();

            if (canUnlock == false)
            {
                _image.color = Color.black;
                _nameText.text = None;
                _descriptionText.text = None;
                _level.text = None;
                _expSlider.value = 0;
                _expProgresText.text = $"{None}/{None}";
                _updateText.text = TextLocalizator.Locked;

                InitStatTextHolder(_noneStat, false);
            }
            else
            {
                RenderStats(_cardModel, isOpen);
            }
        }

        Upgraded?.Invoke();
    }

    private void OnUpgradeFailed ()
    {
        RectTransform sliderRect = _expSlider.transform as RectTransform;
        Vector2 originPos = sliderRect.anchoredPosition;

        _notEnoughXpTween?.Kill();

        _notEnoughXpTween = sliderRect
            .DOPunchAnchorPos(new Vector2(10f, 0f), 0.3f, 10, 0.5f)
            .SetUpdate(true)
            .OnComplete(() => sliderRect.anchoredPosition = originPos);
    }

    private void RenderStats (BallModel ball, bool showNextLevel)
    {
        _statsParent.Enable();

        foreach (var item in ball.BallConfig.GetStats(ball.Level))
        {
            InitStatTextHolder(item, showNextLevel);
        }
    }

    private void InitStatTextHolder (ItemStat stat, bool showNextLevel)
    {
        StatTextHolder statTextHolder = _poolService.Get(_statTextHolder, _statsParent);
        statTextHolder.RenderText(stat, showNextLevel);
        _statTextHolders.Add(statTextHolder);
    }

    private void ClearStats () => ClearCollection(_statTextHolders);

    private void ClearCollection<T> (List<T> collection) where T : MonoBehaviour
    {
        if (collection.Count == 0)
            return;

        foreach (var item in collection)
        {
            Destroy(item.gameObject);
        }

        collection.Clear();
    }

    private void Upgrade ()
    {
        if (_node.BallModel.IsOpen)
        {
            _node.UpdateConnections();
            bool isUpgradeComplite = _node.Upgrade();

            if (_node.BallModel.NoneGameLevel == _node.BallModel.MaxNoneGameLevel)
            {
                _upgradeButton.Disable();
                _maxLevelText.Enable();
                return;
            }

            if (isUpgradeComplite == false)
            {
                OnUpgradeFailed();
                _soundSpawner.Spawn(SoundType.Error);
                return;
            }

            _soundSpawner.Spawn(SoundType.Upgrade, _node.BallModel.NoneGameLevel - 1, _node.BallModel.MaxNoneGameLevel);
        }
        else
        {
            if (_node.Unlock())
            {
                _soundSpawner.Spawn(SoundType.Upgrade, _node.BallModel.NoneGameLevel - 1, _node.BallModel.MaxNoneGameLevel);

            }
            else
            {
                _soundSpawner.Spawn(SoundType.Error);
                return;
            }
        }

        _node.UpdateState();
        View(_cardModel);
    }
}