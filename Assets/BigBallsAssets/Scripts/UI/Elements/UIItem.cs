using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(HighliterUI))]
public class UIItem : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerClickHandler
{
    [SerializeField] private RectTransform _selfRectTransform;
    [SerializeField] private Slider _slider;
    [SerializeField] private int _sortingIndex;
    [field: SerializeField] public CanvasGroup CanvasGroup { get; private set; }
    [field: SerializeField] public Image SlotImag { get; private set; }
    [field: SerializeField] public TMP_Text ItemLevelText { get; private set; }
    [field: SerializeField] public StatsButton StatsButton { get; private set; }

    private Transform _inventory;
    private Canvas _mainCanvas;
    private Transform _previousParent;
    private Slot _previousSlot;
    private bool _dropHandled;
    public Slot SourceSlot { get; private set; }

    public void SetSourceSlot (Slot slot) => SourceSlot = slot;


    public event Action DragStarted;
    public event Action<UIItem> Clicked;

    public event Action DragEnded;
    public HighliterUI HighliterUI { get; private set; }
    public ICardModel Model { get; private set; }


    private void OnDisable ()
    {
        if (Model != null)
            Model.Upgraded -= Render;
    }

    public void Init (Transform inventory, Canvas mainCanvas)
    {
        HighliterUI = GetComponent<HighliterUI>();
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

    public void OnPointerClick (PointerEventData eventData)
    {
        Clicked?.Invoke(this);
        StatsButton.View();
    }

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
        HighliterUI.Show(4);

        transform.SetParent(_inventory);
        transform.SetAsLastSibling();
        CanvasGroup.blocksRaycasts = false;

        Clicked?.Invoke(this);
        DragStarted?.Invoke();
    }

    public void OnEndDrag (PointerEventData eventData)
    {
        CanvasGroup.blocksRaycasts = true;

        if (_dropHandled)
            return;

        if (_previousParent != null)
        {
            transform.SetParent(_previousParent);
            //HighliterUI.Hide();
            transform.localPosition = Vector3.zero;
            DragEnded?.Invoke();
        }
    }

    public void MarkDropHandled () => _dropHandled = true;

}
