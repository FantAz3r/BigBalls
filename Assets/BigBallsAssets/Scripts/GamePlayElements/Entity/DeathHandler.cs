
using System.Collections.Generic;
using UnityEngine;

namespace BigBalls.GameplayObjects
{
    public class DeathHandler : ISubscribable
    {
        private readonly Stat _health;
        private readonly List<ISubscribable> _subscribables;

        public DeathHandler( Stat health, List<ISubscribable> subscribables)
        {
            _health = health;
            _subscribables = subscribables;
        }

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
                //Debug.Log("Die");
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
    }
}

