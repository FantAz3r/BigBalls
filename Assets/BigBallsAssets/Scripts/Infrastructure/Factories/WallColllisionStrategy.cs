using BigBalls.GameplayObjects;
using UnityEngine;

namespace BigBalls.Factories
{
    internal class WallColllisionStrategy : ICollisionStrategy
    {
        public bool HandleCollision(Ball ball, Collision collision)
        {
            if (collision.gameObject.TryGetComponent<Wall>(out _))
            {
                ball.EventHandler.Return(ball);
                return true;
            }
            return true;
        }
    }
}