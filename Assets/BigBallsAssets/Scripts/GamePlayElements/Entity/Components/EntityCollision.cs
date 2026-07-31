using System;
using BigBalls.GameplayObjects;
using UnityEngine;

public class EntityCollision : MonoBehaviour
{
    public event Action PlayerCollision;

    private void OnCollisionEnter (Collision collision)
    {
        if (collision.collider.TryGetComponent<Player>(out _))
        {
            PlayerCollision?.Invoke();
        }
    }
}
