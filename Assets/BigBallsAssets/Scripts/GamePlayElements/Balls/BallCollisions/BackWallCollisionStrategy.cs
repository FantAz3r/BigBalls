using BigBalls.GameplayObjects;
using UnityEngine;

public class BackWallCollisionStrategy : ICollisionStrategy
{
    private Player _player;

    public BackWallCollisionStrategy(Player playerProvider)
    {
        _player = playerProvider;
    }

    public bool HandleCollision(Ball ball, Collision collision)
    {
        if (collision.gameObject.TryGetComponent<BackWall>(out _))
        {
            ball.SetCanReturnToBag(true);

            if (_player != null)
            {
               
                ball.Mover.SetTarget(_player.transform);
            }

            return true;
        }

        return false;
    }
}
