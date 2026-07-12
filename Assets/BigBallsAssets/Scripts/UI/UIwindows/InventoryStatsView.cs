using BigBalls.Services;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class InventoryStatsView : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _descriptionText;
    [SerializeField] private TMP_Text _level;
    [SerializeField] private ItemUpgradeButton _upgradeButton;
    [SerializeField] private RectTransform _statsParent;

    private ICardModel _cardModel;
    private StatTextHolder _statTextHolder;
    private List<StatTextHolder> _statTextHolders = new ();

    private void Start() => gameObject.SetActive(false);

    private void OnDestroy() => _cardModel.Upgraded -= View;

    [Inject]
    public void Init(IResourceLoader resourceLoader)
    {
        _statTextHolder = resourceLoader.Load<StatTextHolder>();
    }

    public void View(ICardModel card)
    {
        ClearStats();

        if (_cardModel != null)
        {
            _cardModel.Upgraded -= View;
        }
        _cardModel = card;
        _cardModel.Upgraded += View;

        gameObject.SetActive(true);
        _upgradeButton.Init(_cardModel);
        _image.sprite = _cardModel.Config.Icon;
        _nameText.text = _cardModel.Name;
        _descriptionText.text = _cardModel.Description;
        _level.text = _cardModel.Level.ToString();
        RenderStats(card);
    }

    private void RenderStats(ICardModel card)
    {
        foreach (var item in card.GetStatsText(true))
        {
            StatTextHolder statTextHolder = Instantiate(_statTextHolder, _statsParent);
            statTextHolder.RenderText(item);
            _statTextHolders.Add(statTextHolder);
        }
    }

    private void ClearStats()
    {
        if (_statTextHolders.Count == 0)
            return;

        foreach (var item in _statTextHolders)
        {
            Destroy(item.gameObject);
            _statTextHolders.Remove(item);
        }
    }
}