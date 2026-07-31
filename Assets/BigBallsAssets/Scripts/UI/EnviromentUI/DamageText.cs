using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private float _animationDuration = 1.2f;
    [SerializeField] private float _maxHorizontalSpread = 2f;
    [SerializeField] private float _maxVerticalHeight = 2f;
    [SerializeField] private float _startScale = 0.5f;
    [SerializeField] private float _endScale = 1.2f;

    [Header("References")]
    [SerializeField] private TMP_Text _text;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private Vector3 _offset = new Vector3(0, 0, -1);

    public event Action<DamageText> Removed;
    private Camera _camera;
    private Vector3 _startLocalPos;

    private void OnEnable ()
    {
        _canvasGroup.alpha = 1f;
        transform.localScale = Vector3.one * _startScale;
    }

    public void Init (ICameraProvider cameraProvider)
    {
        _camera = cameraProvider.Camera;
    }

    public void SetDamageText (int damage)
    {
        _text.text = damage.ToString();

        float randomXDir = UnityEngine.Random.Range(-1f, 1f);
        float targetX = randomXDir * _maxHorizontalSpread;
        float peakY = UnityEngine.Random.Range(_maxVerticalHeight * 0.5f, _maxVerticalHeight);

        FaceCamera();
        PlayParabolaAnimation(targetX, peakY);
    }

    private void PlayParabolaAnimation (float targetX, float peakY)
    {
        _startLocalPos = transform.localPosition + _offset;
        float elapsed = 0f;

        DOTween.To(() => elapsed, x => elapsed = x, _animationDuration, _animationDuration)
            .SetEase(Ease.Linear)
            .OnUpdate(() =>
            {
                float t = elapsed / _animationDuration;
                float currentY = 4 * peakY * t * (1 - t);
                float currentX = targetX * t;

                transform.localPosition = _startLocalPos + new Vector3(currentX, currentY, 0);

                float scaleFactor = Mathf.Lerp(_startScale, _endScale, Mathf.Sin(t * Mathf.PI));
                transform.localScale = Vector3.one * scaleFactor;

                if (t > 0.5f)
                {
                    float fadeT = (t - 0.5f) * 2f;
                    _canvasGroup.alpha = Mathf.Lerp(1f, 0f, fadeT);
                }
            })
            .OnComplete(() =>
            {
                Removed?.Invoke(this);
            });
    }

    private void FaceCamera ()
    {
        transform.LookAt(_camera.transform);
        transform.localRotation = Quaternion.Euler(-transform.localRotation.eulerAngles.x, 0, 0);
    }
}

