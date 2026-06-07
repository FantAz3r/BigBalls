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
                Vector3 direction = (_player.transform.position - ball.transform.position).normalized;
                direction.y = 0;
                ball.Mover.SetDirection(new Vector2(direction.x, direction.z));
            }

            return true;
        }

        return false;
    }
}
