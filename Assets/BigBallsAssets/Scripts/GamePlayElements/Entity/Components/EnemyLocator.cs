using System.Collections.Generic;
using UnityEngine;
using Utils;

public class EnemyLocator : MonoBehaviour
{
    [SerializeField] private LayerMask _enemyLayer;
    [SerializeField] private float _radius;

    public Transform GetNearestEnemy (float radius = 0)
    {
        if (radius == 0)
            radius = _radius;

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius, _enemyLayer);
        Transform nearestEnemy = null;

        List<Transform> transforms = new List<Transform>();

        foreach (Collider collider in hitColliders)
        {
            transforms.Add(collider.gameObject.transform);
        }

        transforms.Remove(transform);

        nearestEnemy = Math.FindNearestEfficient(transforms, transform);
        return nearestEnemy;
    }


    private void OnDrawGizmosSelected ()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _radius);
    }
}

