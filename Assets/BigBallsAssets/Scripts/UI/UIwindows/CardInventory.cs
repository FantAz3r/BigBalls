using BigBalls.Infrastructure.DI;
using BigBalls.Services;
using BigBalls.UI;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;

public class CardInventory : WindowBase
{
    [SerializeField] private Slot _slotPrefab;
    [SerializeField] private UIItem _itemPrefab;
    [SerializeField] private GridLayoutGroup _parent;
    [SerializeField] private TMP_Text _tipText;

    private Canvas _canvas;
    private CardsData _cardData;
    private List<Slot> _slots = new();
    private IResourceLoader _resourceLoader;
    private WeaponRepository _weaponRepository;
    private ArmorRepository _armorRepository;
    private List<ICardModel> _models = new();
    private IObjectResolverProvider _objectResolverProvider;

    [field: SerializeField] public InventoryStatsView StatsView { get; private set; }

    private void OnEnable()
    {
        ViewAll();
    }

    [Inject]
    public void Construct(
        IResourceLoader resourceLoader,
        ArmorRepository armorRepository,
        WeaponRepository weaponRepository,
        IObjectResolverProvider objectResolverProvider)
    {
        _resourceLoader = resourceLoader;
        _weaponRepository = weaponRepository;
        _armorRepository = armorRepository;
        _cardData = _resourceLoader.Load<CardsData>();
        _objectResolverProvider = objectResolverProvider;

        _canvas = GetComponent<Canvas>();
        _models.AddRange(_weaponRepository.AllModels.Values);
        _models.AddRange(_armorRepository.AllModels.Values);
    }

    public void ViewAll() => ViewSlots(card => card is WeaponModel || card is ArmorModel || card is HelmetModel);

    private void ViewSlots(Func<ICardModel, bool> filter)
    {
        RemoveAllSlots();

        foreach (var card in _models)
        {
            if (filter(card))
            {
                if (card.IsOpen || card.Level > 0)
                {
                    Slot slot = Instantiate(_slotPrefab, _parent.transform);
                    _slots.Add(slot);
                    slot.Init(CardType.Any, false);

                    if (card.HasPlayer == false)
                    {
                        UIItem item = _objectResolverProvider.CurrentResolver.Instantiate(_itemPrefab, slot.transform);
                        item.Init(transform, _canvas);
                        item.StatsButton.Init(StatsView);
                        item.Render(card);
                        slot.AddItem(item);
                    }

                    slot.transform.SetAsFirstSibling();
                }
            }
        }

        if (_slots.Count == 0)
        {
            _tipText.gameObject.SetActive(true);
        }
        else
        {
            _tipText.gameObject.SetActive(false);
        }
    }

    private void RemoveAllSlots()
    {
        var children = _parent.GetComponentsInChildren<Slot>();

        foreach (var child in children)
        {
            Destroy(child.gameObject);
        }

        _slots.Clear();
    }
}
