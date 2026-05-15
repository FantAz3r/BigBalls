using System;
using System.Linq;
using UnityEngine;

namespace BigBalls.GameplayObjects
{
    public class Health
    {
        private readonly Stat _health;

        public Health(IStatContainer statContainer)
        {
            _health = statContainer.Stats.Where(s => s.StatType == StatType.Health).FirstOrDefault();
        }

        public float MaxHealth { get; private set; }
        public bool IsAlive => _health.Value >= 0 ;

        public event Action<float, float> IsValueChange, MaxHealthChanged;
        public event Action<float> DamageTaken, Healed;

        public void TakeDamage(float damage)
        {
            if (IsAlive == false) return;

            if (_health.Value <= 0) return;

            float damageTaken = Mathf.Min(damage, _health.Value);

            _health.ChangeStat(_health.Value - damageTaken);

            DamageTaken?.Invoke(damageTaken);
            IsValueChange?.Invoke(_health.Value, MaxHealth);

            if (_health.Value <= 0)
            {
                //Die
            }
        }
    }
}