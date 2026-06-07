using BigBalls.Services;
using UnityEngine;

namespace BigBalls.GameplayObjects
{
    public class Rotator : IUpdateble, ISubscribable
    {
        private readonly IUpdateService _updateService;

        private bool _canRotate = true;
        private Stat _rotationSpeed;

        public Rotator(Stat rotationSpeed, Transform rotatebleObject, IUpdateService updateService)
        {
            _updateService = updateService;
            RotatebleObject = rotatebleObject;
            _rotationSpeed = rotationSpeed;
        }

        public Transform RotatebleObject { get; private set; }

        public Vector2 CurrentDirection { get; private set; }
        public void SetDirection(Vector2 direction) => CurrentDirection = direction;

        public void Subscribe()
        {
            _updateService.Register(this);

        }

        public void Unsubscribe()
        {
            _updateService.Unregister(this);
            RotatebleObject = null;

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
            float currentAngle = RotatebleObject.eulerAngles.y;
            float newAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, _rotationSpeed.CurrentValue);

            RotatebleObject.rotation = Quaternion.Euler(0f, newAngle, 0f);
        }
    }
}