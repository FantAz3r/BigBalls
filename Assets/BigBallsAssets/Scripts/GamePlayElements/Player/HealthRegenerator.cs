using BigBalls.Services;
using UnityEngine;

namespace BigBalls.GameplayObjects
{
    public class HealthRegenerator : IUpdateble
    {
        private readonly Stat _healthRegen;
        private readonly Stat _health;
        private readonly IUpdateService _updateService;

        private bool _canRegen;
        public HealthRegenerator(Stat healthRegen, Stat health, IUpdateService updateService)
        {
            _healthRegen = healthRegen;
            _health = health;
            _updateService = updateService;

            EnableRegen();
            updateService.Register(this);
        }

        public void EnableRegen() => _canRegen = true;
        
        public void DisableRegen() => _canRegen = false;

        public void Dispose()
        {
            DisableRegen();
            _updateService.Unregister(this);
        }

        public void Tick()
        {
            if (_canRegen)
            {
                if(_health.CurrentValue < _health.MaxValue)
                {
                    _health.AddCurrentValue(_healthRegen.CurrentValue * Time.deltaTime);
                }
            }
        }
    }
}