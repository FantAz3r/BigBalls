using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Switch : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private bool _isOn = false;

    public event Action<bool> OnValueChanged;

    public bool IsOn
    {
        get => _isOn;
        set
        {
            if (_isOn != value)
            {
                _isOn = value;
                OnValueChanged?.Invoke(_isOn);
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        IsOn = IsOn == false;
    }
}
