using System;
using System.Collections.Generic;
using UnityEngine;

namespace BigBalls.GameplayObjects
{
    public class DeathHandler : ISubscribable
    {
        private readonly Stat _health;
        private readonly List<ISubscribable> _subscribables;
        private readonly Transform _diebleObject;

        public DeathHandler(Stat health, List<ISubscribable> subscribables, Transform diebleObject)
        {
            _health = health;
            _subscribables = subscribables;
            _diebleObject = diebleObject;
        }

        public event Action<DeathHandler, Transform> Died;

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
            _diebleObject.gameObject.SetActive(false);
        }
    }
}

