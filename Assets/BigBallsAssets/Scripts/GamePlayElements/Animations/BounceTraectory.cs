using System.Collections.Generic;
using BigBalls.GameplayObjects;
using BigBalls.Services;
using UnityEngine;
using VContainer;

public class BounceTraectory : MonoBehaviour
{
    [Header("Настройки линии")]
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private float _maxTotalLength = 20f;
    [SerializeField] private int _maxSegments = 3;
    [SerializeField] private LayerMask _collisionMask;
    [SerializeField] private Vector3 _offset = new Vector3(0.5f, 0.5f, 0.5f);

    private IInputService _inputService;
    private Vector3 _startPosition;
    private List<Vector3> _trajectoryPoints = new List<Vector3>();
    private Vector3 _rotateDirection;
    private Transform _firePoint;

    [Inject]
    public void Construct (IInputService inputService)
    {
        _firePoint = GetComponent<Player>().WeaponSpawnPoint;
        _inputService = inputService;
        _inputService.RotateDirectionSeted += OnMoveDirectionSeted;
    }

    private void OnDestroy ()
    {
        if (_inputService != null)
        {
            _inputService.RotateDirectionSeted -= OnMoveDirectionSeted;
        }
    }

    private void Update ()
    {
        _startPosition = _firePoint.position;
        DrawTrajectory(_startPosition, _rotateDirection);
    }

    private void OnMoveDirectionSeted (Vector2 cursorScreenPos)
    {
        Vector3 playerScreenPos = Camera.main.WorldToScreenPoint(transform.position);
        Vector2 direction = cursorScreenPos - new Vector2(playerScreenPos.x, playerScreenPos.y);

        if (direction.sqrMagnitude > 0f)
            direction.Normalize();
        else
            direction = Vector2.zero;

        Vector3 direction3D = new Vector3(direction.x, 0f, direction.y).normalized;

        if (direction3D == Vector3.zero)
        {
            _lineRenderer.positionCount = 0;
            return;
        }

        _rotateDirection = direction3D;
    }

    private void DrawTrajectory (Vector3 origin, Vector3 direction)
    {
        _trajectoryPoints.Clear();
        _trajectoryPoints.Add(origin);

        Vector3 currentPosition = origin;
        Vector3 currentDirection = direction;
        float totalLength = 0f;
        int segmentCount = 0;

        while (segmentCount < _maxSegments && totalLength < _maxTotalLength)
        {
            float distanceToCollision = CalculateDistanceToCollision(currentPosition, currentDirection);
            float segmentLength = Mathf.Min(distanceToCollision, _maxTotalLength - totalLength);

            if (distanceToCollision >= _maxTotalLength - totalLength)
            {
                Vector3 endPoint = currentPosition + currentDirection * segmentLength;
                _trajectoryPoints.Add(endPoint);
                totalLength += segmentLength;
                break;
            }

            Vector3 collisionPoint = currentPosition + currentDirection * distanceToCollision;
            _trajectoryPoints.Add(collisionPoint);
            totalLength += distanceToCollision;

            segmentCount++;
            if (segmentCount >= _maxSegments)
            {
                break;
            }

            Vector3 reflectedDirection = ReflectDirection(currentPosition, currentDirection, collisionPoint);

            currentPosition = collisionPoint;
            currentDirection = reflectedDirection;

            currentPosition += currentDirection * 0.01f;
        }

        UpdateLineRenderer();
    }

    private float CalculateDistanceToCollision (Vector3 origin, Vector3 direction)
    {
        RaycastHit hit;

        if (Physics.Raycast(origin, direction, out hit, _maxTotalLength, _collisionMask))
        {
            return hit.distance;
        }

        return _maxTotalLength;
    }

    private Vector3 ReflectDirection (Vector3 origin, Vector3 direction, Vector3 collisionPoint)
    {
        RaycastHit hit;

        if (Physics.Raycast(origin, direction, out hit, _maxTotalLength, _collisionMask))
        {
            return Vector3.Reflect(direction, hit.normal).normalized;
        }

        return direction;
    }

    private void UpdateLineRenderer ()
    {
        if (_trajectoryPoints.Count < 2)
        {
            _lineRenderer.positionCount = 0;
            return;
        }

        _lineRenderer.positionCount = _trajectoryPoints.Count;

        for (int i = 0; i < _trajectoryPoints.Count; i++)
        {
            _lineRenderer.SetPosition(i, _trajectoryPoints[i]);
        }
    }
}
