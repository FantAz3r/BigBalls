using BigBalls.GameplayObjects;
using UnityEngine;
using VContainer.Unity;

public class EnemyMover : IStartable
{
    private readonly Mover _mover;

    public EnemyMover(Mover mover)
    {
        _mover = mover;
    }

    public void Start()
    {
        _mover.SetDirection(Vector3.back);
    }
}
