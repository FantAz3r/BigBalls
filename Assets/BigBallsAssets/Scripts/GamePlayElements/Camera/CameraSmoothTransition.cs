using DG.Tweening;
using System;
using UnityEngine;

public class CameraSmoothTransition : MonoBehaviour
{
    [Header("Настройки источника данных")]
    [SerializeField] private GameObject cameraPrefab; // Префаб, из которого берем позицию/поворот

    [Header("Настройки анимации")]
    [SerializeField] private float duration = 2.0f;    // Длительность перехода
    [SerializeField] private Ease easeType = Ease.InOutQuad; // Тип плавности

    private Vector3 _targetPosition;
    private Quaternion _targetRotation;

    private void Awake()
    {
        _targetPosition = transform.position;
        _targetRotation = transform.rotation;
    }

    public void MoveCamera(Action action)
    {
        transform.DOKill();
        transform.DORotateQuaternion(_targetRotation, duration).SetEase(easeType);
        transform.DOMove(_targetPosition, duration).SetEase(easeType)
            .OnComplete(() =>
            {
                action.Invoke();
            });
    }
}

