using System;
using UnityEngine;

[Serializable]
public class ButtonStateSettings
{
    [SerializeField] private Sprite _sprite;
    [SerializeField] private Color _color;
    //[SerializeField] private AudioClip _clip;
    [SerializeField] private DotweenAnimationSettings _animationSettings;

    public Sprite Sprite => _sprite;
    public Color Color => _color;
    //public AudioClip Clip => _clip;
    public DotweenAnimationSettings AnimationSettings => _animationSettings;
}
