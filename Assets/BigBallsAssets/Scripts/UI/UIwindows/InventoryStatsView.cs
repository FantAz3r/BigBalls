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
    [SerializeField] private RectTransform _slotsParent;

    private ICardModel _cardModel;
    private StatTextHolder _statTextHolder;
    private StatsSlot _slotPrefab;
    private List<StatTextHolder> _statTextHolders = new();
    private List<StatsSlot> _statsSlots = new();

    private void Start() => gameObject.SetActive(false);

    private void OnDestroy() => _cardModel.Upgraded -= View;

    [Inject]
    public void Init(IResourceLoader resourceLoader)
    {
        _statTextHolder = resourceLoader.Load<StatTextHolder>();
        _slotPrefab = resourceLoader.Load<StatsSlot>();
    }

    public void View(ICardModel card)
    {
        ClearStats();
        ClearSlots();

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
        _slotsParent.gameObject.SetActive(false);
        _statsParent.gameObject.SetActive(false);

        if (card is WeaponModel weaponModel)
        {
            _slotsParent.gameObject.SetActive(true);

            foreach (var item in weaponModel.UniqueBallModels())
            {
                StatsSlot slot = Instantiate(_slotPrefab, _slotsParent);
                slot.RenderSlots(item, card);
                _statsSlots.Add(slot);
            }
        }
        else if (card is HelmetModel helmetModel)
        {
            _slotsParent.gameObject.SetActive(true);

            foreach (var item in helmetModel.GetArtefacts())
            {
                StatsSlot slot = Instantiate(_slotPrefab, _slotsParent);
                slot.RenderSlots(item, card);
                _statsSlots.Add(slot);
            }
        }
        else
        {
            _statsParent.gameObject.SetActive(true);

            foreach (var item in card.GetStatsText(true))
            {
                StatTextHolder statTextHolder = Instantiate(_statTextHolder, _statsParent);
                statTextHolder.RenderText(item);
                _statTextHolders.Add(statTextHolder);
            }
        }
    }

    private void ClearStats() => ClearCollection(_statTextHolders);

    private void ClearSlots() => ClearCollection(_statsSlots);

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
}