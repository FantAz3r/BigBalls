using BigBalls.Providers;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VContainer;

public class Slot : MonoBehaviour, IDropHandler
{
    [SerializeField] private Image _image;
    [SerializeField] private TMP_Text _text;

    [SerializeField] private Image _weaponTip;
    [SerializeField] private Image _armorTip;
    [SerializeField] private Image _helmetTip;

    [SerializeField] private CardType _slotType;
    private IItemConainerProvider _itemContainer;

    [field: SerializeField] public bool IsActiveSlot { get; private set; }

    public ICardModel Card { get; private set; }
    public Image CurrentImage { get; private set; }
    public UIItem CurrentItem { get; private set; }

    [Inject]
    public void Construct(IItemConainerProvider itemConainerProvider)
    {
        _itemContainer = itemConainerProvider;
    }

    public void Init(CardType slotType, bool isActiveSlot)
    {
        _slotType = slotType;
        IsActiveSlot = isActiveSlot;

        if (_slotType == CardType.Weapon)
        {
            _weaponTip.enabled = true;
        }
        else if (_slotType == CardType.Armor)
        {
            _armorTip.enabled = true;
        }
        else if (_slotType == CardType.Helmet)
        {
            _helmetTip.enabled = true;
        }
    }

    public void View(ICardModel card)
    {
        Card = card;
        _image.sprite = card.Config.Icon;
        _text.text = card.Level.ToString();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (CurrentItem != null)
            return;

        if (_slotType == CardType.None)
            return;

        var itemTransform = eventData.pointerDrag.transform;
        var draggedItem = itemTransform.GetComponent<UIItem>();

        if (_slotType != CardType.Any && draggedItem.Model.Config.Type != _slotType)
            return;

        itemTransform.SetParent(transform);
        itemTransform.localPosition = Vector3.zero;

        if (CurrentImage != null)
            CurrentImage.enabled = false;
    }

    public void SetItem(UIItem item)
    {
        CurrentItem = item;
    }

    public void AddItem(UIItem item)
    {
        SetItem(item);

        if (IsActiveSlot)
            _itemContainer.Add(item.Model);
    }

    public void RemoveItem(UIItem item)
    {
        if (IsActiveSlot)
            _itemContainer.Remove(item.Model);

        SetItem(null);
    }
}
