using BigBalls.Services;
using System;
using UnityEngine;

namespace BigBalls.GameplayObjects
{
    public class PlayerMover
    {
        private readonly Rotator _rotator;
        private readonly Mover _mover;
        private readonly IInputService _inputService;

        public PlayerMover(IInputService inputService, Rotator rotator, Mover mover)
        {
            _inputService = inputService;
            _rotator = rotator;
            _mover = mover;

            _inputService.MoveDirectionSeted += OnMove;
            _inputService.RotateDirectionSeted += OnRotate;
        }

        public event Action Moved;

        private void OnMove(Vector2 direction)
        {
            _mover.SetDirection(direction);
        }

        private void OnRotate(Vector2 direction)
        {
            _rotator.SetDirection(direction);
        }
    }
}