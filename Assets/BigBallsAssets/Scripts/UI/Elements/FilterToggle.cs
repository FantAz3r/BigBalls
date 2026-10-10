using UnityEngine;
using UnityEngine.UI;

public class FilterToggle : MonoBehaviour
{
    [SerializeField] private int _filterIndex;
    [SerializeField] private CardInventory _inventory;

    private Toggle _toggle;

    private void Awake ()
    {
        _toggle = GetComponent<Toggle>();
    }

    private void OnEnable ()
    {
        _toggle.onValueChanged.AddListener(HandleValueChanged);
    }

    private void OnDisable ()
    {
        _toggle.onValueChanged.RemoveListener(HandleValueChanged);
    }

    private void HandleValueChanged (bool isOn)
    {
        if (isOn)
        {
            _inventory.OnFilterChanged(_filterIndex);
        }
    }
}