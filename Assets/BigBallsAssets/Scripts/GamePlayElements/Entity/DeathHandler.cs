using System;
using System.Collections.Generic;
using UnityEngine;

namespace BigBalls.GameplayObjects
{
    public class DeathHandler<T> : ISubscribable, ILouser
        where T : MonoBehaviour, IEntity
    {
        private readonly Stat _health;
        private readonly List<ISubscribable> _subscribables;
        private readonly T _diebleObject;
        
        public bool IsBallAttacked { get; private set; }

        public DeathHandler (Stat health, List<ISubscribable> subscribables, T diebleObject)
        {
            _health = health;
            _subscribables = subscribables;
            _diebleObject = diebleObject;
            IsBallAttacked = false;
        }

        public event Action Lost;

        public void Subscribe()
        {
            foreach (var item in _subscribables)
            {
                item.Subscribe();
            }

            _health.ValueChanged += HandleDeath;
        }

        public void Unsubscribe()
        {
            foreach (var item in _subscribables)
            {
                item.Unsubscribe();
            }

            _health.ValueChanged -= HandleDeath;
        }

        private void HandleDeath(IReadonlyStat stat)
        {
            if (stat.CurrentValue <= stat.MinValue)
            {
                IsBallAttacked = true;
                _diebleObject.EventHandler.Die(_diebleObject);
                Lost?.Invoke();
            }
        }
    }
}