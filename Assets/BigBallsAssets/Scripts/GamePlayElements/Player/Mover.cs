using BigBalls.Services;
using BigBalls.StaticData;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BigBalls.GameplayObjects
{
    public class Mover : IUpdateble, IDisposable
    {
        private readonly Vector3 _offset = new Vector3(0, 1, 0);
        private readonly IUpdateService _updateService;

        private Transform _movableObject;
        private LayerMask _obstacleLayerMask;
        private float _rayDistance = 0.5f;
        private Stat _moveSpeed;

        public Mover(Stat moveSpeed, Transform movebleObject, IUpdateService updateService, IEntityConfig playerConfig)
        {
            _updateService = updateService;
            _movableObject = movebleObject;
            _moveSpeed = moveSpeed;
            _obstacleLayerMask = playerConfig.ObstacleLayers;
            _updateService.Register(this);
        }

        public Vector2 Direction { get; private set; }

        public void SetDirection(Vector2 direction)
        {
            Direction = direction;
        }

        public void Tick()
        {
            if (_movableObject == null)
            {
                Dispose();
            }

            Move(Direction);
        }

        public void Dispose()
        {
            _updateService.Unregister(this);
        }

        private void Move(Vector2 direction)
        {
            if (direction.sqrMagnitude < 0.001f)
                return;

            Vector3 moveDirection = new Vector3(direction.x, 0f, direction.y).normalized;
            float moveStep = _moveSpeed.CurrentValue * Time.deltaTime;

            List<Vector3> hitNormals = new();

            if (HasCollision(hitNormals, moveDirection))
            {
                Vector3 adjustedDir = moveDirection;

                foreach (var normal in hitNormals)
                {
                    adjustedDir = Vector3.ProjectOnPlane(adjustedDir, normal);
                }

                adjustedDir = adjustedDir.normalized;

                if (adjustedDir.sqrMagnitude > 0.001f)
                {
                    _movableObject.Translate(adjustedDir * moveStep, Space.World);
                }
            }
            else
            {
                _movableObject.Translate(moveDirection * moveStep, Space.World);
            }
        }

        private bool HasCollision(List<Vector3> hitNormals, Vector3 moveDirection)
        {
            bool hasCollision = false;

            Vector3[] rayDirections = new Vector3[]
            {
                _movableObject.forward,
                _movableObject.forward + Quaternion.Euler(0, -30, 0) * _movableObject.forward,
                _movableObject.position + Quaternion.Euler(0, 30, 0) * _movableObject.forward,
            };

            foreach (var directions in rayDirections)
            {
                if (Physics.Raycast(_movableObject.position, moveDirection, out RaycastHit hit, _rayDistance, _obstacleLayerMask))
                {
                    hasCollision = true;
                    hitNormals.Add(hit.normal);
                    Debug.DrawRay(_movableObject.position, moveDirection * _rayDistance, Color.red);
                }
                else
                {
                    Debug.DrawRay(_movableObject.position, moveDirection * _rayDistance, Color.green);
                }
            }

            return hasCollision;
        }
    }
}