using BigBalls.GameplayObjects;
using UnityEngine;

public interface ICollisionStrategy
{
    bool HandleCollision(Ball ball, Collision collision);
}
