using System;
using BigBalls.GameplayObjects;
using UnityEngine;

public class EntityCollision : MonoBehaviour
{
    private bool _canCollision = true;
    public event Action PlayerCollision;

    private void OnCollisionEnter (Collision collision)
    {
        if (_canCollision)
        {
            if (collision.collider.TryGetComponent<Player>(out _))
            {
                PlayerCollision?.Invoke();
            }
        }
    }

    public void Enable ()
    {
        _canCollision = true;
    }

    public void Disable ()
    {
        _canCollision = false;
    }
}
