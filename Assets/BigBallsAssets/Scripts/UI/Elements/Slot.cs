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
    public CardType SlotType => _slotType;

    [Inject]
    public void Construct (IItemConainerProvider itemConainerProvider)
    {
        _itemContainer = itemConainerProvider;
    }

    public void Init (CardType slotType, bool isActiveSlot)
    {
        _slotType = slotType;
        IsActiveSlot = isActiveSlot;

        ShowTip(true);
    }

    public void View (ICardModel card)
    {
        Card = card;
        _image.sprite = card.Config.Icon;
        _text.text = card.Level.ToString();
    }

    public void Clear ()
    {
        Card = null;
        gameObject.SetActive(false);
    }

    public void OnDrop (PointerEventData eventData)
    {
        var draggedItem = eventData.pointerDrag.GetComponent<UIItem>();

        if (draggedItem == null)
            return;

        if (_slotType == CardType.None)
            return;

        if (_slotType != CardType.Any && draggedItem.Model.Config.Type != _slotType)
            return;

        if (CurrentItem == draggedItem)
            return;

        if (CurrentItem != null)
        {
            var otherItem = CurrentItem;
            var sourceSlot = draggedItem.SourceSlot;

            if (sourceSlot == null || sourceSlot == this)
                return;

            if (sourceSlot.SlotType != CardType.Any
                && sourceSlot.SlotType != CardType.None
                && otherItem.Model.Config.Type != sourceSlot.SlotType)
                return;

            sourceSlot.RemoveItem(draggedItem);
            RemoveItem(otherItem);
            PlaceItem(draggedItem);
            sourceSlot.PlaceItem(otherItem);

            draggedItem.MarkDropHandled();
            otherItem.MarkDropHandled();

            return;
        }

        PlaceItem(draggedItem);
        draggedItem.MarkDropHandled();
        draggedItem.StatsButton.View();
    }

    public void PlaceItem (UIItem item)
    {
        if (CurrentItem == item)
            return;

        var oldSlot = item.SourceSlot;
        if (oldSlot != null && oldSlot != this)
        {
            oldSlot.RemoveItem(item);
        }

        SetItem(item);

        item.transform.SetParent(transform);
        item.transform.localPosition = Vector3.zero;
        item.SetSourceSlot(this);

        if (IsActiveSlot)
            _itemContainer.Add(item.Model);
    }

    public void RemoveItem (UIItem item)
    {
        if (IsActiveSlot)
        {
            _itemContainer.Remove(item.Model);

        }

        item.SetSourceSlot(null);
        SetItem(null);
        ShowTip(true);
    }

    public void SetItem (UIItem item)
    {
        CurrentItem = item;
        ShowTip(false);
    }

    private void ShowTip (bool isShow)
    {
        if (_slotType == CardType.Weapon)
        {
            _weaponTip.enabled = isShow;
        }
        else if (_slotType == CardType.Armor)
        {
            _armorTip.enabled = isShow;
        }
        else if (_slotType == CardType.Helmet)
        {
            _helmetTip.enabled = isShow;
        }
    }
}
