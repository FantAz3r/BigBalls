using BigBalls.Services;
using UnityEngine;

public class DamageTextFactory
{
    private const string DamageTextPool = "DamageTextPool";

    private readonly IPoolService _poolService;
    private readonly ISaveService _saveService;

    public DamageTextFactory(IPoolService poolService, ISaveService saveService)
    {
        _poolService = poolService;
        _saveService = saveService;
    }

    public DamageText Create(Vector3 position, float damage)
    {
        if (_saveService.GameProgress.ShowDamageNumbers)
        {
            DamageText damageText = _poolService.GetObject<DamageText>(DamageTextPool, position);
            damageText.SetDamageText((int)damage);
            damageText.Removed += OnRemoved;
            return damageText;
        }

        return null;
    }

    public void OnRemoved(DamageText damageText)
    {
        _poolService.ReleaseObject(damageText);
        damageText.Removed -= OnRemoved;
    }
}

