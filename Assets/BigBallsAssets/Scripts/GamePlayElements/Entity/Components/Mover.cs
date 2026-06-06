using BigBalls.Services;
using BigBalls.StaticData;
using System.Collections.Generic;
using UnityEngine;

namespace BigBalls.GameplayObjects
{
    public class Mover : IUpdateble, ISubscribable
    {
        private readonly IUpdateService _updateService;
        private readonly IRaycastService _raycastService;

        private readonly LayerMask _obstacleLayerMask;
        private readonly Stat _moveSpeed;

        private Vector3[] _raycastDirections;
        private Vector3[] _raycastPoints;

        public Transform MovableObject { get; private set; }
        public Vector2 Direction { get; private set; }

        public Mover(Stat moveSpeed, Transform movableObject, IUpdateService updateService, IRaycastService raycastService = null, IEntityConfig playerConfig = null)
        {
            _updateService = updateService;
            _raycastService = raycastService;
            MovableObject = movableObject;
            _moveSpeed = moveSpeed;
            _obstacleLayerMask = playerConfig?.ObstacleLayers ?? 0;
        }

        public void Subscribe() => _updateService.Register(this);
        public void Unsubscribe() => _updateService.Unregister(this);
        public void SetDirection(Vector2 direction) => Direction = direction;
        public void SetReycastInfo(Vector3[] directions, Vector3[] points)
        {
            _raycastDirections = directions;
            _raycastPoints = points;
        }

        public void Tick() => Move(Direction);

        private void Move(Vector2 direction)
        {
            if (direction.sqrMagnitude < 0.001f) return;

            bool hasCollision = false;
            List<Vector3> hitNormals = new();
            Vector3 moveDirection = new Vector3(direction.x, 0f, direction.y).normalized;
            float moveStep = _moveSpeed.CurrentValue * Time.deltaTime;

            if (_raycastService != null)
            {
                hasCollision = _raycastService.CheckCollisions(MovableObject, _raycastDirections, _raycastPoints, 0.5f, _obstacleLayerMask, out List<Vector3> hitNormalsResult);
                hitNormals = hitNormalsResult;
            }

            if (hasCollision)
            {
                Vector3 adjustedDirection = moveDirection;

                foreach (var normal in hitNormals)
                {
                    adjustedDirection = Vector3.ProjectOnPlane(adjustedDirection, normal);
                }

                adjustedDirection = adjustedDirection.normalized;

                if (adjustedDirection.sqrMagnitude > 0.001f)
                {
                    MovableObject.Translate(adjustedDirection * moveStep, Space.World);
                }
            }
            else
            {
                MovableObject.Translate(moveDirection * moveStep, Space.World);
            }
        }
    }
}