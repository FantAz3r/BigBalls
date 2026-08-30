using System.Collections;
using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private Transform _target;

    [Header("Orbit Settings")]
    [SerializeField] private float _distance = 5.0f;
    [SerializeField] private float _rotationSpeed = 50.0f;
    [SerializeField] private float _verticalAngle = 20.0f;

    private Coroutine _orbitCoroutine;
    private bool _isMoving = false;

    public void StartOrbit(Transform target)
    {
        _target = target;

        if (!_isMoving && target != null)
        {
            _isMoving = true;
            _orbitCoroutine = StartCoroutine(OrbitRoutine());
        }
    }

    public void StopOrbit()
    {
        if (_isMoving)
        {
            _isMoving = false;

            if (_orbitCoroutine != null)
            {
                StopCoroutine(_orbitCoroutine);
            }
        }
    }

    private IEnumerator OrbitRoutine()
    {
        float currentAngle = 0f;

        while (_isMoving)
        {
            currentAngle += _rotationSpeed * Time.deltaTime;

            if (currentAngle >= 360f) 
                currentAngle -= 360f;

            float verticalRad = _verticalAngle * Mathf.Deg2Rad;
            float horizontalRad = currentAngle * Mathf.Deg2Rad;

            float horizontalDistance = _distance * Mathf.Cos(verticalRad);

            float heightOffset = _distance * Mathf.Sin(verticalRad);

            float x = _target.position.x + Mathf.Cos(horizontalRad) * horizontalDistance;
            float z = _target.position.z + Mathf.Sin(horizontalRad) * horizontalDistance;
            float y = _target.position.y + heightOffset;

            transform.position = new Vector3(x, y, z);

            transform.LookAt(_target);

            yield return null; 
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (_target != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(_target.position, _distance);
        }
    }
}
