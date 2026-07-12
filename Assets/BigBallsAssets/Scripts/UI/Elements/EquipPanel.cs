using BigBalls.Infrastructure.DI;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class EquipPanel : MonoBehaviour
{
    [SerializeField] private UIItem _itemPrefab;
    [SerializeField] private Slot _weaponSlot;
    [SerializeField] private Slot _armorSlot;
    [SerializeField] private Slot _helmetSlot;
<<<<<<< HEAD
    [SerializeField] private InventoryStatsView _statsView;
=======
    [SerializeField] private StatsView _statsView;
>>>>>>> f0e424ab67e4621452446e379326bfb4b7df25bf

    private Canvas _mainCanvas;
    private CardsData _cardData;
    private ArmorRepository _armorRepository;
    private IObjectResolverProvider _objectResolverProvider;
    private List<ICardModel> _models = new();

    [Inject]
    public void Construct(ArmorRepository armorRepository, IObjectResolverProvider objectResolverProvider)
    {
        _mainCanvas = GetComponentInParent<Canvas>();
        _armorRepository = armorRepository;
        _objectResolverProvider = objectResolverProvider;
        _models.AddRange(armorRepository.AllModels.Values);
        CreateSlotsWithItems();
    }

    private void CreateSlotsWithItems()
    {
        InitSlot(_armorSlot, CardType.Armor);
        InitSlot(_weaponSlot, CardType.Weapon);
        InitSlot(_helmetSlot, CardType.Helmet);

        foreach (var model in _models)
        {
            if (model.IsOpen && model.HasPlayer)
            {
                if(model is ArmorModel)
                {
                    CreateItem(model, _armorSlot);
                }
                else if (model is WeaponModel)
                {
                    CreateItem(model, _weaponSlot);

                }
                else if (model is HelmetModel)
                {
                    CreateItem(model, _helmetSlot);
                }
            }
        }
    }

    private void InitSlot(Slot slot, CardType cardType)
    {
        _objectResolverProvider.CurrentResolver.Inject(slot);
        slot.Init(cardType, true);
    }

    private void CreateItem(ICardModel model, Slot slot)
    {
        UIItem item = _objectResolverProvider.CurrentResolver.Instantiate(_itemPrefab, slot.transform);
        item.Init(transform, _mainCanvas);
        item.Render(model);
        item.StatsButton.Init(_statsView);
        slot.SetItem(item);
    }
}
