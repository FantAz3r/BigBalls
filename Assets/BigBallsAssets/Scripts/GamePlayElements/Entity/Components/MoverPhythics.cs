using BigBalls.Configs;
using BigBalls.Services;
using UnityEngine;

namespace BigBalls.GameplayObjects
{
    public class MoverPhythics : IMover, ISubscribable, IUpdateble
    {
        private readonly IUpdateService _updateService;
        private readonly IRaycastService _raycastService;

        private readonly LayerMask _obstacleLayerMask;
        private readonly Stat _moveSpeed;

        private Rigidbody _rigidbody;
        private Transform _target;
        private Vector3[] _raycastDirections;
        private Vector3[] _raycastPoints;

        public Transform MovableObject { get; private set; }
        [field: SerializeField] public Vector2 Direction { get; private set; }

        public MoverPhythics (Stat moveSpeed, Transform movableObject, IUpdateService updateService, Rigidbody rigidbody, IRaycastService raycastService = null, IEntityConfig playerConfig = null)
        {
            _rigidbody = rigidbody;
            _updateService = updateService;
            _raycastService = raycastService;
            MovableObject = movableObject;
            _moveSpeed = moveSpeed;
            _obstacleLayerMask = playerConfig?.ObstacleLayers ?? 0;

            if (_rigidbody != null)
            {
                _rigidbody.useGravity = false;
                _rigidbody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
                _rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
                _rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
            }
        }

        public void Subscribe () => _updateService.Register(this);
        public void Unsubscribe () => _updateService.Unregister(this);
        public void SetDirection (Vector2 direction)
        {
            _target = null;
            Direction = direction;
        }

        public void SetTarget (Transform target) => _target = target;

        public void SetReycastInfo (Vector3[] directions, Vector3[] points)
        {
            _raycastDirections = directions;
            _raycastPoints = points;
        }

        public void Tick ()
        {
            if (_rigidbody == null) return;

            Vector3 moveDirection;

            if (_target != null)
            {
                Vector3 dirToTarget = _target.position - MovableObject.position;
                dirToTarget.y = 0;

                if (dirToTarget.sqrMagnitude > 0.001f)
                {
                    moveDirection = dirToTarget.normalized;
                }
                else
                {
                    _rigidbody.velocity = Vector3.zero;
                    return;
                }
            }
            else
            {
                if (Direction.sqrMagnitude < 0.001f)
                {
                    _rigidbody.velocity = Vector3.zero;
                    return;
                }

                moveDirection = new Vector3(Direction.x, 0f, Direction.y).normalized;
            }

            Move(moveDirection);
        }

        private void Move (Vector3 moveDirection)
        {
            Vector3 targetVelocity = moveDirection * _moveSpeed.CurrentValue;
            _rigidbody.velocity = new Vector3(targetVelocity.x, 0f, targetVelocity.z);
        }
    }
}