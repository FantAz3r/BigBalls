using BigBalls.GameplayObjects;
using UnityEngine;

public class Enemy : MonoBehaviour, IEntity
{
    public int Id { get; private set; }

    public void Construct(int id)
    {
        Id = id;
    }
}
