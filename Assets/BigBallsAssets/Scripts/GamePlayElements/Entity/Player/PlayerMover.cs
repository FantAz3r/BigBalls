using System;
using BigBalls.Services;
using UnityEngine;
using VContainer;

namespace BigBalls.GameplayObjects
{
    public class PlayerMover : ISubscribable, IUpdateble
    {
        private readonly Vector3 _offset = new Vector3(0, 0.25f, 0);
        private readonly Rotator _rotator;
        private readonly IMover _mover;
        private readonly int _reycastAngle = 30;

        private Vector2 _direction;
        private Vector3[] _raycastPoints = new Vector3[3];
        private Vector3[] _raycastDirections = new Vector3[3];

        private IInputService _inputService;
        private IUpdateService _updateService;
        public PlayerMover (Rotator rotator, IMover mover)
        {
            _rotator = rotator;
            _mover = mover;

            Vector3 origin = mover.MovableObject.position + _offset;
            _raycastPoints[0] = origin;
            _raycastPoints[1] = origin;
            _raycastPoints[2] = origin;
        }

        [Inject]
        public void Construct (InputServiceAdapter inputServiceAdapter, IUpdateService updateService)
        {
            _inputService = inputServiceAdapter.Get();
            _updateService = updateService;
        }

        public event Action Moved;

        public void Subscribe ()
        {
            _updateService.Register(this);
            _inputService.MoveDirectionSeted += OnMove;
            _inputService.RotateDirectionSeted += OnRotate;
        }

        public void Tick ()
        {

        }

        public void Unsubscribe ()
        {
            _updateService.Unregister(this);

            _inputService.MoveDirectionSeted -= OnMove;
            _inputService.RotateDirectionSeted -= OnRotate;
        }

        private void OnMove (Vector2 direction)
        {
            Vector3 mainDirection = new Vector3(direction.normalized.x, 0, direction.normalized.y);
            Vector3 leftDirection = Quaternion.Euler(0, -_reycastAngle, 0) * mainDirection;
            Vector3 rightDirection = Quaternion.Euler(0, _reycastAngle, 0) * mainDirection;

            _raycastDirections[0] = mainDirection;
            _raycastDirections[1] = leftDirection;
            _raycastDirections[2] = rightDirection;

            _mover.SetReycastInfo(_raycastDirections, _raycastPoints);
            _mover.SetDirection(direction);
        }

        private void OnRotate (Vector2 cursorScreenPos)
        {
            if (_rotator.RotatebleObject == null || Camera.main == null || cursorScreenPos == Vector2.zero)
            {
                _rotator.SetDirection(Vector2.zero);
                return;
            }

            Vector2 direction;

            if (_inputService.GetSelf() is InputService)
            {
                Vector3 playerScreenPos = Camera.main.WorldToScreenPoint(_rotator.RotatebleObject.position);
                direction = cursorScreenPos - new Vector2(playerScreenPos.x, playerScreenPos.y);

                if (direction.sqrMagnitude > 0f)
                    direction.Normalize();
                else
                    direction = Vector2.zero;
            }
            else
            {
                direction = cursorScreenPos;
            }

            _rotator.SetDirection(direction);
        }
    }
}