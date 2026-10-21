using BigBalls.Services;
using UnityEngine;
using VContainer;

namespace BigBalls.GameplayObjects
{
    public class Rotator : IUpdateble, ISubscribable
    {
        private const float MaxStepPerFrame = 180f;

        private IUpdateService _updateService;

        private bool _canRotate = true;
        private Stat _rotationSpeed;
        private Transform _target;

        private float _currentAngle;
        private bool _hasAngle;

        public Rotator (Stat rotationSpeed, Transform rotatebleObject)
        {
            _rotationSpeed = rotationSpeed;
            RotatebleObject = rotatebleObject;

            if (rotatebleObject != null)
            {
                _currentAngle = rotatebleObject.eulerAngles.y;
                _hasAngle = true;
            }
        }

        [Inject]
        public void Construct (IUpdateService updateService)
        {
            _updateService = updateService;
        }

        public Transform RotatebleObject { get; private set; }

        public Vector2 CurrentDirection { get; private set; }
        public void SetDirection (Vector2 direction) => CurrentDirection = direction;
        public void SetTarget (Transform target) => _target = target;

        public void Subscribe ()
        {
            _updateService?.Register(this);
        }

        public void Unsubscribe ()
        {
            _updateService?.Unregister(this);
            _target = null;
        }

        public void Tick ()
        {
            if (_canRotate)
                Rotate();
        }

        private void Rotate ()
        {
            if (RotatebleObject == null || _rotationSpeed == null)
                return;

            Vector3 targetPos = _target != null
                ? _target.position
                : new Vector3(CurrentDirection.x, 0f, CurrentDirection.y) + RotatebleObject.position;

            Vector3 direction = targetPos - RotatebleObject.position;
            direction.y = 0f;

            if (direction.sqrMagnitude < 0.0001f)
                return;

            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

            if (!_hasAngle)
            {
                _currentAngle = RotatebleObject.eulerAngles.y;
                _hasAngle = true;
            }

            float step = _rotationSpeed.CurrentValue * Time.deltaTime;
            step = Mathf.Min(step, MaxStepPerFrame);

            _currentAngle = Mathf.MoveTowardsAngle(_currentAngle, targetAngle, step);
            _currentAngle = Mathf.Repeat(_currentAngle, 360f);

            RotatebleObject.rotation = Quaternion.Euler(0f, _currentAngle, 0f);
        }
    }
}