using BigBalls.Infrastructure.DI;
using BigBalls.UI;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class ItemInventory : WindowBase
{
    [SerializeField] private Slot _prefab;
    [SerializeField] private List<ICardModel> _items = new();
    [SerializeField] private RectTransform _itemPanel;

    private ArmorRepository _armorRepository;
    private IObjectResolverProvider _objectResolverProvider;

    [Inject]
    public void Construct(
        IObjectResolverProvider objectResolverProvider,
        ArmorRepository armorRepository)
    {
        _objectResolverProvider = objectResolverProvider;
        _armorRepository = armorRepository;
        _items.AddRange(_armorRepository.AllModels.Values);
    }

    public override void Open()
    {
        base.Open();
        Show();
    }

    public void Show()
    {
        foreach (var item in _items)
        {
            Slot slot = _objectResolverProvider.CurrentResolver.Instantiate(_prefab, _itemPanel);
            slot.View(item);
        }
    }
}
