using BigBalls.GameplayObjects;
using UnityEngine;

namespace BigBalls.Factories
{
    public class HitPlayerStrategy : ICollisionStrategy
    {
        public bool HandleCollision(Ball ball, Collision collision)
        {
            if (collision.gameObject.TryGetComponent(out Player player))
            {
                ball.EventHandler.HitEntity(player);
                ball.EventHandler.Return(ball);

                return true;
            }

            return false;
        }
    }
}