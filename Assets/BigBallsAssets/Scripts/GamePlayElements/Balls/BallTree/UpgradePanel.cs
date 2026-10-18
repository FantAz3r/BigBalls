using System;
using System.Collections.Generic;
using BigBalls.Localization;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class UpgradePanel : MonoBehaviour
{
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
    private GlobalWallet _globalWallet;
    private List<StatTextHolder> _statTextHolders = new();
    private SoundSpawner _soundSpawner;
    private Tween _notEnoughXpTween;

    public event Action Upgraded;
    public event Action PurchaseRejected;

    private void Start () => gameObject.SetActive(false);

    private void OnDestroy ()
    {
        _upgradeButton.Clicked -= Upgrade;

        if (_cardModel != null)
            _cardModel.Upgraded -= View;
    }

    [Inject]
    public void Construct (GlobalWallet globalWallet, SoundSpawner soundSpawner)
    {
        _globalWallet = globalWallet;
        _soundSpawner = soundSpawner;
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

        gameObject.SetActive(true);
        _upgradeButton.Clicked -= Upgrade;
        _upgradeButton.Clicked += Upgrade;
        _upgradeButton.Init(_cardModel);

        if (_cardModel.NoneGameLevel == _cardModel.MaxNoneGameLevel)
        {
            _upgradeButton.gameObject.SetActive(false);
            _maxLevelText.gameObject.SetActive(true);
        }
        else
        {
            _upgradeButton.gameObject.SetActive(true);
            _maxLevelText.gameObject.SetActive(false);
        }

        View(_cardModel);
    }

    public void View (ICardModel useles)
    {
        ClearStats();

        _image.sprite = _cardModel.Config.Icon;
        _nameText.text = _cardModel.Name;
        _descriptionText.text = _cardModel.Description;
        _level.text = _cardModel.Level.ToString();
        _expSlider.maxValue = _cardModel.EXPForNextLevel;
        _expSlider.minValue = 0;
        _expSlider.value = _cardModel.ItemEXP;
        _expProgresText.text = $"{(int) _cardModel.ItemEXP} / {(int) _cardModel.EXPForNextLevel:0}";

        if (_cardModel.IsOpen)
        {
            _updateText.text = TextLocalizator.Upgrade;
            _expSlider.gameObject.SetActive(true);
            RenderStats(_cardModel, true);

        }
        else
        {
            _updateText.text = TextLocalizator.Unlock;
            _expSlider.gameObject.SetActive(false);
            _costText.text = _cardModel.BallConfig.UnlockPrice.ToString();
            RenderStats(_cardModel, false);

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

    private void RenderStats (ICardModel card, bool showNextLevel)
    {
        if (card is not BallModel ball)
            return;

        _statsParent.gameObject.SetActive(true);

        foreach (var item in ball.BallConfig.GetStats(ball.Level))
        {
            StatTextHolder statTextHolder = Instantiate(_statTextHolder, _statsParent);
            statTextHolder.RenderText(item, showNextLevel);
            _statTextHolders.Add(statTextHolder);
        }
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
                _upgradeButton.gameObject.SetActive(false);
                _maxLevelText.gameObject.SetActive(true);
                return;
            }

            if (isUpgradeComplite == false)
            {
                OnUpgradeFailed();
                _soundSpawner.Spawn(SoundType.Error);
                return;
            }
        }
        else
        {
            _node.Unlock();
        }

        _node.UpdateState();
        _soundSpawner.Spawn(SoundType.Upgrade, _node.BallModel.NoneGameLevel - 1, _node.BallModel.MaxNoneGameLevel);
        View(_cardModel);
    }
}