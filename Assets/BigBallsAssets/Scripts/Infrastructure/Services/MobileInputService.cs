using System;
using BigBalls.Services;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using VContainer.Unity;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class MobileInputService : IInputService, IStartable, ITickable
{
    private bool _isActive;
    private Vector2 _lastOrbitDirection = Vector2.zero;
    private float _lastPinchMagnitude = 0;

    private Joystick _moveJoystick;
    private Joystick _rotateJoystick;
    private JoystickButton _attackButton;

    public event Action<Vector2> MoveDirectionSeted;
    public event Action<Vector2> RotateDirectionSeted;
    public event Action Attack;

    public event Action<Vector2> CameraOrbitSeted;
    public event Action<float> CameraZoomSeted;

    public Vector2 CursorOrigin { get; set; }

    public void Init (Joystick moveJoystick, Joystick rotateJoystick, JoystickButton attackButton)
    {
        EnhancedTouchSupport.Enable();
        _moveJoystick = moveJoystick;
        _rotateJoystick = rotateJoystick;
        _attackButton = attackButton;
    }

    private void OnMovePerformed (Vector2 direction) => MoveDirectionSeted?.Invoke(direction);
    private void OnMoveCanceled () => MoveDirectionSeted?.Invoke(Vector2.zero);

    private void OnRotatePerformed (Vector2 direction) => RotateDirectionSeted?.Invoke(direction);
    private void OnRotateCanceled () => RotateDirectionSeted?.Invoke(Vector2.zero);

    private void OnAttack () => Attack?.Invoke();

    public void EnablePlayer ()
    {
        if (_attackButton != null)
        {
            _attackButton.OnClicked += OnAttack;
        }

        if (_moveJoystick != null)
        {
            _moveJoystick.gameObject.SetActive(true);
            _moveJoystick.MovePerformed += OnMovePerformed;
            _moveJoystick.MoveCanceled += OnMoveCanceled;
        }

        if (_rotateJoystick != null)
        {
            _rotateJoystick.gameObject.SetActive(true);
            _rotateJoystick.MovePerformed += OnRotatePerformed;
            _rotateJoystick.MoveCanceled += OnRotateCanceled;
        }
    }

    public void DisablePlayer ()
    {

        if (_moveJoystick != null)
        {
            _moveJoystick.gameObject.SetActive(false);
            _moveJoystick.MovePerformed -= OnMovePerformed;
            _moveJoystick.MoveCanceled -= OnMoveCanceled;
        }

        if (_attackButton != null)
            _attackButton.enabled = false;

        if (_rotateJoystick != null)
        {
            _rotateJoystick.gameObject.SetActive(false);
            _rotateJoystick.MovePerformed -= OnRotatePerformed;
            _rotateJoystick.MoveCanceled -= OnRotateCanceled;
        }
    }

    public void EnableUI ()
    {
    }

    public void DisableUI ()
    {
    }

    public void Dispose ()
    {
        if (_moveJoystick != null)
        {
            _moveJoystick.MovePerformed -= OnMovePerformed;
            _moveJoystick.MoveCanceled -= OnMoveCanceled;
        }

        if (_rotateJoystick != null)
        {
            _rotateJoystick.MovePerformed -= OnRotatePerformed;
            _rotateJoystick.MoveCanceled -= OnRotateCanceled;
        }

        if (_attackButton != null)
        {
            _attackButton.OnClicked -= OnAttack;
        }
    }

    public IInputService GetSelf ()
    {
        return this;
    }

    public void Start ()
    {
    }

    public void Tick ()
    {
        if (Touch.activeTouches.Count == 1)
        {
            var touch = Touch.activeTouches[0];

            if (touch.phase == UnityEngine.InputSystem.TouchPhase.Moved)
            {
                CameraOrbitSeted?.Invoke(touch.delta);
            }
            else if (touch.phase == UnityEngine.InputSystem.TouchPhase.Ended)
            {
                CameraOrbitSeted?.Invoke(Vector2.zero); 
            }
        }
        else if (Touch.activeTouches.Count == 2)
        {
            var touch1 = Touch.activeTouches[0];
            var touch2 = Touch.activeTouches[1];

            float currentMagnitude = Vector2.Distance(touch1.screenPosition, touch2.screenPosition);

            if (_lastPinchMagnitude > 0)
            {
                float zoomDelta = currentMagnitude - _lastPinchMagnitude;
                CameraZoomSeted?.Invoke(zoomDelta * 0.01f); 
            }

            _lastPinchMagnitude = currentMagnitude;
        }
        else
        {
            _lastPinchMagnitude = 0;
        }
    }

    public void EnableCamera ()
    {
    }

    public void DisableCamera ()
    {
    }
}