using UnityEngine;
using UnityEngine.UI;

public class SwitchEffector : MonoBehaviour
{
    [SerializeField] private Switch _switch;
    [SerializeField] private RectTransform _handle;
    [SerializeField] private Image _background;
    [SerializeField] private float _handleOffset = 30f;
    [SerializeField] private RectTransform _switchOn;
    [SerializeField] private RectTransform _switchOff;

    private Vector2 _handleStartPosition;
    private bool _isInitialized = false;

    private void Awake()
    {
        _switch.OnValueChanged += UpdateUI;
        _handleStartPosition = _handle.anchoredPosition;

        _isInitialized = true;
        UpdateUI(_switch != null ? _switch.IsOn : false);
    }

    private void OnDestroy()
    {
        if (_switch != null)
        {
            _switch.OnValueChanged -= UpdateUI;
        }
    }

    private void UpdateUI(bool isOn)
    {
        if (_isInitialized == false)
            return;

        if (_handle != null)
        {
            Vector2 targetPosition = _handleStartPosition;
            targetPosition.x = isOn ? _handleOffset : -_handleOffset;
            _handle.anchoredPosition = targetPosition;
        }

        if (isOn)
        {
            _switchOn.gameObject.SetActive(true);
            _switchOff.gameObject.SetActive(false);
        }
        else
        {
            _switchOff.gameObject.SetActive(true);
            _switchOn.gameObject.SetActive(false);
        }
    }

    public void SetSwitchState(bool isOn)
    {
        if (_switch != null)
        {
            _switch.IsOn = isOn;
        }
    }
}
