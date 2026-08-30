using BigBalls.Configs;
using BigBalls.Services;
using System.Collections.Generic;
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

        private List<MovementModifier> _movementModifiers = new List<MovementModifier>();

        public Transform MovableObject { get; private set; }
        [field: SerializeField] public Vector2 Direction { get; private set; }

        public MoverPhythics(Stat moveSpeed, Transform movableObject, IUpdateService updateService, Rigidbody rigidbody, IRaycastService raycastService = null, IEntityConfig playerConfig = null)
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

        public void Subscribe() => _updateService.Register(this);
        public void Unsubscribe() => _updateService.Unregister(this);

        public void SetDirection(Vector2 direction)
        {
            _target = null;
            Direction = direction;
        }

        public void SetTarget(Transform target) => _target = target;

        public void SetReycastInfo(Vector3[] directions, Vector3[] points)
        {
            _raycastDirections = directions;
            _raycastPoints = points;
        }
        public void AddModifier(MovementModifier modifier)
        {
            _movementModifiers.Add(modifier);
        }

        public void RemoveModifier(MovementModifier modifier)
        {
            _movementModifiers.Remove(modifier);
        }

        public void ClearModifiers()
        {
            _movementModifiers.Clear();
        }

        public void AddPush(Vector3 pushDirection, float force, float duration = 0.5f)
        {
            AddModifier(new PushModifier(pushDirection * force, duration));
        }

        public void AddInstantPush(Vector3 pushDirection, float force)
        {
            AddModifier(new InstantPushModifier(pushDirection * force));
        }

        public void AddConstantForce(Vector3 direction, float duration)
        {
            AddModifier(new ConstantForceModifier(direction, duration));
        }

        public void Tick()
        {
            if (_rigidbody == null)
                return;

            _movementModifiers.RemoveAll(m => m is PushModifier pm && pm.IsActive == false);
            _movementModifiers.RemoveAll(m => m is ConstantForceModifier cfm && !cfm.IsActive);

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
                moveDirection = new Vector3(Direction.x, 0f, Direction.y).normalized;
            }

            Move(moveDirection);
        }

        public Vector3 GetCurrentVelocity()
        {
            if (_rigidbody == null)
                return Vector3.zero;

            return _rigidbody.velocity;
        }

        public bool HasActiveModifiers() => _movementModifiers.Count > 0;

        private void Move(Vector3 moveDirection)
        {
            Vector3 velocity = moveDirection * _moveSpeed.CurrentValue;

            foreach (var modifier in _movementModifiers)
            {
                velocity = modifier.Modify(velocity);
            }

            _rigidbody.velocity = new Vector3(velocity.x, 0f, velocity.z);
        }
    }
}