using System;
using UnityEngine;

namespace BigBalls.GameplayObjects
{
    public class Health: IDemageable
    {
        private readonly Stat _health;

        public Health(Stat health)
        {
            _health = health;
        }

        public event Action<float, float> IsValueChange;
        public event Action<float> DamageTaken;
        public event Action<float> Healed;

        public float MaxHealth { get; private set; }
        public bool IsAlive => _value >= 0 ;
        private float _value => _health.CurrentValue;


        public void TakeDamage(float damage)
        {
            if (IsAlive == false) return;

            if (_value <= 0) return;

            float damageTaken = Mathf.Min(damage, _value);

            _health.ChangeCurrentStat(_value - damageTaken);

            DamageTaken?.Invoke(damageTaken);
            IsValueChange?.Invoke(_value, MaxHealth);

            if (_value <= 0)
            {
                //Die
            }
        }
    }
}