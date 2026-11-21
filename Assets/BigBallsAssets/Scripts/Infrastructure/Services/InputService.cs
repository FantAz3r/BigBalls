using System;
using BigBalls.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer.Unity;

namespace BigBalls.Services
{
    public class InputService : IInputService, IStartable, ITickable
    {
        private readonly IWindowService _windowService;
        private readonly ITimeService _timeService;
        private PlayerInputActions _inputActions;

        public event Action<Vector2> MoveDirectionSeted;
        public event Action<Vector2> RotateDirectionSeted;
        public event Action Attack;

        public event Action<Vector2> CameraOrbitSeted;
        public event Action<float> CameraZoomSeted;

        public InputService (IWindowService windowService, ITimeService timeService)
        {
            _inputActions = new PlayerInputActions();
            _windowService = windowService;
            _timeService = timeService;
        }

        public Vector2 CursorOrigin { get; set; }

        public void Start ()
        {
            DisableCamera();
            DisablePlayer();
            DisableUI();
        }

        public void EnableGameplayInput()
        {
            _timeService.IsPausedChanged += OnPauseChanged;
        }

        public IInputService GetSelf () => this;

        public void Tick ()
        {
            bool isHeld = Pointer.current != null && Pointer.current.press.isPressed;

            if (!isHeld)
            {
                CameraOrbitSeted?.Invoke(Vector2.zero);
                return;
            }

            Vector2 delta = _inputActions.Camera.Orbit.ReadValue<Vector2>();
            CameraOrbitSeted?.Invoke(delta);
        }

        private void OnZoomPerformed (InputAction.CallbackContext context)
        {
            float scrollValue = context.ReadValue<float>();
            CameraZoomSeted?.Invoke(scrollValue);
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
            if (_inputActions == null)
                return;
            _inputActions.UI.Enable();
            _inputActions.UI.Pause.performed += PauseGame;
            _inputActions.UI.Timescale1.performed += SetTimescale1;
            _inputActions.UI.Timescale2.performed += SetTimescale2;
            _inputActions.UI.Timescale3.performed += SetTimescale3;

        }

        public void DisableUI ()
        {
            if (_inputActions == null)
                return;
            _inputActions.UI.Disable();
            _inputActions.UI.Pause.performed -= PauseGame;
            _inputActions.UI.Timescale1.performed -= SetTimescale1;
            _inputActions.UI.Timescale2.performed -= SetTimescale2;
            _inputActions.UI.Timescale3.performed -= SetTimescale3;
        }

        public void EnablePlayer ()
        {
            if (_inputActions == null)
                return;
            _inputActions.Player.Enable();
            _inputActions.Player.Move.performed += OnMovePerformed;
            _inputActions.Player.Move.canceled += OnMoveCanceled;
            _inputActions.Player.Rotate.performed += OnRotatePerformed;
            _inputActions.Player.Rotate.canceled += OnRotateCanceled;
            _inputActions.Player.Attack.performed += OnAttack;
        }

        public void DisablePlayer ()
        {
            if (_inputActions == null)
                return;
            _inputActions.Player.Disable();
            _inputActions.Player.Move.performed -= OnMovePerformed;
            _inputActions.Player.Move.canceled -= OnMoveCanceled;
            _inputActions.Player.Rotate.performed -= OnRotatePerformed;
            _inputActions.Player.Rotate.canceled -= OnRotateCanceled;
        }

        public void Dispose ()
        {
            if (_inputActions == null)
                return;

            _timeService.IsPausedChanged -= OnPauseChanged;
            _inputActions.Dispose();
            _inputActions = null;
        }

        public void EnableCamera ()
        {
            if (_inputActions == null)
                return;
            _inputActions.Camera.Enable();
            _inputActions.Camera.Zoom.performed += OnZoomPerformed;
        }

        public void DisableCamera ()
        {
            if (_inputActions == null)
                return;
            _inputActions.Camera.Disable();
            _inputActions.Camera.Zoom.performed -= OnZoomPerformed;
        }

        private void OnPauseChanged(bool pause)
        {
            if (pause)
            {
                DisablePlayer();
                DisableUI();
            }
            else
            {
                EnablePlayer();
                EnableUI();
            }

        }
    }
}
