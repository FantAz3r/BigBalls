using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    [SerializeField] private float _moveUpDistance = 1f;
    [SerializeField] private float _animationDuration = 1f;
    [SerializeField] private float _fadeDuration = 0.5f;

    [SerializeField] private TMP_Text _text;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private Vector3 _offset = new Vector3(0, 0, -1);
    public event Action<DamageText> Removed;

    private void OnEnable ()
    {
        FaceCamera();

        _canvasGroup.alpha = 1f;
        _text.transform.localPosition = Vector3.zero;

        _text.transform.DOLocalMoveY(_moveUpDistance, _animationDuration).SetEase(Ease.OutCubic);
        _canvasGroup.DOFade(0, _fadeDuration).SetDelay(_animationDuration - _fadeDuration).OnComplete(() =>
        {
            Removed?.Invoke(this);
        });
    }

    private void FaceCamera ()
    {
        Camera mainCamera = Camera.main;

        if (mainCamera != null)
        {
            transform.LookAt(mainCamera.transform);
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, -180, transform.rotation.eulerAngles.z);
            transform.position += _offset;
        }
    }

    public void SetDamageText(int damage)
    {
        _text.text = damage.ToString();
    }
}
