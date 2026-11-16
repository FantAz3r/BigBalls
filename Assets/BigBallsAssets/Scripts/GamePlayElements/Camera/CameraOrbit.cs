using System.Collections;
using UnityEngine;
using BigBalls.Services;
using VContainer;

public class CameraOrbit : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private Transform _target;
    [SerializeField] private float _offsetY = 1f;
    
    [Header("Orbit Settings")]
    [SerializeField] private float _distance = 5.0f;
    [SerializeField] private float _rotationSpeed = 50.0f;
    [SerializeField] private float _verticalAngle = 20.0f;

    [Header("Manual Control Settings")]
    [SerializeField] private float _manualRotationSensitivity = 0.5f; 
    [SerializeField] private float _minSpeedForAutoRotate = 0.5f; 
    [SerializeField] private float _zoomSensitivity = 0.5f; 
    [SerializeField] private float _minZoom = 2f;
    [SerializeField] private float _maxZoom = 10f;
    [SerializeField] private float _inertiaDamping = 2f;

    private IInputService _inputService;
    private Coroutine _orbitCoroutine;
    private bool _isMoving = false;

    private float _currentHorizontalAngle = 0f;
    private float _targetZoom;

    private int _rotateDirection =1 ;
    private Vector2 _lastInputDelta;
    private bool _isDragging;

    [Inject]
    public void Construct (InputServiceAdapter inputServiceAdapter)
    {
        _inputService = inputServiceAdapter.Get();
    }

    private void Awake ()
    {
        _targetZoom = _distance;
        _currentHorizontalAngle = 0f;
    }

    private void OnEnable ()
    {
        if (_inputService != null)
        {
            _inputService.CameraOrbitSeted += OnOrbitInput;
            _inputService.CameraZoomSeted += OnZoomInput;
        }
    }

    private void OnDisable ()
    {
        if (_inputService != null)
        {
            _inputService.CameraOrbitSeted -= OnOrbitInput;
            _inputService.CameraZoomSeted -= OnZoomInput;
        }
    }

    public void StartOrbit (Transform target)
    {
        _target = target;

        if (!_isMoving && target != null)
        {
            _isMoving = true;
            _orbitCoroutine = StartCoroutine(OrbitRoutine());
        }
    }

    public void StopOrbit ()
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

    private void OnOrbitInput (Vector2 delta)
    {
        if (delta.magnitude > 0.01f)
        {
            _isDragging = true;
            _lastInputDelta = -delta;
        }
        else
        {
            _isDragging = false;
        }
    }

    private void OnZoomInput (float delta)
    {
        _targetZoom -= delta * _zoomSensitivity;
        _targetZoom = Mathf.Clamp(_targetZoom, _minZoom, _maxZoom);
    }

    private IEnumerator OrbitRoutine ()
    {
        while (_isMoving)
        {
            if (_isDragging)
            {
                _currentHorizontalAngle += _lastInputDelta.x * _manualRotationSensitivity;
            }
            else
            {
                if (Mathf.Abs(_lastInputDelta.x) > _minSpeedForAutoRotate)
                {
                    _currentHorizontalAngle += _lastInputDelta.x * _manualRotationSensitivity;
                    _lastInputDelta.x = Mathf.Lerp(_lastInputDelta.x, 0, Time.deltaTime * _inertiaDamping);

                    _rotateDirection = _lastInputDelta.x > 0 ? 1 : -1;
                }
                else
                {
                    _currentHorizontalAngle += _rotateDirection *_rotationSpeed * Time.deltaTime;
                }
            }

            if (_currentHorizontalAngle >= 360f)
                _currentHorizontalAngle -= 360f;
            else if (_currentHorizontalAngle <= -360f) 
                _currentHorizontalAngle += 360f;

            _distance = Mathf.Lerp(_distance, _targetZoom, Time.deltaTime * 10f);
            float verticalRad = _verticalAngle * Mathf.Deg2Rad;
            float horizontalRad = _currentHorizontalAngle * Mathf.Deg2Rad;

            float horizontalDistance = _distance * Mathf.Cos(verticalRad);
            float heightOffset = _distance * Mathf.Sin(verticalRad);

            float x = _target.position.x + Mathf.Cos(horizontalRad) * horizontalDistance;
            float z = _target.position.z + Mathf.Sin(horizontalRad) * horizontalDistance;
            float y = _target.position.y + heightOffset + _offsetY;

            transform.position = new Vector3(x, y, z);

            transform.LookAt(_target.position + new Vector3(0, _offsetY, 0));
            yield return null;
        }
    }
}