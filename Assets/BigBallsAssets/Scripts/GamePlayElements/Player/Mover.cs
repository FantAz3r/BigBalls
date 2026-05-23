using System;
using System.Collections.Generic;
using UnityEngine;
using BigBalls.Services;
using BigBalls.StaticData;

namespace BigBalls.GameplayObjects
{
    public class Mover : IUpdateble, IDisposable
    {
        private readonly Vector3 _offset = new Vector3(0, 1, 0);
        private readonly IUpdateService _updateService;

        private Transform _movableObject;
        private LayerMask _obstacleLayerMask;
        private float _rayDistance = 1f;
        private Stat _moveSpeed;

        public Mover(Stat moveSpeed, Transform movebleObject, IUpdateService updateService, PlayerConfig playerConfig)
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

            Vector3 moveDir = new Vector3(direction.x, 0f, direction.y).normalized;
            float moveStep = _moveSpeed.CurrentValue * Time.deltaTime;

            List<Vector3> hitNormals = new List<Vector3>();

            Vector3[] rayStartPoints = new Vector3[]
            {
                _movableObject.position + _offset,
                _movableObject.position + _offset + _movableObject.right * 0.5f,
                _movableObject.position + _offset - _movableObject.right * 0.5f,
            };

            bool hasCollision = false;

            foreach (var startPoint in rayStartPoints)
            {

                if (Physics.Raycast(startPoint, moveDir, out RaycastHit hit, _rayDistance, _obstacleLayerMask))
                {
                    hasCollision = true;
                    hitNormals.Add(hit.normal);
                    Debug.DrawRay(startPoint, moveDir * _rayDistance, Color.red);
                }
                else
                {
                    Debug.DrawRay(startPoint, moveDir * _rayDistance, Color.green);
                }
            }

            if (hasCollision)
            {
                Vector3 adjustedDir = moveDir;

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
                _movableObject.Translate(moveDir * moveStep, Space.World);
            }
        }
    }
}