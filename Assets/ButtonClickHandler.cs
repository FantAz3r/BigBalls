using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]

public class ButtonClickHandler : MonoBehaviour
{
    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnClick);
    }
    private void OnDestroy()
    {
        _button.onClick.RemoveListener(OnClick);
    }

    protected virtual void OnClick()
    {

    }
}