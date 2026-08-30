using BigBalls.Services;
using UnityEngine;

public class DamageTextFactory
{
    private Color _color = Color.white;

    private readonly IPoolService _poolService;
    private readonly ISaveService _saveService;
    private readonly ICameraProvider _cameraProvider;

    private DamageText _prefab;
    private Vector3 _offset = new Vector3(0, 0.6f, 0);

    public DamageTextFactory (IPoolService poolService, ISaveService saveService, ICameraProvider cameraProvider, IResourceLoader resourceLoader)
    {
        _poolService = poolService;
        _saveService = saveService;
        _cameraProvider = cameraProvider;
        _prefab = resourceLoader.Load<DamageText>();
    }

    public DamageText Create (Vector3 position, float damage, Color color = default)
    {
        if (_saveService.GameProgress.ShowDamageNumbers)
        {
            DamageText damageText = _poolService.GetObject(_prefab, position + _offset);
            damageText.Init(_cameraProvider);

            if (color == default)
            {
                damageText.SetDamageText((int) damage, _color);
            }
            else
            {
                damageText.SetDamageText((int) damage, color);
            }

            damageText.Removed += OnRemoved;
            return damageText;
        }

        return null;
    }

    public void OnRemoved (DamageText damageText)
    {
        _poolService.ReleaseObject(damageText);
        damageText.Removed -= OnRemoved;
    }
}

