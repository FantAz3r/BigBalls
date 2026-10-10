using BigBalls.GameplayObjects;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class WarningViewer : MonoBehaviour
{
    private Image _image;
    private float _shakeDuration = 3f;
    private float _shakeStrength = 0.1f;
    private Camera _camera;
    private EntityTrigger _entityTrigger;
    private Tween _shakeTween;
    private Vector3 _startPosition;

    private void Awake ()
    {
        _image = GetComponent<Image>();
        _entityTrigger = GetComponentInParent<Enemy>().EntityTrigger;
        gameObject.SetActive(true);
    }

    private void OnEnable ()
    {
        _camera = Camera.main;

        _entityTrigger.PlayerDetected += Show;
        _entityTrigger.PlayerLost += Hide;
        _image.enabled = false;
    }

    private void OnDisable ()
    {
        _entityTrigger.PlayerDetected -= Show;
        _entityTrigger.PlayerLost -= Hide;

        KillTween();
    }

    private void Show ()
    {
        _image.enabled = true;
        FaceCamera();
        KillTween();

        _shakeTween = _image.rectTransform.DOShakePosition(_shakeDuration, _shakeStrength)
            .SetLoops(-1, LoopType.Restart);
    }

    private void Hide ()
    {
        KillTween();
        _image.enabled = false;
    }

    private void KillTween ()
    {
        if (_shakeTween != null && _shakeTween.IsActive())
        {
            _shakeTween.Kill();
        }
    }

    private void FaceCamera ()
    {
        if(_camera != null)
        {
            transform.LookAt(_camera.transform);
            transform.localRotation = Quaternion.Euler(-transform.localRotation.eulerAngles.x, 0, 0);
        }
    }
}
