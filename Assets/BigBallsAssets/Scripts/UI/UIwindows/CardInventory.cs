using BigBalls.Configs;
using BigBalls.Infrastructure.DI;
using BigBalls.Services;
using BigBalls.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor.Graphs;
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
    private ArmorRepository _armorRepository;
    private List<ICardModel> _models = new();
    private IObjectResolverProvider _objectResolverProvider;

    private void OnEnable()
    {
        ViewAll();
    }

    [Inject]
    public void Construct(
        IResourceLoader resourceLoader,
        ArmorRepository armorRepository,
        IObjectResolverProvider objectResolverProvider
        )
    {
        _resourceLoader = resourceLoader;
        _armorRepository = armorRepository;
        _cardData = _resourceLoader.Load<CardsData>();
        _objectResolverProvider = objectResolverProvider;
        _models.AddRange(_armorRepository.AllModels.Values);
        _canvas = GetComponent<Canvas>();
    }

    public void ViewAll() => ViewSlots(card => card is WeaponModel || card is ArmorModel);
   
    private void ViewSlots(Func<ICardModel, bool> filter)
    {
        RemoveAllSlots();

        Debug.Log(_models.Count);

        foreach (var card in _models)
        {
            if (filter(card))
            {
                if (true) //card.IsOpen && card.Level > 0
                {
                    Slot slot = Instantiate(_slotPrefab, _parent.transform);
                    _slots.Add(slot);
                    slot.Init(CardType.Any, false);

                    if (card.HasPlayer == false)
                    {
                        UIItem item = _objectResolverProvider.CurrentResolver.Instantiate(_itemPrefab, slot.transform);
                        item.Init(transform, _canvas);
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
