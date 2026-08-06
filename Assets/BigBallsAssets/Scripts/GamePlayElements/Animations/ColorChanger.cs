using BigBalls.GameplayObjects;
using DG.Tweening;
using UnityEngine;
using System.Collections.Generic;

public class ColorChanger : MonoBehaviour
{
    private static readonly int _emissionColorID = Shader.PropertyToID("_EmissionColor");

    [SerializeField] private bool _enabled = true;
    [SerializeField] private int _flashCount = 1;
    [SerializeField] private Color _flashColor = Color.white;
    [SerializeField] private float _totalDuration = 0.2f;

    private Tween _colorTween;
    private Stat _health;
    private List<Material> _materials = new List<Material>();
    private List<Color> _baseEmissionColors = new List<Color>();
    [field: SerializeField] public Renderer[] Renderers { get; private set; }

    private void Awake ()
    {
        foreach (var renderer in Renderers)
        {
            if (renderer == null) continue;

            Material[] mats = renderer.materials;

            foreach (var mat in mats)
            {
                mat.EnableKeyword("_EMISSION");
                _materials.Add(mat);
                _baseEmissionColors.Add(mat.GetColor(_emissionColorID));
            }
        }
    }

    private void OnEnable ()
    {
        if (_health != null)
            _health.ValueChanged += OnHit;
    }

    private void OnDisable ()
    {
        _colorTween?.Kill();
        if (_health != null)
            _health.ValueChanged -= OnHit;
    }

    public void Init (Stat stat)
    {
        _health = stat;

        if (_health != null)
            _health.ValueChanged += OnHit;
    }

    private void OnHit (IReadonlyStat useles)
    {
        SwapColor(_flashColor, _totalDuration / (_flashCount * 2), _flashCount * 2);
    }

    public void SwapColor (Color color, float duration, int loops)
    {
        if (_enabled == false)
            return;

        for (int i = 0; i < _materials.Count; i++)
        {
            Material material = _materials[i];
            Color targetColor = _baseEmissionColors[i];

            material.SetColor(_emissionColorID, color);

            material.DOColor(color, _emissionColorID, duration)
                 .From(targetColor)
                 .SetLoops(loops, LoopType.Yoyo)
                 .OnKill(() =>
                 {
                     material.SetColor(_emissionColorID, targetColor);
                 });
        }
    }
}
