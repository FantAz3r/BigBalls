using System;
using BigBalls.Services;
using UnityEngine;
using VContainer.Unity;

public class MobileInputService : IInputService, IStartable
{
    private bool _isActive;

    private Joystick _moveJoystick;
    private Joystick _rotateJoystick;
    private JoystickButton _attackButton; 

    public event Action<Vector2> MoveDirectionSeted;
    public event Action<Vector2> RotateDirectionSeted;
    public event Action Attack;

    public Vector2 CursorOrigin { get; set; }

    public void Init (Joystick moveJoystick, Joystick rotateJoystick, JoystickButton attackButton)
    {
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
}