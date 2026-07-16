using BigBalls.GameplayObjects;
using DG.Tweening;
using UnityEngine;

public class HitFlash : MonoBehaviour
{
    private static readonly int _baseColorID = Shader.PropertyToID("_BaseColor");

    [SerializeField] private Renderer[] _renderers;
    [SerializeField] private bool _enabled;
    [SerializeField] private int _flashCount;
    [SerializeField] private Color _flashColor = Color.white;
    [SerializeField] private float _totalDuration;

    private Stat _health;
    private Material[] _materials;
    private Color[] _baseColors;

    private void Awake()
    {
        _materials = new Material[_renderers.Length];
        _baseColors = new Color[_renderers.Length];

        for (int i = 0; i < _renderers.Length; i++)
        {
            _materials[i] = _renderers[i].material;
            _baseColors[i] = _materials[i].GetColor(_baseColorID);
        }
    }

    private void OnEnable()
    {
        if (_health != null)
            _health.ValueChanged += OnHit;
    }

    private void OnDisable()
    {
        if(_health != null)
            _health.ValueChanged -= OnHit;
    }

    public void Init(Stat stat)
    {
        _health = stat;
        _health.ValueChanged += OnHit;
    }

    private void OnHit(IReadonlyStat stat)
    {
        if (_enabled == false)
            return;

        float cycles = _flashCount * 2;
        float halfDuration = _totalDuration / cycles;

        for (int i = 0; i < _materials.Length; i++)
        {
            Material material = _materials[i];
            Color color = _baseColors[i];

            transform.DOKill(material);

            material.DOColor(color, _totalDuration)
                 .From(_flashColor)
                 .SetEase(Ease.Linear)
                 .SetLoops(_flashCount, LoopType.Yoyo)
                 .SetUpdate(true);
        }
    }
}
