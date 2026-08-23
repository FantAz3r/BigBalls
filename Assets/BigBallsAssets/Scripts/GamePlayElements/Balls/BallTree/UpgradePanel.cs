using BigBalls.Localization;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    
    private BallTreeNodeUI _node;
    private BallModel _cardModel;
    private StatTextHolder _statTextHolder;
    private List<StatTextHolder> _statTextHolders = new();

    public event Action<BallTreeNodeUI> Upgraded;

    private void Start() => gameObject.SetActive(false);

    private void OnDestroy()
    {
        _upgradeButton.Clicked -= Upgrade;

        if (_cardModel != null)
            _cardModel.Upgraded -= View;
    }

    public void Init(StatTextHolder statTextHolder)
    {
        _statTextHolder = statTextHolder;
    }

    public void SetNode(BallTreeNodeUI ballTreeNodeUI)
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

        View(_cardModel);
    }

    public void View(ICardModel useles)
    {
        ClearStats();

        _image.sprite = _cardModel.Config.Icon;
        _nameText.text = _cardModel.Name;
        _descriptionText.text = _cardModel.Description;
        _level.text = _cardModel.Level.ToString();
        _expSlider.maxValue = _cardModel.EXPForNextLevel;
        _expSlider.minValue = 0;
        _expSlider.value = _cardModel.ItemEXP;
        _expProgresText.text = $"{_cardModel.ItemEXP} / {_cardModel.EXPForNextLevel}";

        RenderStats(_cardModel);

        if (_cardModel.IsOpen)
        {
            _updateText.text = TextLocalizator.Upgrade;
            _expSlider.gameObject.SetActive(true);
        }
        else
        {
            _updateText.text = TextLocalizator.Unlock;
            _expSlider.gameObject.SetActive(false);
            _costText.text = _cardModel.BallConfig.UnlockPrice.ToString();
        }
    }

    private void RenderStats(ICardModel card)
    {
        if (card is not BallModel ball)
            return;

        _statsParent.gameObject.SetActive(true);

        foreach (var item in ball.BallConfig.GetStats(ball.Level))
        {
            StatTextHolder statTextHolder = Instantiate(_statTextHolder, _statsParent);
            statTextHolder.RenderText(item);
            _statTextHolders.Add(statTextHolder);
        }
    }

    private void ClearStats() => ClearCollection(_statTextHolders);

    private void ClearCollection<T>(List<T> collection) where T : MonoBehaviour
    {
        if (collection.Count == 0)
            return;

        foreach (var item in collection)
        {
            Destroy(item.gameObject);
        }

        collection.Clear();
    }

    private void Upgrade()
    {
        if (_node.BallModel.IsOpen)
        {
            _node.UpdateConnections();
        }
        else
        {
            _node.Unlock();
        }

        _node.UpdateState();
        View(_cardModel);
    }
}