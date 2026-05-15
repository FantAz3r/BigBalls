using UnityEngine;

namespace BigBalls.GameplayObjects
{
    public class Shooter
    {
        private readonly Transform _firePoint;

        public Shooter(Transform firePoint)
        {
            _firePoint = firePoint;
        }
    }
}