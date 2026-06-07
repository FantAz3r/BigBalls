using System;
using System.Collections.Generic;
using UnityEngine;

namespace BigBalls.GameplayObjects
{
    public class DeathHandler<T> : ISubscribable where T : MonoBehaviour
    {
        private readonly Stat _health;
        private readonly List<ISubscribable> _subscribables;
        private readonly T _diebleObject;

        public DeathHandler(Stat health, List<ISubscribable> subscribables, T diebleObject)
        {
            _health = health;
            _subscribables = subscribables;
            _diebleObject = diebleObject;
        }

        public event Action<DeathHandler<T>, T> Died;

        public void Subscribe()
        {
            foreach (var item in _subscribables)
            {
                item.Subscribe();
            }

            _health.ValueChanged += HandleDeath;
        }

        private void HandleDeath(IReadonlyStat stat)
        {
            if (stat.CurrentValue <= stat.MinValue)
            {
                Died?.Invoke(this, _diebleObject);
            }
        }

        public void Unsubscribe()
        {
            foreach (var item in _subscribables)
            {
                item.Unsubscribe();
            }

            _health.ValueChanged -= HandleDeath;
        }

        public void Die()
        {
            Unsubscribe();

            if (_diebleObject is Transform transformObject)
            {
                transformObject.gameObject.SetActive(false);
            }
            
        }
    }
}