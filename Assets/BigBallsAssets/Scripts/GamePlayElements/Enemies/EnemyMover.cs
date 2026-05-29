using BigBalls.GameplayObjects;
using UnityEngine;

public class EnemyMover
{
    private readonly Mover _mover;

    public EnemyMover(Mover mover)
    {
        _mover = mover;
        _mover.SetDirection(Vector2.down);
    }
}
