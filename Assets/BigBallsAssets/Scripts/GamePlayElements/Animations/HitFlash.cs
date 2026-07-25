using BigBalls.GameplayObjects;
using DG.Tweening;
using UnityEngine;

public class HitFlash : MonoBehaviour
{
    private static readonly int _emissionColorID = Shader.PropertyToID("_EmissionColor");

    [SerializeField] private Renderer[] _renderers;
    [SerializeField] private bool _enabled = true;
    [SerializeField] private int _flashCount = 1;
    [SerializeField] private Color _flashColor = Color.white;
    [SerializeField] private float _totalDuration = 0.2f;

    private Stat _health;
    private Material[] _materials;
    private Color[] _baseEmissionColors;

    private void Awake ()
    {
        _materials = new Material[_renderers.Length];
        _baseEmissionColors = new Color[_renderers.Length];

        for (int i = 0; i < _renderers.Length; i++)
        {
            _materials[i] = _renderers[i].material;
            _materials[i].EnableKeyword("_EMISSION");
            _baseEmissionColors[i] = _materials[i].GetColor(_emissionColorID);
        }
    }

    private void OnEnable ()
    {
        if (_health != null)
            _health.ValueChanged += OnHit;
    }

    private void OnDisable ()
    {
        if (_health != null)
            _health.ValueChanged -= OnHit;
    }

    public void Init (Stat stat)
    {
        _health = stat;

        if (_health != null)
            _health.ValueChanged += OnHit;
    }

    private void OnHit (IReadonlyStat stat)
    {
        if (_enabled == false)
            return;

        for (int i = 0; i < _materials.Length; i++)
        {
            Material material = _materials[i];
            Color targetColor = _baseEmissionColors[i];

            material.DOKill();
            material.DOColor(_flashColor, _emissionColorID, _totalDuration / (_flashCount * 2))
                 .From(targetColor)
                 .SetEase(Ease.InOutSine)
                 .SetLoops(_flashCount * 2, LoopType.Yoyo)
                 .OnComplete(() =>
                 {
                     material.SetColor(_emissionColorID, targetColor);
                 });
        }
    }
}
