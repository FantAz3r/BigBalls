using UnityEngine;

namespace BigBalls.GameplayObjects
{
    public class Rotator
    {
        private readonly Transform _rotatebleObject;

        public Rotator(Transform rotatebleObject)
        {
            _rotatebleObject = rotatebleObject;
        }
    }
}