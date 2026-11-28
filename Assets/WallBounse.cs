using System.Collections;
using System.Collections.Generic;
using BigBalls.GameplayObjects;
using UnityEngine;
[RequireComponent(typeof(Enemy))]
public class WallBounse : MonoBehaviour
{
    private Enemy _enemy;

    private void Start ()
    {
        _enemy = GetComponent<Enemy>();
    }

    private void OnCollisionEnter (Collision collision)
    {
        if(collision.gameObject.TryGetComponent(out Wall wall))
        {
            Vector3 currentDirection = new Vector3(_enemy.Mover.Direction.normalized.x, 0, _enemy.Mover.Direction.normalized.y);
            Vector3 normal = collision.contacts[0].normal;
            Vector3 reflectedDirection = Vector3.Reflect(currentDirection, normal);
            reflectedDirection.y = 0;
            reflectedDirection.Normalize();

            reflectedDirection += normal * 0.02f;
            reflectedDirection.Normalize();

            _enemy.Mover.SetDirection(new Vector2(reflectedDirection.x, reflectedDirection.z));
            _enemy.EventHandler.Reflect(_enemy);
        }
    }
}
