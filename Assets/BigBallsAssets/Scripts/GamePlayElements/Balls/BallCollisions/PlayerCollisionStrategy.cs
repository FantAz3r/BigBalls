using BigBalls.GameplayObjects;
using UnityEngine;

public class PlayerCollisionStrategy : ICollisionStrategy
{
    public bool HandleCollision(Ball ball, Collision collision)
    {
        if (collision.gameObject.TryGetComponent<Player>(out _))
        {
            if (ball.CanReturnToBag)
            {
                ball.EventHandler.Return(ball);
            }

            return true;
        }

        return false;
    }
}
