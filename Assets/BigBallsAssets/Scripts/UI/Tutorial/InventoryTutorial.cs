using System.Collections.Generic;
using TMPro;
using UnityEngine;
using VContainer;

public class InventoryTutorial : MonoBehaviour
{
    [SerializeField] private List<Vector3> _points = new();
    [SerializeField] private List<HighliterUI> _highliterUIs = new();

    [SerializeField] private TutorialPanel _tutorialPanel;
    [SerializeField] private TextViewer _textViewer;
    [SerializeField] private HighliterUI _exitButton;
    [SerializeField] private HighliterUI _ugradebutton;
    [SerializeField] private Slot _weaponSlot;
    [SerializeField] private RectTransform _inventoryPanel;
    [SerializeField] private RectTransform _equipPanel;
    [SerializeField] private RectTransform _upgradePanel;

    [SerializeField] private CardInventory _inventory;

    [SerializeField] private ClickHand _clickHand;

    private Slot _highlitedSlot;
    private ISaveService _saveService;

    [Inject]
    public void Construct (ISaveService saveService)
    {
        _saveService = saveService;
    }

    private void OnEnable ()
    {
        if (_saveService == null)
            return;

        _inventory.Inited += WriteStartText;
    }

    private void OnDisable ()
    {
        _inventory.Inited -= WriteStartText;

        _tutorialPanel.Clicked -= ClickCardPanel;
    }

    public void WriteStartText()
    {
        _inventory.Inited -= WriteStartText;

        if (_saveService.GameProgress.InventoryTutotialComplite)
        {
            CloseTutorial();
            return;
        }

        _tutorialPanel.Clicked += ClickCardPanel;
        _textViewer.WriteNext();
    }

    public void ClickCardPanel ()
    {
        _tutorialPanel.Clicked -= ClickCardPanel;


        _textViewer.WriteNext();
        _clickHand.PlayClick(_points[0]);

        if (_inventory.Slots.Count > 0)
        {
            _highlitedSlot = _inventory.Slots[1];
            _highlitedSlot.HighliterUI.Show(3);
            _highlitedSlot.CurrentItem?.HighliterUI.Show(4);
        }


        foreach (var slot in _inventory.Slots)
        {
            if (slot.CurrentItem != null)
                slot.CurrentItem.Clicked += ShowDrag;
        }
    }

    public void ClickUpgrade ()
    {
        _textViewer.WriteNext();

        _highlitedSlot.HighliterUI.Hide();
        _highlitedSlot.CurrentItem?.HighliterUI.Hide();

        _weaponSlot.HighliterUI.Hide();
        _weaponSlot.CurrentItem?.HighliterUI.Hide();

        _weaponSlot.Droped -= ClickUpgrade;
        _ugradebutton.Show(3);
        _clickHand.PlayClick(_points[2]);

        _weaponSlot.CurrentItem.Model.AddItemEXP(50);
        _weaponSlot.CurrentItem.Render(_weaponSlot.CurrentItem.Model);

        ItemUpgradeButton upgradeButton = _ugradebutton.GetComponent<ItemUpgradeButton>();
        upgradeButton.Clicked += Exit;

    }

    public void ShowDrag (UIItem iItem)
    {
        _textViewer.WriteNext();

        foreach (var slot in _inventory.Slots)
        {
            if (slot.CurrentItem != null)
                slot.CurrentItem.Clicked -= ShowDrag;
        }

        _weaponSlot.HighliterUI.Show(3);
        _weaponSlot.Droped += ClickUpgrade;
        _clickHand.PlayDragItem(iItem, _points[1], _weaponSlot);
    }

    public void Exit ()
    {
        _textViewer.WriteNext();

        ItemUpgradeButton upgradeButton = _ugradebutton.GetComponent<ItemUpgradeButton>();
        upgradeButton.Clicked -= Exit;
        _ugradebutton.Hide();
        _exitButton.Show(2);
        _clickHand.PlayClick(_points[3]);

        _saveService.GameProgress.InventoryTutotialComplite = true;
        _saveService.Save();
    }

    public void CloseTutorial ()
    {
        foreach (var item in _highliterUIs)
        {
            item.Show(1);

            if (item.gameObject.TryGetComponent(out Slot slot))
            {
                slot.CurrentItem?.HighliterUI.Show(2);
            }
        }

        foreach (var item in _inventory.Slots)
        {
            item.HighliterUI.Show(1);
            item.CurrentItem?.HighliterUI.Show(2);
        }

        _tutorialPanel.Disable();
    }
}
