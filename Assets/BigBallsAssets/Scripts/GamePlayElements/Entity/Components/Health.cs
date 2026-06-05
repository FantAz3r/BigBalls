using System;
using UnityEngine;

namespace BigBalls.GameplayObjects
{
    //public class Health : IDemageable
    //{
    //    private readonly Stat _stat;
    //
    //    public Health(Stat stat)
    //    {
    //        _stat = stat;
    //        _stat.ValueChanged += TakeDamage;
    //    }
    //
    //    public event Action<float, float> ValueChanged;
    //    public event Action<float> DamageTaken;
    //    public event Action<float> Healed;
    //    public event Action Died;
    //
    //    public float MaxHealth => _stat.MaxValue;
    //    public float CurrentValue => _stat.CurrentValue;
    //    public bool IsAlive => CurrentValue >= 0 ;
    //
    //    public void TakeDamage(float damage)
    //    {
    //        if (IsAlive == false) return;
    //
    //        if (CurrentValue <= 0) return;
    //
    //        float damageTaken = Mathf.Min(damage, CurrentValue);
    //
    //        _stat.ChangeCurrentStat(CurrentValue - damageTaken);
    //
    //        DamageTaken?.Invoke(damageTaken);
    //        ValueChanged?.Invoke(CurrentValue, MaxHealth);
    //
    //        if (CurrentValue <= 0)
    //        {
    //            Died?.Invoke();
    //        }
    //    }
    //}
}