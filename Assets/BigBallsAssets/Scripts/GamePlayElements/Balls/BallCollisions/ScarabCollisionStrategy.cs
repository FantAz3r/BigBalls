using BigBalls.GameplayObjects;
using UnityEngine;

public class ScarabCollisionStrategy : ICollisionStrategy
{
    public bool HandleCollision (Ball ball, Collision collision)
    {
        ball.EventHandler.Reflect(ball);
        return false;
    }

    public void HandleTrigger (Ball ball, Collider other)
    {
    }
}
