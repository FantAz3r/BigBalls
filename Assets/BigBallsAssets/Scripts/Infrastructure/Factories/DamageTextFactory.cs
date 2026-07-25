using BigBalls.Services;
using UnityEngine;

public class DamageTextFactory
{
    private const string DamageTextPool = "DamageTextPool";

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

    public DamageText Create (Vector3 position, float damage)
    {
        if (_saveService.GameProgress.ShowDamageNumbers)
        {
            DamageText damageText = _poolService.GetObject<DamageText>(DamageTextPool, position + _offset);
            damageText.Init(_cameraProvider);
            damageText.SetDamageText((int) damage);
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

