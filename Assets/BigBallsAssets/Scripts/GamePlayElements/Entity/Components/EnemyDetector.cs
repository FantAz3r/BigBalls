using BigBalls.GameplayObjects;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class EnemyDetector : MonoBehaviour
{
    private SphereCollider _sphereCollider;

    public bool HasEnemy { get; private set; }

    private void Awake ()
    {
        _sphereCollider = GetComponent<SphereCollider>();
    }

    private void OnTriggerEnter (Collider other)
    {
        if(other.TryGetComponent(out Enemy enemy))
        {
            HasEnemy = true;
        }
    }

    private void OnTriggerExit (Collider other)
    {
        if (other.TryGetComponent(out Enemy enemy))
        {
            HasEnemy = false;
        }
    }
}
