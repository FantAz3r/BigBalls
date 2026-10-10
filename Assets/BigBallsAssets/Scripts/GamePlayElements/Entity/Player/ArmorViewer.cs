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
    private ArmorSet _currentArmor;
    private ArmorModel _currentArmorModel;

    [Inject]
    public void Construct (IItemConainerProvider itemConainerProvider)
    {
        _itemConainerProvider = itemConainerProvider;
    }

    public void Init (StatHolder statHolder)
    {
        _statHolder = statHolder;
        View(_itemConainerProvider.Armor);
        _itemConainerProvider.ArmorSeted += View;
        _itemConainerProvider.ArmorRemoved += OnRemoveArmor;

    }

    private void OnDisable ()
    {
        if (_itemConainerProvider != null)
        {
            _itemConainerProvider.ArmorSeted -= View;
            _itemConainerProvider.ArmorRemoved -= OnRemoveArmor;
        }
    }

    private void OnRemoveArmor()
    {
        if (_currentArmor != null)
        {
            foreach (GameObject item in _currentArmor.ArmorElements)
            {
                item.SetActive(false);
            }

            _currentArmor = null;
        }
    }

    private void View (ArmorModel armor)
    {
        OnRemoveArmor();

        if (armor == null)
            return;

        _currentArmorModel = armor;

        foreach (var armorSet in _armors)
        {
            if (armorSet.ItemType == _currentArmorModel.ArmorConfig.ItemType)
            {
                _currentArmor = armorSet;
                foreach (GameObject item in armorSet.ArmorElements)
                {
                    item.SetActive(true);
                }

                _statHolder.Set(armor);
            }
        }

        Debug.Log(_currentArmor);
    }
}
