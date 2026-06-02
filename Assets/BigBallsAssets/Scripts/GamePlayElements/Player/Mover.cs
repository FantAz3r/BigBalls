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

        private LayerMask _obstacleLayerMask;
        private Stat _moveSpeed;

        private Vector3[] _raycastDirections;
        private Vector3[] _raycastPoints;

        public Mover(Stat moveSpeed, Transform movebleObject, IUpdateService updateService, IEntityConfig playerConfig, IRaycastService raycastService)
        {
            _updateService = updateService;
            _raycastService = raycastService;
            MovableObject = movebleObject;
            _moveSpeed = moveSpeed;
            _obstacleLayerMask = playerConfig.ObstacleLayers;
        }

        public Transform MovableObject { get; private set;}
        public Vector2 Direction { get; private set; }

        public void Subscribe()
        {
            _updateService.Register(this);
        }

        public void Unsubscribe()
        {
            _updateService.Unregister(this);
        }

        public void SetDirection(Vector2 direction)
        {
            Direction = direction;
        }

        public void Tick()
        {
            Move(Direction);
        }


        public void SetReycastInfo(Vector3[] rarcastDirections, Vector3[] raycastPoints)
        {
            _raycastDirections = rarcastDirections;
            _raycastPoints = raycastPoints;
        }

        private void Move(Vector2 direction)
        {
            if (direction.sqrMagnitude < 0.001f)
                return;

            Vector3 moveDirection = new Vector3(direction.x, 0f, direction.y).normalized;
            float moveStep = _moveSpeed.CurrentValue * Time.deltaTime;  

            if (HasCollision(out List<Vector3> hitNormals, _obstacleLayerMask))
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

        public bool HasCollision(out List<Vector3> hitNormals, LayerMask obstacleLayerMask)
        {
            hitNormals = new List<Vector3>();
            bool hasCollision = false;

            for (int i = 0; i < _raycastPoints.Length; i++)
            {
                Vector3 startPoint = _raycastPoints[i] + MovableObject.position;
                Vector3 direction = _raycastDirections[i].normalized;

                if (Physics.Raycast(startPoint, direction, out RaycastHit hit, 0.5f, obstacleLayerMask))
                {
                    hasCollision = true;
                    hitNormals.Add(hit.normal);
                    Debug.DrawRay(startPoint, direction * hit.distance, Color.red);
                }
                else
                {
                    Debug.DrawRay(startPoint, direction * 0.5f, Color.green);
                }
            }

            return hasCollision;
        }
    }
}