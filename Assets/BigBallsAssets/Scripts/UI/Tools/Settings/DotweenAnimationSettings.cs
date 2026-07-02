using System;
using DG.Tweening;
using UnityEngine;

[Serializable]
public struct DotweenAnimationSettings
{
    [Header("Scale")]
    [SerializeField] private bool _useScale;
    [SerializeField] private Vector3 _targetScale;

    [Header("Position Offset")]
    [SerializeField] private bool _usePosition;
    [SerializeField] private bool _enabledPositionChange;
    [SerializeField] private Vector2 _anchoredOffset;

    [Header("Rotation")]
    [SerializeField] private bool _useRotation;
    [SerializeField] private Vector3 _targetRotation;

    [Header("CanvasGroup properties")]
    [Tooltip("Requires CanvasGroup")]
    [SerializeField] private bool _enableCanvasGroupChange;
    [SerializeField] private bool _useFade;

    [Range(0f, 1f)]
    [SerializeField] private float _targetAlpha;
    [SerializeField] private bool _interactableWhenPlay;
    [SerializeField] private bool _blockRaycastWhenPlay;
    [SerializeField] private bool _ignoreParentGroupWhenPlay;

    [Header("Timing")]
    [SerializeField] private float _duration;
    [SerializeField] private Ease _ease;
    [SerializeField] private float _delay;

    public bool EnabledPositionChange => _enabledPositionChange;
    public bool EnabledCanvasGroupChange => _enableCanvasGroupChange;

    public bool InteractableWhenPlay => _interactableWhenPlay;
    public bool BlockRaycastWhenPlay => _blockRaycastWhenPlay;
    public bool IgnoreParentGroupWhenPlay => _ignoreParentGroupWhenPlay;
    public float Duration => _duration;
    public Ease Ease => _ease;
    public float Delay => _delay;

    public bool TryGetTargetScale(out Vector3 targetScale)
    {
        targetScale = _targetScale;

        return _useScale;
    }

    public bool TryGetAnchoredOffset(out Vector2 anchoredOffset)
    {
        anchoredOffset = _anchoredOffset;

        return _usePosition;
    }

    public bool TryGetTargetRotation(out Vector3 targetRotation)
    {
        targetRotation = _targetRotation;

        return _useRotation;
    }

    public bool TryGetTargetAlpha(out float targetAlpha)
    {
        targetAlpha = _targetAlpha;

        return _useFade;
    }

    public override string ToString()
    {
        string scale = _useScale ? _targetScale.ToString() : "none";
        string position = _usePosition ? _anchoredOffset.ToString() : "none";
        string rotation = _useRotation ? _targetRotation.ToString() : "none";
        string alpha = _useFade ? _targetAlpha.ToString("0.00") : "none";

        return $"{scale}, {position}, {rotation}," +
            $" {alpha}, {_duration:0.00}, {_ease}, {_delay:0.00}";
    }
}