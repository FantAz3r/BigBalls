using BigBalls.Services;
using UnityEngine;

public class DamageTextFactory
{
    private const string DamageTextPool = "DamageTextPool";
    private Color _color = Color.white;

    private readonly IPoolService _poolService;
    private readonly ISaveService _saveService;
    private readonly ICameraProvider _cameraProvider;
    private Vector3 _offset = new Vector3(0, 0.6f, 0);

    public DamageTextFactory (IPoolService poolService, ISaveService saveService, ICameraProvider cameraProvider)
    {
        _poolService = poolService;
        _saveService = saveService;
        _cameraProvider = cameraProvider;
    }

    public DamageText Create (Vector3 position, float damage, Color color = default)
    {
        if (_saveService.GameProgress.ShowDamageNumbers)
        {
            DamageText damageText = _poolService.GetObject<DamageText>(DamageTextPool, position + _offset);
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

