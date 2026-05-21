using UnityEngine;

namespace BigBalls.GameplayObjects
{
    public class Shooter
    {
        private readonly Transform _firePoint;
        private Stat _damage;
        public Shooter(IStatContainer statContainer, Transform firePoint)
        {
            _firePoint = firePoint;
            _damage = statContainer.Get(StatType.Damage);
        }
    }
}