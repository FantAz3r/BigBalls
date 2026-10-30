using System;
using UnityEngine;

public class JoystickButton : MonoBehaviour
{
    [SerializeField] private Joystick _joystick;
    public event Action OnClicked;

    private void OnEnable ()
    {
        _joystick.Clicked += OnClick;
    }

    private void OnDisable ()
    {
        _joystick.Clicked -= OnClick;
    }

    private void OnClick ()
    {
        OnClicked?.Invoke();
    }
}
