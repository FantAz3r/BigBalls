using BigBalls.Services;
using System;
using UnityEngine;

namespace BigBalls.GameplayObjects
{
    public class Rotator : IUpdateble, ISubscribable
    {
        private readonly IUpdateService _updateService;

        private  Transform _rotatebleObject;
        private bool _canRotate = true;
        private Stat _rotationSpeed;

        public Rotator(Stat rotationSpeed, Transform rotatebleObject, IUpdateService updateService)
        {
            _updateService = updateService;
            _rotatebleObject = rotatebleObject;
            _rotationSpeed = rotationSpeed;
        }

        public Vector2 CurrentDirection { get; private set; }
        public void SetDirection(Vector2 direction) => CurrentDirection = direction;

        public void Subscribe()
        {
            _updateService.Register(this);

        }

        public void Unsubscribe()
        {
            _updateService.Unregister(this);
            _rotatebleObject = null;

        }

        public void Tick()
        {
            if (_canRotate)
            {
                Rotate();
            }
        }

        private void Rotate()
        {
            Vector3 direction = new Vector3(CurrentDirection.x, 0f, CurrentDirection.y);

            if (direction.sqrMagnitude < 0.0001f)
                return;

            direction.Normalize();

            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            float currentAngle = _rotatebleObject.eulerAngles.y;
            float newAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, _rotationSpeed.CurrentValue);

            _rotatebleObject.rotation = Quaternion.Euler(0f, newAngle, 0f);
        }
    }
}