using BigBalls.Services;
using UnityEngine;

namespace BigBalls.GameplayObjects
{
    public class Rotator : IUpdateble, ISubscribable
    {
        private readonly IUpdateService _updateService;

        private bool _canRotate = true;
        private Stat _rotationSpeed;
        private Transform _target;
        public Rotator(Stat rotationSpeed, Transform rotatebleObject, IUpdateService updateService)
        {
            _updateService = updateService;
            RotatebleObject = rotatebleObject;
            _rotationSpeed = rotationSpeed;
        }

        public Transform RotatebleObject { get; private set; }

        public Vector2 CurrentDirection { get; private set; }
        public void SetDirection(Vector2 direction) => CurrentDirection = direction;
        public void SetTarget(Transform target) => _target = target;

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
            Vector3 targetPos;

            if (_target != null)
            {
                targetPos = _target.position;
            }
            else
            {
                targetPos = new Vector3(CurrentDirection.x, 0, CurrentDirection.y) + RotatebleObject.position;
            }

            Vector3 direction = targetPos - RotatebleObject.position;
            direction.y = 0;

            if (direction.sqrMagnitude < 0.0001f)
                return;

            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            float currentAngle = RotatebleObject.eulerAngles.y;

            float newAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, _rotationSpeed.CurrentValue * Time.deltaTime);

            RotatebleObject.rotation = Quaternion.Euler(0f, newAngle, 0f);
        }

    }
}