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
    private bool _dropHandled;
    public Slot SourceSlot { get; private set; }

    public void SetSourceSlot (Slot slot) => SourceSlot = slot;

    [field: SerializeField] public Image SlotImag { get; private set; }
    [field: SerializeField] public TMP_Text ItemLevelText { get; private set; }
    [field: SerializeField] public StatsButton StatsButton { get; private set; }

    public ICardModel Model { get; private set; }


    private void OnDisable () => Model.Upgraded -= Render;

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
        SourceSlot = _previousSlot;
        _dropHandled = false;

        transform.SetParent(_inventory);
        transform.SetAsLastSibling();
        _canvasGroup.blocksRaycasts = false;
    }

    public void OnEndDrag (PointerEventData eventData)
    {
        _canvasGroup.blocksRaycasts = true;

        if (_dropHandled)
            return;

        if (_previousParent != null)
        {
            transform.SetParent(_previousParent);
            transform.localPosition = Vector3.zero;
        }
    }

    public void MarkDropHandled () => _dropHandled = true;
}
