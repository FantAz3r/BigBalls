using BigBalls.Services;
using System;
using UnityEngine;

namespace BigBalls.GameplayObjects
{
    public class PlayerMover : ISubscribable
    {
        private readonly Vector3 Offset = new Vector3(0, 0.25f, 0);
        private readonly Rotator _rotator;
        private readonly Mover _mover;
        private readonly IInputService _inputService;
        private readonly int ReycastAngle = 30;

        private Vector3[] _raycastPoints = new Vector3[3];
        private Vector3[] _raycastDirections = new Vector3[3];

        public PlayerMover(IInputService inputService, Rotator rotator, Mover mover)
        {
            _inputService = inputService;
            _rotator = rotator;
            _mover = mover;

            Vector3 origin = mover.MovableObject.position + Offset;
            _raycastPoints[0] = origin;
            _raycastPoints[1] = origin;
            _raycastPoints[2] = origin;
        }

        public event Action Moved;

        public void Subscribe()
        {
            _inputService.MoveDirectionSeted += OnMove;
            _inputService.RotateDirectionSeted += OnRotate;
        }

        public void Unsubscribe()
        {
            _inputService.MoveDirectionSeted -= OnMove;
            _inputService.RotateDirectionSeted -= OnRotate;
        }

        private void OnMove(Vector2 direction)
        {
            Vector3 mainDirection = new Vector3(direction.normalized.x, 0, direction.normalized.y);
            Vector3 leftDirection = Quaternion.Euler(0, -ReycastAngle, 0) * mainDirection;
            Vector3 rightDirection = Quaternion.Euler(0, ReycastAngle, 0) * mainDirection;

            _raycastDirections[0] = mainDirection;
            _raycastDirections[1] = leftDirection;
            _raycastDirections[2] = rightDirection;

            _mover.SetReycastInfo(_raycastDirections, _raycastPoints);
            _mover.SetDirection(direction);
        }

        private void OnRotate(Vector2 cursorScreenPos)
        {
            if (_rotator.RotatebleObject == null || Camera.main == null || cursorScreenPos == Vector2.zero)
            {
                _rotator.SetDirection(Vector2.zero);
                return;
            }

            Vector3 playerScreenPos = Camera.main.WorldToScreenPoint(_rotator.RotatebleObject.position);
            Vector2 direction = cursorScreenPos - new Vector2(playerScreenPos.x, playerScreenPos.y);

            if (direction.sqrMagnitude > 0f)
                direction.Normalize();
            else
                direction = Vector2.zero;

            _rotator.SetDirection(direction);
        }
    }
}