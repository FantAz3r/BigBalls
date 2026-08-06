using BigBalls.UI;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer.Unity;

namespace BigBalls.Services
{
    public class InputService : IInputService, IStartable
    {
        private readonly IWindowService _windowService;
        private PlayerInputActions _inputActions;

        public event Action<Vector2> MoveDirectionSeted;
        public event Action<Vector2> RotateDirectionSeted;
        public event Action Attack;

        public InputService(IWindowService windowService)
        {
            _inputActions = new PlayerInputActions();
            _windowService = windowService;
        }

        public Vector2 CursorOrigin { get; set; }

        public void Start()
        {
            _inputActions.Enable();

            _inputActions.Player.Move.performed += OnMovePerformed;
            _inputActions.Player.Move.canceled += OnMoveCanceled;

            _inputActions.UI.Pause.performed += PauseGame;

            _inputActions.Player.Rotate.performed += OnRotatePerformed;
            _inputActions.Player.Rotate.canceled += OnRotateCanceled;
            _inputActions.Player.Attack.performed += OnAttack;
        }

        private void OnMovePerformed(InputAction.CallbackContext context)
        {
            Vector2 direction = context.ReadValue<Vector2>();
            MoveDirectionSeted?.Invoke(direction);
        }

        private void OnMoveCanceled(InputAction.CallbackContext context)
        {
            MoveDirectionSeted?.Invoke(Vector2.zero);
        }

        private void PauseGame(InputAction.CallbackContext context)
        {
            _windowService.Open<PauseWindow>();
        }

        private void OnRotatePerformed(InputAction.CallbackContext context)
        {
            RotateDirectionSeted?.Invoke(context.ReadValue<Vector2>());
        }

        private void OnRotateCanceled(InputAction.CallbackContext context)
        {
            RotateDirectionSeted?.Invoke(Vector2.zero);
        }

        private void OnAttack(InputAction.CallbackContext context)
        {
            Attack?.Invoke();
        }

        public void Enable()
        {
            _inputActions.Player.Enable();
        }

        public void Disable()
        {
            _inputActions.Player.Disable();
        }

        public void Dispose()
        {
            if (_inputActions == null)
                return;

            _inputActions.Player.Move.performed -= OnMovePerformed;
            _inputActions.Player.Move.canceled -= OnMoveCanceled;

            _inputActions.Player.Rotate.performed -= OnRotatePerformed;
            _inputActions.Player.Rotate.canceled -= OnRotateCanceled;

            _inputActions.Dispose();
            _inputActions = null;
        }

        public IInputService GetSelf()
        {
            return this;
        }
    }
}
