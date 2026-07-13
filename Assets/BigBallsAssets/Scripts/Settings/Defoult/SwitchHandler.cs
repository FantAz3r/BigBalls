using UnityEngine;

public class SwitchHandler : MonoBehaviour
{
    [field: SerializeField] public Switch Switch { get; private set; }

    private void Awake()
    {
        Switch.OnValueChanged += OnClick;
    }

    private void OnDestroy()
    {
        Switch.OnValueChanged -= OnClick;
    }

    protected virtual void OnClick(bool isOn)
    {
    }
}
