using System;
using BigBalls.GameplayObjects;
using UnityEngine;

public class Hook : MonoBehaviour
{
    [field: SerializeField] public Collider Collider;

    public event Action<Collision> CatchedPlayer;
    public event Action<Collision> CatchedEnemy;

    private void OnCollisionEnter (Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out NetherButcher self))
        {
            return;
        }

        if (collision.gameObject.TryGetComponent(out Player player))
        {
            CatchedPlayer?.Invoke(collision);
        }

        if (collision.gameObject.TryGetComponent(out Enemy enemy))
        {
            CatchedEnemy?.Invoke(collision);
        }
    }
}
