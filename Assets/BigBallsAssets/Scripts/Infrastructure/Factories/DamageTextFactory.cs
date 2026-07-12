using BigBalls.Services;
using UnityEngine;

public class DamageTextFactory
{
    private const string DamageTextPool = "DamageTextPool";

    private readonly IPoolService _poolService;

    public DamageTextFactory (IPoolService poolService)
    {
        _poolService = poolService;
    }

    public DamageText Create (Vector3 position, float damage)
    {
        DamageText damageText = _poolService.GetObject<DamageText>(DamageTextPool, position);
        damageText.SetDamageText((int) damage);
        damageText.Removed += OnRemoved;
        return damageText;
    }

    public void OnRemoved (DamageText damageText)
    {
        _poolService.ReleaseObject(damageText);
        damageText.Removed -= OnRemoved;
    }
}
