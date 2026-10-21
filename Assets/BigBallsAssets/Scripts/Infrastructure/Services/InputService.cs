using System;
using BigBalls.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer.Unity;

namespace BigBalls.Services
{
    public class InputService : IInputService, IStartable
    {
        private bool _isActive;
        private readonly IWindowService _windowService;
        private readonly ITimeService _timeService;
        private PlayerInputActions _inputActions;

        public event Action<Vector2> MoveDirectionSeted;
        public event Action<Vector2> RotateDirectionSeted;
        public event Action Attack;

        public InputService (IWindowService windowService, ITimeService timeService)
        {
            _inputActions = new PlayerInputActions();
            _windowService = windowService;
            _timeService = timeService;
        }

        public Vector2 CursorOrigin { get; set; }

        public void Start ()
        {
            _inputActions.UI.Pause.performed += PauseGame;
            _inputActions.UI.Timescale1.performed += SetTimescale1;
            _inputActions.UI.Timescale2.performed += SetTimescale2;
            _inputActions.UI.Timescale3.performed += SetTimescale3;

            _inputActions.Player.Move.performed += OnMovePerformed;
            _inputActions.Player.Move.canceled += OnMoveCanceled;
            _inputActions.Player.Rotate.performed += OnRotatePerformed;
            _inputActions.Player.Rotate.canceled += OnRotateCanceled;
            _inputActions.Player.Attack.performed += OnAttack;
        }

        private void OnMovePerformed (InputAction.CallbackContext context)
        {
            Vector2 direction = context.ReadValue<Vector2>();
            MoveDirectionSeted?.Invoke(direction);
        }

        private void OnMoveCanceled (InputAction.CallbackContext context)
        {
            MoveDirectionSeted?.Invoke(Vector2.zero);
        }

        private void PauseGame (InputAction.CallbackContext context)
        {
            _windowService.Open<PauseWindow>();
        }

        private void SetTimescale1 (InputAction.CallbackContext context)
        {
            _timeService.SetTimescale(1);
        }

        private void SetTimescale2 (InputAction.CallbackContext context)
        {
            _timeService.SetTimescale(2);
        }

        private void SetTimescale3 (InputAction.CallbackContext context)
        {
            _timeService.SetTimescale(3);
        }

        private void OnRotatePerformed (InputAction.CallbackContext context)
        {
            RotateDirectionSeted?.Invoke(context.ReadValue<Vector2>());
        }

        private void OnRotateCanceled (InputAction.CallbackContext context)
        {
            RotateDirectionSeted?.Invoke(Vector2.zero);
        }

        private void OnAttack (InputAction.CallbackContext context)
        {
            Attack?.Invoke();
        }

        public void EnableUI ()
        {
            _inputActions.UI.Enable();
        }

        public void DisableUI ()
        {
            _inputActions.UI.Disable();
        }

        public void EnablePlayer ()
        {
            _inputActions.Player.Enable();
        }
        public void DisablePlayer ()
        {
            _inputActions.Player.Disable();
        }

        public void Dispose ()
        {
            if (_inputActions == null)
                return;

            _inputActions.UI.Pause.performed -= PauseGame;
            _inputActions.UI.Timescale1.performed -= SetTimescale1;
            _inputActions.UI.Timescale2.performed -= SetTimescale2;
            _inputActions.UI.Timescale3.performed -= SetTimescale3;

            _inputActions.Player.Move.performed -= OnMovePerformed;
            _inputActions.Player.Move.canceled -= OnMoveCanceled;
            _inputActions.Player.Rotate.performed -= OnRotatePerformed;
            _inputActions.Player.Rotate.canceled -= OnRotateCanceled;

            _inputActions.Dispose();
            _inputActions = null;
        }

        public IInputService GetSelf ()
        {
            return this;
        }
    }
}
