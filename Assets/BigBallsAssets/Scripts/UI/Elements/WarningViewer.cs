using BigBalls.GameplayObjects;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class WarningViewer : MonoBehaviour
{
    [Header("Shake")]
    [SerializeField] private float _shakeDuration = 0.3f;
    [SerializeField] private float _minShakeStrength = 0.02f;
    [SerializeField] private float _maxShakeStrength = 0.4f;

    [Header("Color")]
    [SerializeField] private Color _startColor = Color.white;
    [SerializeField] private Color _alertColor = Color.red;

    private Image _image;
    private Camera _camera;
    private EntityTrigger _entityTrigger;
    private Tween _shakeTween;
    private Vector2 _basePosition;
    private bool _baseCaptured;
    private float _currentProgress;
    private float _currentStrength = -1f;

    private void Awake ()
    {
        _image = GetComponent<Image>();
        _entityTrigger = GetComponentInParent<Enemy>().EntityTrigger;
        CaptureBasePosition();
        gameObject.SetActive(true);
    }

    private void OnEnable ()
    {
        _camera = Camera.main;

        if (_entityTrigger != null)
        {
            _entityTrigger.PlayerDetected += Show;
            _entityTrigger.PlayerLost += Hide;
            _entityTrigger.DetectionProgressChanged += OnProgress;
        }

        _image.enabled = false;
    }

    private void OnDisable ()
    {
        if (_entityTrigger != null)
        {
            _entityTrigger.PlayerDetected -= Show;
            _entityTrigger.PlayerLost -= Hide;
            _entityTrigger.DetectionProgressChanged -= OnProgress;
        }

        KillTween();
    }

    private void OnProgress (float progress)
    {
        _currentProgress = Mathf.Clamp01(progress);

        _image.color = Color.Lerp(_startColor, _alertColor, _currentProgress);
        float targetStrength = Mathf.Lerp(_minShakeStrength, _maxShakeStrength, _currentProgress);

        if (Mathf.Abs(targetStrength - _currentStrength) > 0.02f)
        {
            _currentStrength = targetStrength;
            RestartShake(targetStrength);
        }
    }

    private void RestartShake (float strength)
    {
        KillTween();

        if (!_image.enabled)
            return;

        _image.rectTransform.anchoredPosition = _basePosition;

        _shakeTween = _image.rectTransform
            .DOShakePosition(_shakeDuration, strength)
            .SetLoops(-1, LoopType.Restart);
    }

    private void Show ()
    {
        _image.enabled = true;
        FaceCamera();

        _currentProgress = 0f;
        _currentStrength = -1f;
        _image.color = _startColor;
        RestartShake(_minShakeStrength);
    }

    private void Hide ()
    {
        KillTween();
        _image.rectTransform.anchoredPosition = _basePosition;
        _image.enabled = false;
        _currentProgress = 0f;
        _currentStrength = -1f;
    }

    private void KillTween ()
    {
        if (_shakeTween != null && _shakeTween.IsActive())
            _shakeTween.Kill();
    }

    private void FaceCamera ()
    {
        if (_camera != null)
        {
            transform.LookAt(_camera.transform);
            transform.localRotation = Quaternion.Euler(-transform.localRotation.eulerAngles.x, 0, 0);
        }
    }

    private void CaptureBasePosition ()
    {
        if (_baseCaptured)
            return;

        _basePosition = _image.rectTransform.anchoredPosition;
        _baseCaptured = true;
    }
}