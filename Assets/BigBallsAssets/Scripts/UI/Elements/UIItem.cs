using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIItem : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerClickHandler
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private RectTransform _selfRectTransform;
    [SerializeField] private Slider _slider;

    private Transform _inventory;
    private Canvas _mainCanvas;
    private Transform _previousParent;
    private Slot _previousSlot;

    [field: SerializeField] public Image SlotImag { get; private set; }
    [field: SerializeField] public TMP_Text ItemLevelText { get; private set; }
    [field: SerializeField] public StatsButton StatsButton { get; private set; }

    public ICardModel Model { get; private set; }


    private void OnDestroy () => Model.Upgraded -= Render;

    public void Init (Transform inventory, Canvas mainCanvas)
    {
        _inventory = inventory;
        _mainCanvas = mainCanvas;
    }

    public void Render (ICardModel card)
    {
        if (Model != null)
        {
            Model.Upgraded -= Render;
        }

        Model = card;
        Model.Upgraded += Render;

        _slider.maxValue = card.EXPForNextLevel;
        _slider.value = card.ItemEXP;
        SlotImag.sprite = Model.Config.Icon;
        ItemLevelText.text = Model.Level.ToString();
    }

    public void OnPointerClick (PointerEventData eventData) => StatsButton.View();

    public void OnDrag (PointerEventData eventData)
    {
        _selfRectTransform.anchoredPosition += eventData.delta / _mainCanvas.scaleFactor;
    }

    public void OnBeginDrag (PointerEventData eventData)
    {
        StatsButton.View();

        _previousParent = transform.parent;
        _previousSlot = _previousParent.GetComponent<Slot>();

        transform.SetParent(_inventory);
        transform.SetAsLastSibling();
        _canvasGroup.blocksRaycasts = false;
    }

    public void OnEndDrag (PointerEventData eventData)
    {
        Slot newSlot = null;

        if (transform.parent != null)
        {
            newSlot = transform.parent.GetComponent<Slot>();
        }

        if (newSlot == null || newSlot.CurrentItem != null)
        {
            transform.SetParent(_previousParent);
            transform.localPosition = Vector3.zero;
        }
        else
        {
            if (_previousSlot != null && _previousSlot != newSlot)
            {
                _previousSlot.RemoveItem(this);
                newSlot.AddItem(this);

                if (_previousSlot.CurrentImage != null)
                {
                    _previousSlot.CurrentImage.enabled = true;
                }
            }
            else if (_previousSlot == newSlot)
            {
                transform.localPosition = Vector3.zero;
            }
        }

        _canvasGroup.blocksRaycasts = true;
    }

}
