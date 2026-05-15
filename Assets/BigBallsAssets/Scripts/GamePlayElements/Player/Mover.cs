using System.Linq;
using UnityEngine;

namespace BigBalls.GameplayObjects
{
    public class Mover
    {
        private readonly Transform _movebleObject;

        private LayerMask _obstacleLayerMask;
        private float _rayDistance = 1f;
        private Stat _moveSpeed;
        private Transform _movbleObject;

        public Mover(IStatContainer statholder, Transform transform)
        {
            _movbleObject = transform;
            _moveSpeed = statholder.Stats.Where(s => s.StatType == StatType.MoveSpeed).FirstOrDefault();
        }

        public Vector2 Direction { get; private set; }

        public void SetDirection(Vector2 direction) => Direction = direction;

        private void Move(Vector2 direction)
        {
            if (direction.sqrMagnitude < 0.001f)
            {
                return;
            }

            Vector3 moveDir = new Vector3(direction.x, 0f, direction.y).normalized;
            Vector3 offset = new Vector3(0, 1, 0);
            RaycastHit hit;
            bool isHit = Physics.Raycast(_movbleObject.position + offset, moveDir, out hit, _rayDistance, _obstacleLayerMask);

            Debug.DrawRay(_movbleObject.position + offset, moveDir * _rayDistance, isHit ? Color.red : Color.green);

            if (isHit)
            {
                return;
            }

            float moveStep = _moveSpeed.Value * Time.deltaTime;
            _movbleObject.Translate(moveDir * moveStep, Space.World);
        }
    }
}