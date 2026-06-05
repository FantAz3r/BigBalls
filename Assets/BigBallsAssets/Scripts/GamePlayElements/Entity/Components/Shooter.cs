using UnityEngine;

namespace BigBalls.GameplayObjects
{
    public class Shooter : ISubscribable
    {
        private readonly Transform _firePoint;
        private Stat _damage;
        public Shooter(Stat damage, Transform firePoint)
        {
            _firePoint = firePoint;
            _damage = damage;
        }

        public void Subscribe()
        {
        }

        public void Unsubscribe()
        {
        }
    }
}