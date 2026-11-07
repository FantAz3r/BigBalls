using BigBalls.GameplayObjects;
using BigBalls.Services;
using UnityEngine;
using VContainer;

public class BackWallCollisionStrategy : ICollisionStrategy
{
    private Player _player;

    [Inject]
    public void Construct(IPlayerProvider playerProvider)
    {
        _player = playerProvider.Player;
    }

    public bool HandleCollision (Ball ball, Collision collision)
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

    public void HandleTrigger (Ball ball, Collider other)
    {
    }
}
