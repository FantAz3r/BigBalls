using System;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer.Unity;
using YG;

namespace BigBalls.Services
{
    public class InputService : IInputService, IStartable
    {
        private PlayerInputActions _inputActions;

        public event Action<Vector2> MoveDirectionSeted;
        public event Action<Vector2> RotateDirectionSeted;

        public InputService()
        {
            _inputActions = new PlayerInputActions();
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
            //ƒодумать, надо ли нам свой тик, или используем Vcontainer
        }

        private void OnRotatePerformed(InputAction.CallbackContext context)
        {
            Vector2 direction = Vector2.zero;

            if (YG2.envir.isDesktop)
            {
                CursorOrigin = new Vector2(Screen.width / 2f, Screen.height / 2f);
                Vector2 cursorPos = context.ReadValue<Vector2>();
                direction = cursorPos - CursorOrigin;
            }

            if (direction.sqrMagnitude > 0f)
                direction.Normalize();
            else
                direction = Vector2.zero;

            RotateDirectionSeted?.Invoke(direction);
        }

        private void OnRotateCanceled(InputAction.CallbackContext context)
        {
            RotateDirectionSeted?.Invoke(Vector2.zero);
        }

        public void EnableInput()
        {
            _inputActions.Player.Enable();
        }

        public void DisableInput()
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
