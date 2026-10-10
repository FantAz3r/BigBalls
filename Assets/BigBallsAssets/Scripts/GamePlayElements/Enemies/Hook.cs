using System;
using BigBalls.GameplayObjects;
using UnityEngine;

public class Hook : MonoBehaviour
{
    [field: SerializeField] public Collider Collider;

    public event Action<Collision> СatchedPlayer;
    public event Action<Collision> СatchedEnemy;

    private void OnCollisionEnter (Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out NetherButcher self))
        {
            return;
        }

        if (collision.gameObject.TryGetComponent(out Player player))
        {
            СatchedPlayer?.Invoke(collision);
        }

        if (collision.gameObject.TryGetComponent(out Enemy enemy))
        {
            СatchedEnemy?.Invoke(collision);
        }
    }
}
