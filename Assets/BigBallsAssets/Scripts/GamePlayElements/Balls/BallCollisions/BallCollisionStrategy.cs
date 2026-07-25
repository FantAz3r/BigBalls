using System.Collections;
using System.Collections.Generic;
using BigBalls.GameplayObjects;
using UnityEngine;

public class BallCollisionStrategy : ICollisionStrategy
{
    public bool HandleCollision (Ball ball, Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out Ball otherBall))
        {
            if (otherBall.gameObject.layer == ball.gameObject.layer)
            {
                return false;
            }

            ball.EventHandler.HitEntity(otherBall);
            ball.EventHandler.Return(ball);
            return true;
        }

        return false;
    }
}
