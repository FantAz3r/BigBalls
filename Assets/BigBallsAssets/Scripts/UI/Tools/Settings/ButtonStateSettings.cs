using System;
using UnityEngine;

[Serializable]
public class ButtonStateSettings
{
    [SerializeField] private Sprite _sprite;
    [SerializeField] private Color _color;
    [SerializeField] private DotweenAnimationSettings _animationSettings;

    public Sprite Sprite => _sprite;
    public Color Color => _color;
    public DotweenAnimationSettings AnimationSettings => _animationSettings;
}
