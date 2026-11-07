using BigBalls.GameplayObjects;
using UnityEngine;

public interface ICollisionStrategy
{
    bool HandleCollision(Ball ball, Collision collision);
    void HandleTrigger (Ball ball, Collider other);
}
