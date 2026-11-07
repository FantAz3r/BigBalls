using System.Collections.Generic;
using BigBalls.Infrastructure.DI;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class EquipPanel : MonoBehaviour
{
    [SerializeField] private UIItem _itemPrefab;
    [SerializeField] private Slot _weaponSlot;
    [SerializeField] private Slot _armorSlot;
    [SerializeField] private Slot _helmetSlot;
    [SerializeField] private InventoryStatsView _statsView;

    private Canvas _mainCanvas;
    private ArmorRepository _armorRepository;
    private WeaponRepository _weaponRepository;
    private HelmetRepository _helmetRepository;
    private IObjectResolverProvider _objectResolverProvider;
    private List<ICardModel> _models = new();

    [Inject]
    public void Construct (
        ArmorRepository armorRepository,
        WeaponRepository weaponRepository,
        HelmetRepository helmetRepository,
        IObjectResolverProvider objectResolverProvider)
    {
        _mainCanvas = GetComponentInParent<Canvas>();
        _armorRepository = armorRepository;
        _weaponRepository = weaponRepository;
        _helmetRepository = helmetRepository;
        _objectResolverProvider = objectResolverProvider;

        _models.Clear();
        _models.AddRange(_armorRepository.AllModels.Values);
        _models.AddRange(_weaponRepository.AllModels.Values);
        _models.AddRange(_helmetRepository.AllModels.Values);

        CreateSlotsWithItems();
    }

    private void CreateSlotsWithItems ()
    {
        InitSlot(_armorSlot, CardType.Armor);
        InitSlot(_weaponSlot, CardType.Weapon);
        InitSlot(_helmetSlot, CardType.Helmet);

        foreach (var model in _models)
        {
            if (model.HasPlayer == false)
                continue;

            if (model is ArmorModel)
                CreateItem(model, _armorSlot);
            else if (model is WeaponModel)
                CreateItem(model, _weaponSlot);
            else if (model is HelmetModel)
                CreateItem(model, _helmetSlot);
        }
    }

    private void InitSlot (Slot slot, CardType cardType)
    {
        _objectResolverProvider.CurrentResolver.Inject(slot);
        slot.Init(cardType, true);
    }

    private void CreateItem (ICardModel model, Slot slot)
    {
        UIItem item = _objectResolverProvider.CurrentResolver.Instantiate(_itemPrefab, slot.transform);
        item.Init(transform, _mainCanvas);
        item.Render(model);
        item.StatsButton.Init(_statsView);

        slot.PlaceItem(item);
        item.SetSourceSlot(slot);
    }
}