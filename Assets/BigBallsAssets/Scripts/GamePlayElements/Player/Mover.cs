using UnityEngine;
using System.Linq;
using BigBalls.Services;
using System;

namespace BigBalls.GameplayObjects
{
    public class Mover : IUpdateble, IDisposable
    {
        private readonly IUpdateService _updateService;
        private Transform _movebleObject;

        private LayerMask _obstacleLayerMask; //прокинуть ссылку
        private float _rayDistance = 1f;
        private Stat _moveSpeed;

        public Mover(IStatContainer statholder, Transform transform, IUpdateService updateService)
        {
            _updateService = updateService;
            _movebleObject = transform;
            _moveSpeed = statholder.Stats.Where(s => s.StatType == StatType.MoveSpeed).FirstOrDefault();
            _updateService.Register(this);
        }

        public Vector2 Direction { get; private set; }

        public void SetDirection(Vector2 direction)
        {
            Direction = direction;
        }

        public void Tick()
        {
            if(_movebleObject == null)
            {
                Dispose();
            }

            Move(Direction);
        }

        private void Move(Vector2 direction)
        {
            if (direction.sqrMagnitude < 0.001f)
            {
                return;
            }

            Vector3 moveDir = new Vector3(direction.x, 0f, direction.y).normalized;
            //Vector3 offset = new Vector3(0, 1, 0);
            //RaycastHit hit;
            //bool isHit = Physics.Raycast(_movbleObject.position + offset, moveDir, out hit, _rayDistance, _obstacleLayerMask);
            //
            //Debug.DrawRay(_movbleObject.position + offset, moveDir * _rayDistance, isHit ? Color.red : Color.green);
            //
            //if (isHit)
            //{
            //    return;
            //}

            float moveStep = _moveSpeed.Value * Time.deltaTime;
            _movebleObject.Translate(moveDir * moveStep, Space.World);
        }

        public void Dispose()
        {
            _updateService.Unregister(this);
        }
    }
}