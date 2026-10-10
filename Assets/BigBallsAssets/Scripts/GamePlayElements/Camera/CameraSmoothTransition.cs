using System;
using DG.Tweening;
using UnityEngine;

public class CameraSmoothTransition : MonoBehaviour
{
    [Header("Настройки источника данных")]
    [SerializeField] private GameObject _cameraPrefab;

    [Header("Настройки анимации")]
    [SerializeField] private float _duration = 2.0f;
    [SerializeField] private Ease _easeType = Ease.InOutQuad;

    private Vector3 _targetPosition;
    private Quaternion _targetRotation;

    private void Awake ()
    {
        _targetPosition = transform.position;
        _targetRotation = transform.rotation;
    }

    public void MoveCamera (Action action)
    {
        transform.DOKill();
        transform.DORotateQuaternion(_targetRotation, _duration)
            .SetEase(_easeType);
        transform.DOMove(_targetPosition, _duration)
            .SetEase(_easeType)
            .OnComplete(() =>
            {
                action.Invoke();
            });
    }
}

