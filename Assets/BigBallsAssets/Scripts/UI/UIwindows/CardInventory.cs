using BigBalls.Infrastructure.DI;
using BigBalls.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;

public class CardInventory : PopupWindow
{
    [SerializeField] private Slot _slotPrefab;
    [SerializeField] private UIItem _itemPrefab;
    [SerializeField] private GridLayoutGroup _parent;
    [SerializeField] private TMP_Text _tipText;

    private Canvas _canvas;
    private List<Slot> _slots = new();
    private WeaponRepository _weaponRepository;
    private ArmorRepository _armorRepository;
    private HelmetRepository _helmetRepository;
    private List<ICardModel> _models = new();
    private IObjectResolverProvider _objectResolverProvider;

    [field: SerializeField] public InventoryStatsView StatsView { get; private set; }

    private void OnEnable()
    {
        ViewAll();
    }

    [Inject]
    public void Construct(
        ArmorRepository armorRepository,
        WeaponRepository weaponRepository,
        HelmetRepository helmetRepository,
        IObjectResolverProvider objectResolverProvider)
    {
        _helmetRepository = helmetRepository;
        _weaponRepository = weaponRepository;
        _armorRepository = armorRepository;
        _objectResolverProvider = objectResolverProvider;

        _canvas = GetComponent<Canvas>();
        _models.AddRange(_weaponRepository.AllModels.Values);
        _models.AddRange(_armorRepository.AllModels.Values);
        _models.AddRange(_helmetRepository.AllModels.Values);
    }

    public void ViewAll() => ViewSlots(card => card is WeaponModel || card is ArmorModel || card is HelmetModel);
    public void ViewWeapons() => ViewSlots(card => card is WeaponModel);
    public void ViewHelmets() => ViewSlots(card => card is HelmetModel);
    public void ViewArmors() => ViewSlots(card => card is ArmorModel);

    private void ViewSlots(Func<ICardModel, bool> filter)
    {
        RemoveAllSlots();

        foreach (var card in _models)
        {
            if (filter(card))
            {
                if (card.IsOpen || card.Level > 1 || true)
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
