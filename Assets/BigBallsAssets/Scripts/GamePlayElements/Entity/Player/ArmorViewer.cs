using System.Collections.Generic;
using BigBalls.Providers;
using UnityEngine;
using VContainer;

public class ArmorViewer : MonoBehaviour
{
    [SerializeField] private List<ArmorSet> _armors = new();
    [SerializeField] private ArmorSet _baseSet;

    private IItemConainerProvider _itemConainerProvider;
    private StatHolder _statHolder;
    private ArmorSet _currentArmorSet;
    private ArmorModel _currentArmorModel;

    [Inject]
    public void Construct (IItemConainerProvider itemConainerProvider)
    {
        _itemConainerProvider = itemConainerProvider;
    }

    public void Init (StatHolder statHolder)
    {
        _statHolder = statHolder;
        UpdateArmorVisual(_itemConainerProvider.Armor);

        _itemConainerProvider.ArmorSeted += OnArmorSeted;
        _itemConainerProvider.ArmorRemoved += OnArmorRemoved;
    }

    private void OnDisable ()
    {
        if (_itemConainerProvider != null)
        {
            _itemConainerProvider.ArmorSeted -= OnArmorSeted;
            _itemConainerProvider.ArmorRemoved -= OnArmorRemoved;
        }
    }

    private void OnArmorSeted (ArmorModel armorModel)
    {
        if (_currentArmorModel == armorModel)
            return;

        UpdateArmorVisual(armorModel);
        _statHolder.Set(armorModel);
    }

    private void OnArmorRemoved (ArmorModel armorModel = null)
    {
        UpdateArmorVisual(null);
        _statHolder.Set(null);
    }

    private void UpdateArmorVisual (ArmorModel armorModel)
    {
        _currentArmorModel = armorModel;

        if (_currentArmorSet != null)
        {
            SetArmorActive(_currentArmorSet, false);
            _currentArmorSet = null;
        }

        if (armorModel == null)
        {
            if (_baseSet != null)
            {
                SetArmorActive(_baseSet, true);
                _currentArmorSet = _baseSet;
            }
            return;
        }

        foreach (var armorSet in _armors)
        {
            if (armorSet.ItemType == armorModel.ArmorConfig.ItemType)
            {
                SetArmorActive(armorSet, true);
                _currentArmorSet = armorSet;
                return;
            }
        }

        if (_baseSet != null)
        {
            SetArmorActive(_baseSet, true);
            _currentArmorSet = _baseSet;
        }
    }

    private void SetArmorActive (ArmorSet armorSet, bool isActive)
    {
        if (armorSet == null || armorSet.ArmorElements == null) return;

        foreach (GameObject item in armorSet.ArmorElements)
        {
            if (item != null)
                item.SetActive(isActive);
        }
    }
}